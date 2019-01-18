using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class UpgradeRoomPlan : AbPlayerPlan
	{
		public static bool CheckExecutable(PlayerManager player, AbNestRoomData room)
		{
			return player.Resourcer.IsEnough(ResourceTypes.Meat, 10) && room.CanUpgrade;
		}

		public AbNestRoomData TargetRoom;

		public UpgradeRoomPlan(PlayerManager player, Vector2Int pos)
		{
			this.Player = player;
			this.PlanType = PlanTypes.CreateUnit;
			this.TargetRoom = player.Nester.Rooms[pos];
			this.Delay = 2;
		}

		public override bool IsExecutable
		{
			get
			{
				return CheckExecutable(Player, TargetRoom);
			}
		}

		public override void Prepare()
		{
			Player.Resourcer.Spend(ResourceTypes.Meat, 10);
			Player.Nester.SetUpgradeRoom(TargetRoom, true);
		}

		public override void Execute()
		{
			Player.Nester.SetUpgradeRoom(TargetRoom, false);
			Success();
		}

		public override void Success()
		{
			base.Success();
		}

		public override void Cancel()
		{
			Player.Resourcer.Store(ResourceTypes.Meat, 10);
			Player.Nester.SetUpgradeRoom(TargetRoom, false);

			base.Cancel();
		}

		public override void Fail()
		{
			Player.Resourcer.Store(ResourceTypes.Meat, 10);
			Player.Nester.SetUpgradeRoom(TargetRoom, false);

			base.Fail();
		}
	}
}