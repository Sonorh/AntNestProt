using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class SpawnUnitPlan : AbPlayerPlan
	{
		public static bool CheckExecutable(PlayerManager player, AntSwarm swarm)
		{
			if (swarm.Ants.Count > 1)
			{
				return false;
			}

			bool isLarvaSpawn = swarm.Ants.ContainsKey(AntTypes.Larva);
			return player.Resourcer.IsEnough(ResourceTypes.Meat, 10) && player.Anter.HasPlaceForAnts(swarm) && (isLarvaSpawn || player.Anter.HasFreeAnts(AntTypes.Larva, swarm.AntCount));
		}

		private bool isLarva;

		public SpawnUnitPlan(PlayerManager player, AntTypes type)
		{
			this.Player = player;
			this.PlanType = PlanTypes.CreateUnit;
			this.isLarva = type == AntTypes.Larva;
			this.Ants = new AntSwarm(player, type, 1);
			this.Delay = isLarva ? 2 : 1;
		}

		public override bool IsExecutable
		{
			get
			{
				return CheckExecutable(Player, Ants);
			}
		}

		public override void Prepare()
		{
			Player.Resourcer.Spend(ResourceTypes.Meat, 10);
			if (!isLarva)
			{
				Player.Anter.AntDecline(AntTypes.Larva, Ants.AntCount);
			}
		}

		public override void Execute()
		{
			if (!Player.Anter.HasPlaceForAnts(Ants))
			{
				Fail();
				return;
			}

			Player.Anter.AntIncome(Ants);
			Success();
		}

		public override void Success()
		{
			base.Success();
		}

		public override void Cancel()
		{
			Player.Resourcer.Store(ResourceTypes.Meat, 10);
			if (!isLarva)
			{
				Player.Anter.AntIncome(AntTypes.Larva, Ants.AntCount);
			}

			base.Cancel();
		}

		public override void Fail()
		{
			if (!isLarva)
			{
				Player.Anter.AntIncome(AntTypes.Larva, Ants.AntCount);
			}

			base.Fail();
		}
	}
}