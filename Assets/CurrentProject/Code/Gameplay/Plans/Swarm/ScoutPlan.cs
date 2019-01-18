using PM.UsefulThings.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class ScoutPlan : AbAvatarPlan
	{
		public static bool CheckExecutable(PlayerManager player, AbCellData target, AntSwarm swarm)
		{
			return player.Anter.HasFreeAnts(swarm)
					&& swarm.AntCount > 0
					&& player.Anter.CalculateFightPower(swarm) >= 3
				    && MapManager.Instance.GetPath(player.Nest, target).Count > 0;
		}

		public ScoutPlan(PlayerManager player, AbCellData target, AntSwarm swarm)
		{
			this.Player = player;
			this.PlanType = PlanTypes.Scout;
			this.TargetCell = target;
			this.Ants = swarm;
			this.Delay = 1;

			this.Ants.Position = Player.Nest;
		}

		public override bool IsExecutable
		{
			get
			{
				return CheckExecutable(Player, TargetCell, Ants) && base.IsExecutable;
			}
		}

		public override void Execute()
		{
			if (Ants.Position != TargetCell)
			{
				MoveToTarget();
				return;
			}

			var tg = (TargetCell as EnviromentCellData);
			if (tg != null)
			{
				TargetCell.Explore();
				Success();
				return;
			}

			Fail();
		}

		public override void Success()
		{
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

			antAvatar.transform.position = MapManager.Instance.Field[Ants.Position.FieldPosition].ScreenPosition;

			base.Prepare();
		}
	}
}