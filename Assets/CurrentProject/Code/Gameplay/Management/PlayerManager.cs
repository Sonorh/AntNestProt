using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class PlayerManager
	{
		public NestCellData Nest;

		public PlanManager Planer = new PlanManager();
		public ResourceManager Resourcer = new ResourceManager();
		public AntManager Anter = new AntManager();
		public NestManager Nester = new NestManager();
		private bool isLost = false;

		public PlayerManager()
		{
			Anter.Owner = this;

			TimeManager.Instance.OnEndOfTurn += OnEndOfTurnHandler;
			TimeManager.Instance.OnTimeChanged += OnTimeChangedHandler;
			TimeManager.Instance.OnStartTurn += OnStartTurnHandler;

			foreach (var item in System.Enum.GetValues(typeof(AntTypes)))
			{
				var type = (AntTypes)item;
				var config = GameplayHolderManager.Instance.Configs.AntConfigs.Find((x) => x.Type == type);

				Anter.AntConfigs.Add(type, config);
			}

			Nester.OnNestRebuilt += OnNestRebuiltHandler;
			OnNestRebuiltHandler();

			GameplayManager.Instance.OnGameplayOver += OnGameplayOver;
		}

		public void OnGameplayOver()
		{
			GameplayManager.Instance.OnGameplayOver -= OnGameplayOver;

			Nester.OnNestRebuilt -= OnNestRebuiltHandler;

			TimeManager.Instance.OnEndOfTurn -= OnEndOfTurnHandler;
			TimeManager.Instance.OnTimeChanged -= OnTimeChangedHandler;
			TimeManager.Instance.OnStartTurn -= OnStartTurnHandler;
		}

		private void OnNestRebuiltHandler()
		{
			var warriorCapacity = Nester.Rooms.Count(x => x.Type == RoomTypes.Armory) * 20;
			var workerCapacity = 30 + Nester.Rooms.Count(x => x.Type == RoomTypes.Nursery) * 20;
			var storageCapacity = 100 + (Nester.Rooms.Count(x => x.Type == RoomTypes.Warehause) - 1) * 100;

			Anter.AntMaxCapacity[AntTypes.Warrior] = warriorCapacity;
			Anter.AntMaxCapacity[AntTypes.Worker] = workerCapacity;
			Anter.AntMaxCapacity[AntTypes.Larva] = 50;

			Anter.Update();

			Resourcer.StorageMaxCapacity = storageCapacity;

			Resourcer.Update();
		}

		private void OnEndOfTurnHandler()
		{
			if (Resourcer.IsEnough(ResourceTypes.Meat, Anter.LarvaCount))
			{
				Resourcer.Spend(ResourceTypes.Meat, Anter.LarvaCount);
			}
			else
			{
				var meat = Resourcer.Meat;
				Resourcer.Spend(ResourceTypes.Meat, Resourcer.Meat);
				Anter.AntDecline(AntTypes.Larva, Anter.GetFreeAnts(AntTypes.Larva) - meat);
			}

			if (Resourcer.IsEnough(ResourceTypes.Fruit, Anter.Count / 5))
			{
				Resourcer.Spend(ResourceTypes.Fruit, Anter.Count / 5);
			}
			else
			{
				Resourcer.Spend(ResourceTypes.Fruit, Resourcer.Fruit);
				if (!isLost)
				{
					isLost = true;
					Debug.Log("NO FRUITS! YOU LOSE!");
					WindowManagerUT.Instance.OpenNewPanel<GameOverPanel>(WindowCloseModes.CloseNothing);
				}
			}
		}

		private void OnTimeChangedHandler()
		{
			if (TimeManager.Instance.IsWinter)
			{
				foreach (var swarm in Anter.AntsOnFields)
				{
					swarm.Die();
				}
				Anter.AntsOnFields.Clear();
			}

			Planer.UpdatePlans();
		}

		private void OnStartTurnHandler()
		{
			if (Anter.Count == 0)
			{
				if (!isLost)
				{
					isLost = true;
					Debug.Log("NO Ants! YOU LOSE!");
					WindowManagerUT.Instance.OpenNewPanel<GameOverPanel>(WindowCloseModes.CloseNothing);
				}
			}
		}
	}
}