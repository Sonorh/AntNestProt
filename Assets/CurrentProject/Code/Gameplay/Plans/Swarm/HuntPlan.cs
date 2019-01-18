using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class HuntPlan : AbAvatarPlan
	{
		public static bool CheckExecutable(PlayerManager player, AbCellData data, AntSwarm ants)
		{
			return player.Anter.HasFreeAnts(ants) 
				&& player.Anter.CalculateFightPower(ants) > 0
				&& !(data as EnviromentCellData).IsOccupied
				&& (data as EnviromentCellData).IsBugged;
		}

		public HuntPlan(PlayerManager player, AbCellData target, AntSwarm ants)
		{
			this.Player = player;
			this.PlanType = PlanTypes.Hunt;
			this.Ants = ants;
			this.TargetCell = target;
			this.Delay = 0;

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
			if (tg != null && tg.IsBugged)
			{
				var bug = AbBug.Create(tg.Bug, tg.BugCount);
				if (bug.Fight(Ants))
				{
					tg.Bug = BugTypes.None;
					Player.Resourcer.Store(ResourceTypes.Meat, bug.Reward);
					Success();
					return;
				}
			}

			Fail();
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