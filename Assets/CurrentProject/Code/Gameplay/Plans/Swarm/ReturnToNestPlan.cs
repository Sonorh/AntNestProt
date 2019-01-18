using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class ReturnToNestPlan : AbAvatarPlan
	{
		public static bool CheckExecutable(PlayerManager player, AntSwarm ants)
		{
			return ants.AntCount > 0;
		}

		public ReturnToNestPlan(PlayerManager player, AntSwarm swarm)
		{
			this.Player = player;
			this.PlanType = PlanTypes.Going;
			this.TargetCell = player.Nest;
			this.Ants = swarm;
			this.Delay = 1;
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
			if (Ants.Position != TargetCell)
			{
				MoveToTarget();
			}

			if (Ants.Position == TargetCell)
			{
				Player.Anter.ComeHome(Ants);

				Success();
			}
		}

		public override void Prepare()
		{
			SpawnAvatar();

			antAvatar.transform.position = MapManager.Instance.Field[Ants.Position.FieldPosition].ScreenPosition;

			base.Prepare();

			if (Ants.Position == TargetCell)
			{
				Player.Anter.ComeHome(Ants);

				Success();
			}
		}
	}
}