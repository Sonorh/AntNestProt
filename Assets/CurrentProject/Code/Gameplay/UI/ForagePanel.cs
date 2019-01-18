using PM.UsefulThings;
using System.Collections;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.Gameplay
{
	public class ForagePanel : AbPanel
	{
		public ListProperty ants = new ListProperty();
		public StringProperty time = new StringProperty();
		public IntProperty fruits = new IntProperty();
		public IntProperty meat = new IntProperty();
		public IntProperty storageCapacity = new IntProperty();
		public BoolProperty canHibernate = new BoolProperty();

		private void Start()
		{
			time.value = TimeManager.Instance.Now.ToString() + " " + TimeManager.Instance.Year;
			UpdateAntCountInfo();
		}

		private void OnEnable()
		{
			GameplayManager.Instance.Player.Resourcer.OnResourceChanged += OnResourceChanged;
			GameplayManager.Instance.Player.Anter.OnCompositionChanged += UpdateAntCountInfo;
			OnResourceChanged(ResourceTypes.Meat, 0);
		}

		private void OnDisable()
		{
			if (GameplayManager.HasInstance)
			{
				GameplayManager.Instance.Player.Resourcer.OnResourceChanged -= OnResourceChanged;
				GameplayManager.Instance.Player.Anter.OnCompositionChanged -= UpdateAntCountInfo;
			}
		}

		public void MenuClick()
		{
			GameplayManager.Instance.Pause();
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