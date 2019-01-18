using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class BuildRoomPlan : AbPlayerPlan
	{
		public static bool CheckExecutable(PlayerManager player, RoomTypes type, Vector2Int pos, BaseBuildingConfig config)
		{
			return config != null && player.Resourcer.IsEnough(ResourceTypes.Fruit, config.FruitCost)
					&& player.Resourcer.IsEnough(ResourceTypes.Meat, config.MeatCost) && player.Nester.CanBuild(type, pos);
		}

		public RoomTypes TargetType;
		public Vector2Int TargetPosition;
		public BaseBuildingConfig Config;

		public BuildRoomPlan(PlayerManager player, RoomTypes type, Vector2Int pos)
		{
			this.Player = player;
			this.PlanType = PlanTypes.CreateUnit;

			this.TargetType = type;
			this.TargetPosition = pos;

			this.Config = GameplayHolderManager.Instance.Configs.BuildingConfigs.Find((x) => x.Type == type);


			this.Delay = 2;
		}

		public override bool IsExecutable
		{
			get
			{
				return CheckExecutable(Player, TargetType, TargetPosition, Config);
			}
		}

		public override void Prepare()
		{
			Player.Resourcer.Spend(ResourceTypes.Fruit, Config.FruitCost);
			Player.Resourcer.Spend(ResourceTypes.Meat, Config.MeatCost);
			Player.Nester.AddConstruction(TargetType, TargetPosition);
		}

		public override void Execute()
		{
			Player.Nester.Build(TargetType, TargetPosition);

			Success();
		}

		public override void Success()
		{
			base.Success();
		}

		public override void Cancel()
		{
			Player.Resourcer.Store(ResourceTypes.Meat, Config.FruitCost / 2);
			Player.Resourcer.Store(ResourceTypes.Meat, Config.FruitCost / 2);
			Player.Nester.RemoveConstruction(TargetPosition);

			base.Cancel();
		}

		public override void Fail()
		{
			Player.Resourcer.Store(ResourceTypes.Meat, Config.FruitCost / 2);
			Player.Resourcer.Store(ResourceTypes.Meat, Config.FruitCost / 2);
			Player.Nester.RemoveConstruction(TargetPosition);

			base.Fail();
		}
	}
}