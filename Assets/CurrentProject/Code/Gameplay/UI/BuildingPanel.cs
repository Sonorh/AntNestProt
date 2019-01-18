using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class BuildingPanel : AbPanel
	{
		public System.Action<RoomTypes> OnBuild;

		private readonly ListProperty buildingList = new ListProperty();
		private readonly BoolProperty isBuildCheckPassed = new BoolProperty();

		private List<AntTypes> availableTypes = new List<AntTypes>
		{
			AntTypes.Queen,
			AntTypes.Larva,
			AntTypes.Drone,
			AntTypes.Worker,
			AntTypes.Warrior,
			AntTypes.Scout,
			AntTypes.Gatherer,
			AntTypes.Builder,
			AntTypes.Bruiser
		};
		private AntTypes? defaultType = null;
		private int defaultCount = -1;
		private Func<AntSwarm, bool> sendCheck;

		private void Start()
		{
			buildingList.OnSelectionChanged += BuildingTypeSelectionChangedHandler;

			FillBuildingTypes();
		}

		private void BuildingTypeSelectionChangedHandler(BaseListElementData element)
		{
			var typeData = element as BuildingTypeData;

			var config = GameplayHolderManager.Instance.Configs.BuildingConfigs.Find((x) => x.Type == typeData.Type);
			if (config == null)
			{
				isBuildCheckPassed.value = false;
				return;
			}

			var resourcer = GameplayManager.Instance.Player.Resourcer;
			isBuildCheckPassed.value = resourcer.IsEnough(ResourceTypes.Fruit, config.FruitCost) && resourcer.IsEnough(ResourceTypes.Meat, config.MeatCost);
		}

		private void FillBuildingTypes()
		{
			var elements = new List<BaseListElementData>();

			foreach (var item in Enum.GetValues(typeof(RoomTypes)))
			{
				var type = (RoomTypes)item;
				if ((int)type > 2)
				{
					var config = GameplayHolderManager.Instance.Configs.BuildingConfigs.Find((x) => x.Type == type);
					if (config == null)
					{
						continue;
					}

					var elem = new BuildingTypeData(type, config.FruitCost, config.MeatCost);
					elements.Add(elem);
				}
			}

			buildingList.value = elements;

			elements[0].Selected = true;
		}

		public void Build()
		{
			if (!isBuildCheckPassed.value)
			{
				return;
			}

			OnBuild?.Invoke((buildingList.GetSelected() as BuildingTypeData).Type);
			Close();
		}

		public void Cancel()
		{
			Close();
		}
	}
}