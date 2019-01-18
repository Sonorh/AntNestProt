using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class GatherPlan : AbAvatarPlan
	{
		public static bool CheckExecutable(PlayerManager player, AntSwarm swarm)
		{
			return player.Anter.HasFreeAnts(swarm) && player.Anter.CalculateGatherPower(swarm) > 0;
		}

		private bool isGathered = false;

		public GatherPlan(PlayerManager player, AbCellData target, AntSwarm ants)
		{
			this.Player = player;
			this.PlanType = PlanTypes.Gather;
			this.Ants = ants;
			this.TargetCell = target;
			this.Delay = 1;

			this.Ants.Position = Player.Nest;
		}

		public override bool IsExecutable
		{
			get
			{
				return CheckExecutable(Player, Ants) && base.IsExecutable;
			}
		}

		public override void Execute()
		{
			var cell = (TargetCell as EnviromentCellData);
			if (cell == null || cell.Source == null || cell.Source.IsDepleted)
			{
				Fail();
				return;
			}

			if (Ants.Position != TargetCell)
			{
				MoveToTarget();
				return;
			}

			TargetCell.Explore();

			if (!cell.IsGatherable)
			{
				if (isGathered)
				{
					Success();
				}
				else
				{
					Fail();
				}
				return;
			}
			isGathered = true;

			var source = cell.Source;

			var power = Player.Anter.CalculateGatherPower(Ants);
			var amount = source.Gather(power);
			Player.Resourcer.Store(source.ResourceType, amount);

			if (source.IsDepleted)
			{
				Success();
			}
		}

		public override void Success()
		{
			TargetCell.Explore();
			base.Success();

			var plan = new ReturnToNestPlan(Player, Ants);
			Player.Planer.TryToAddPlan(plan);
		}

		public override void Cancel()
		{
			base.Cancel();

			var plan = new ReturnToNestPlan(Player, Ants);
			Player.Planer.TryToAddPlan(plan);
		}

		public override void Fail()
		{
			base.Fail();

			var plan = new ReturnToNestPlan(Player, Ants);
			Player.Planer.TryToAddPlan(plan);
		}


		public override void Prepare()
		{
			Player.Anter.ToTheField(Ants);

			SpawnAvatar();
			Ants.Position = Player.Nest;

			antAvatar.transform.position = MapManager.Instance.Field[Player.Nest.FieldPosition].ScreenPosition;

			base.Prepare();
		}
	}
}