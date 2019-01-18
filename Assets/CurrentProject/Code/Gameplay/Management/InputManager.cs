using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class InputManager : SoftMonoSingleton<InputManager>
	{
		/// Possible Actions:
		/// click on enviroment cell
		/// /// empty
		/// /// has source
		/// /// has bug
		/// /// occupied by enemies
		/// 
		/// click on Nest
		/// /// our nest
		/// /// enemy nest

		private AntSwarm selectedSwarm;

		private void Start()
		{
			TimeManager.Instance.OnStartTurn += OnTimePass;
		}

		private void Update()
		{
			var x = Input.GetAxis("Horizontal");
			var y = Input.GetAxis("Vertical");

			x *= Time.deltaTime * 10;
			y *= Time.deltaTime * 10;

			Camera.main.transform.position += new Vector3(x, y);

			if (Input.GetButtonDown("Jump"))
			{
				Camera.main.transform.position = Vector3.back * 10;
			}
		}

		private void OnTimePass()
		{
			UpdatePath(false);
		}

		public void OnCellClick(AbCellData cellData)
		{
			if (cellData is NestCellData)
			{
				var data = cellData as NestCellData;

				//todo: get player?
				if (data == GameplayManager.Instance.Player.Nest)
				{
					MapManager.Instance.Hide();
					WindowManagerUT.Instance.OpenNewPanel<NestPanel>();
				}
				else
				{
					// enemy nest. attack options.
				}
			}
			else
			{
				var data = cellData as EnviromentCellData;

				if (data.IsAllyArmy)
				{
					if (selectedSwarm != data.Occupant)
					{
						UpdatePath(false);
						selectedSwarm = data.Occupant;
						UpdatePath(true);
						return;
					}
				}

				if (selectedSwarm != null)
				{
					var selectedPlan = GameplayManager.Instance.Player.Planer.Plans.Find(x => x.Ants == selectedSwarm);
					if (selectedPlan != null && selectedPlan.TargetCell == data)
					{
						UpdatePath(false);

						selectedPlan.Cancel();

						return;
					}
				}

				UpdatePath(false);


				if (!data.isExplored)
				{
					//show gather popup
					var types = new List<AntTypes>
					{
						AntTypes.Worker,
						AntTypes.Gatherer,
						AntTypes.Builder,
						AntTypes.Warrior,
						AntTypes.Scout,
						AntTypes.Stopper,
					};

					if (GameplayManager.Instance.Player.Planer.CheckCell(data))
					{
						var actionPanel = WindowManagerUT.Instance.OpenNewPanel<ForageActionPanel>(WindowCloseModes.CloseNothing);
						System.Func<AntSwarm, bool> check = (s) => { return ScoutPlan.CheckExecutable(GameplayManager.Instance.Player, cellData, s); };
						actionPanel.SetParameters(types, AntTypes.Worker, 3, check);
						actionPanel.OnSend = (swarm =>
						{
							var plan = new ScoutPlan(GameplayManager.Instance.Player, data, swarm);
							GameplayManager.Instance.Player.Planer.TryToAddPlan(plan);
						});
					}
				}
				else if (data.IsEmpty)
				{
					//show smth like "you can't do a shit here" message
				}
				else if (data.IsAllyArmy)
				{
					//show ally popup;
				}
				else if (data.IsEnemyArmy)
				{
					//show enemy popup;
				}
				else if (data.IsGatherable)
				{
					//show gather popup
					var types = new List<AntTypes>
					{
						AntTypes.Worker,
						AntTypes.Gatherer,
						AntTypes.Builder,
						AntTypes.Warrior,
						AntTypes.Scout,
						AntTypes.Stopper,
					};

					if (GameplayManager.Instance.Player.Planer.CheckCell(data))
					{
						var actionPanel = WindowManagerUT.Instance.OpenNewPanel<ForageActionPanel>(WindowCloseModes.CloseNothing);
						System.Func<AntSwarm, bool> check = (s) => { return GatherPlan.CheckExecutable(GameplayManager.Instance.Player, s); };
						actionPanel.SetParameters(types, AntTypes.Worker, 10, check);
						actionPanel.OnSend = (swarm =>
							{
								var plan = new GatherPlan(GameplayManager.Instance.Player, data, swarm);
								GameplayManager.Instance.Player.Planer.TryToAddPlan(plan);
							});
					}
				}
				else if (data.IsBugged)
				{
					//show fight popup
					var types = new List<AntTypes>
					{
						AntTypes.Warrior,
						AntTypes.Scout,
						AntTypes.Stopper,
					};

					if (GameplayManager.Instance.Player.Planer.CheckCell(data))
					{
						var actionPanel = WindowManagerUT.Instance.OpenNewPanel<ForageActionPanel>(WindowCloseModes.CloseNothing);
						System.Func<AntSwarm, bool> check = (s) => { return HuntPlan.CheckExecutable(GameplayManager.Instance.Player, data, s); };
						actionPanel.SetParameters(types, AntTypes.Warrior, 5, check);
						actionPanel.OnSend = (swarm =>
						{
							var plan = new HuntPlan(GameplayManager.Instance.Player, data, swarm);
							GameplayManager.Instance.Player.Planer.TryToAddPlan(plan);
						});
					}
				}
			}
		}

		private void UpdatePath(bool isShow)
		{
			if (selectedSwarm == null)
			{
				return;
			}

			var plan = GameplayManager.Instance.Player.Planer.Plans.Find(x => x.Ants == selectedSwarm);

			if (plan == null)
			{
				return;
			}

			var path = MapManager.Instance.GetPath(plan.Ants.Position, plan.TargetCell);

			if (isShow)
			{
				if (path.Count > 0)
				{
					foreach (var pos in path)
					{
						MapManager.Instance.Field[pos].SetColor(new Color32(100, 100, 255, 255));
					}
				}
				MapManager.Instance.Field[plan.TargetCell.FieldPosition].SetColor(new Color32(0, 0, 255, 255));
			}
			else
			{
				if (path.Count > 0)
				{
					foreach (var pos in path)
					{
						MapManager.Instance.Field[pos].UpdateInfo();
					}
				}
				MapManager.Instance.Field[plan.TargetCell.FieldPosition].UpdateInfo();
			}

			if (!isShow)
			{
				selectedSwarm = null;
			}
		}

		public void EndTurn()
		{
			TimeManager.Instance.Next();
		}

		public void Hibernate()
		{
			foreach (var swarm in GameplayManager.Instance.Player.Anter.AntsOnFields)
			{
				swarm.Die();
			}
			GameplayManager.Instance.Player.Anter.AntsOnFields.Clear();
			foreach (var player in GameplayManager.Instance.Enemies)
			{
				foreach (var swarm in player.Anter.AntsOnFields)
				{
					swarm.Die();
				}
				player.Anter.AntsOnFields.Clear();
			}
			TimeManager.Instance.ScrollTo(Weeks.MarchFirst);
		}

		public void OnRoomClick(AbNestRoomData roomData)
		{
			switch (roomData.Type)
			{
				case RoomTypes.Entrance:
					break;
				case RoomTypes.BuildingSpot:
					var actionPanel = WindowManagerUT.Instance.OpenNewPanel<BuildingPanel>(WindowCloseModes.CloseNothing);
					actionPanel.OnBuild = (type =>
					{
						var plan = new BuildRoomPlan(GameplayManager.Instance.Player, type, roomData.Position);
						GameplayManager.Instance.Player.Planer.TryToAddPlan(plan);
					});
					break;
				case RoomTypes.Construction:
					var planToRemove = GameplayManager.Instance.Player.Planer.Plans.Find(x => (x is BuildRoomPlan) && (x as BuildRoomPlan).TargetPosition == roomData.Position);
					planToRemove?.Cancel();
					break;
				case RoomTypes.Warehause:
					return;
					var warehouseUpgradePlan = new UpgradeRoomPlan(GameplayManager.Instance.Player, roomData.Position);
					GameplayManager.Instance.Player.Planer.TryToAddPlan(warehouseUpgradePlan);
					break;
				case RoomTypes.Nursery:
					return;
					var nurseryUpgradePlan = new UpgradeRoomPlan(GameplayManager.Instance.Player, roomData.Position);
					GameplayManager.Instance.Player.Planer.TryToAddPlan(nurseryUpgradePlan);
					break;
				case RoomTypes.Armory:
					break;
				default:
					break;
			}
		}
	}
}