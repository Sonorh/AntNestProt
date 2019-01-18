using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.Gameplay
{
	public class NestPanel : AbPanel
	{
		public ListProperty ants = new ListProperty();
		public StringProperty time = new StringProperty();
		public IntProperty fruits = new IntProperty();
		public IntProperty meat = new IntProperty();
		public IntProperty storageCapacity = new IntProperty();
		public BoolProperty canHibernate = new BoolProperty();

		public RectTransform RoomContext;

		private void Start()
		{
			time.value = TimeManager.Instance.Now.ToString() + " " + TimeManager.Instance.Year;
			UpdateAntCountInfo();

			UpdateRooms();
		}

		private void OnEnable()
		{
			GameplayManager.Instance.Player.Resourcer.OnResourceChanged += OnResourceChanged;
			GameplayManager.Instance.Player.Nester.OnNestRebuilt += UpdateRooms;
			GameplayManager.Instance.Player.Anter.OnCompositionChanged += UpdateAntCountInfo;
			OnResourceChanged(ResourceTypes.Meat, 0);
		}

		private void OnDisable()
		{
			if (GameplayManager.HasInstance)
			{
				GameplayManager.Instance.Player.Resourcer.OnResourceChanged -= OnResourceChanged;
				GameplayManager.Instance.Player.Nester.OnNestRebuilt -= UpdateRooms;
				GameplayManager.Instance.Player.Anter.OnCompositionChanged -= UpdateAntCountInfo;
			}
		}

		private void UpdateRooms()
		{
			//clear
			for (int i = 0; i < RoomContext.childCount; i++)
			{
				Destroy(RoomContext.GetChild(i).gameObject);
			}

			foreach (var data in GameplayManager.Instance.Player.Nester.Rooms)
			{
				var room = Instantiate(GameplayHolderManager.Instance.Prefabs.NestRoomPrefab, RoomContext);
				room.Data = data;
			}
		}

		public void OpenForageFrame()
		{
			Close();
			MapManager.Instance.Show();
			WindowManagerUT.Instance.OpenNewPanel<ForagePanel>();
		}

		public void OpenSettings()
		{

		}

		public void CreateUnit(int typeId)
		{
			//todo create plan
			var type = (AntTypes)typeId;
			var plan = new SpawnUnitPlan(GameplayManager.Instance.Player, type);
			GameplayManager.Instance.Player.Planer.TryToAddPlan(plan);
		}

		public void StopCreatingUnit(AbPlayerPlan plan)
		{

		}

		public void EndTurn()
		{
			InputManager.Instance.EndTurn();
			time.value = TimeManager.Instance.Now.ToString() + " " + TimeManager.Instance.Year;
			canHibernate.value = TimeManager.Instance.IsWinter;
		}

		public void Hibernate()
		{
			InputManager.Instance.Hibernate();
			time.value = TimeManager.Instance.Now.ToString() + " " + TimeManager.Instance.Year;
			canHibernate.value = TimeManager.Instance.IsWinter;
		}

		private void UpdateAntCountInfo()
		{
			var elements = new List<BaseListElementData>();
			foreach (var item in GameplayManager.Instance.Player.Anter.AntsInNest)
			{
				elements.Add(new NestAntCountData(item.Key, item.Value));
			}
			ants.value = elements;
		}

		private void OnResourceChanged(ResourceTypes type, int value)
		{
			fruits.value = GameplayManager.Instance.Player.Resourcer.Fruit;
			meat.value = GameplayManager.Instance.Player.Resourcer.Meat;
			storageCapacity.value = GameplayManager.Instance.Player.Resourcer.StorageMaxCapacity;
		}
	}
}