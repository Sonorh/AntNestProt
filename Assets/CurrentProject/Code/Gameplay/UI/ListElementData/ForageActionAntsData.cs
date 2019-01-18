using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.Gameplay
{
	public class ForageActionAntsData : BaseListElementData
	{
		public event System.Action OnTypeChanged;
		public event System.Action OnCountChanged;

		public bool IsTypeChosen { get { return antType.value > 0; } }
		public bool HasUnitsOfType { get { return IsTypeChosen && GameplayManager.Instance.Player.Anter.HasFreeAnts(types[antType.value - 1], 1); } }
		public AntTypes ChosenType { get { return IsTypeChosen ? types[antType.value - 1] : AntTypes.Worker; } }
		public int Count { get { return antCount.value; } }

		private readonly EnumerableProperty antTypes = new EnumerableProperty();
		private readonly IntProperty antType = new IntProperty();
		private readonly BoolProperty canChoseCount = new BoolProperty();
		private readonly IntProperty antCount = new IntProperty();
		private readonly FloatProperty antCountSlider = new FloatProperty();

		private readonly List<AntTypes> types = new List<AntTypes>();

		private AntTypes? defaultType = null;
		private int defaultCount = -1;

		public ForageActionAntsData(List<AntTypes> types, bool setDefault = false, AntTypes? defaultType = null, int defaultCount = -1)
		{
			this.types = types;
			this.defaultType = defaultType;
			this.defaultCount = defaultCount;

			var options = new List<string>();
			options.Add("None".Localized());
			foreach (var item in types)
			{
				options.Add(item.ToString().Localized());
			}
			this.antTypes.value = options;

			if (setDefault && defaultType != null)
			{
				this.antType.value = types.IndexOf(defaultType.Value) + 1;
			}


			if (setDefault && HasUnitsOfType && defaultCount >= 0)
			{
				antCountSlider.value = Mathf.Min((float)defaultCount, GameplayManager.Instance.Player.Anter.GetFreeAnts(types[antType.value - 1])) / GameplayManager.Instance.Player.Anter.GetFreeAnts(types[antType.value - 1]);
			}
			else
			{
				antCountSlider.value = 1;
			}

			canChoseCount.value = HasUnitsOfType;

			antCountSlider.OnValueChanged += OnAntCountChangedHandler;
			antType.OnValueChanged += OnAntTypeChangedHandler;

			OnAntTypeChangedHandler();
		}

		private void OnAntTypeChangedHandler()
		{
			OnTypeChanged?.Invoke();

			if (HasUnitsOfType && defaultType != null && defaultType == types[antType.value - 1] && defaultCount >= 0)
			{
				antCountSlider.value = Mathf.Min((float)defaultCount, GameplayManager.Instance.Player.Anter.GetFreeAnts(types[antType.value - 1])) / GameplayManager.Instance.Player.Anter.GetFreeAnts(types[antType.value - 1]);
			}
			else
			{
				antCountSlider.value = 1;
			}

			OnAntCountChangedHandler();
			canChoseCount.value = HasUnitsOfType;
		}

		private void OnAntCountChangedHandler()
		{
			if (HasUnitsOfType)
			{
				var maxCount = GameplayManager.Instance.Player.Anter.GetFreeAnts(types[antType.value - 1]);
				antCount.value = Mathf.FloorToInt(maxCount * antCountSlider.value);
			}
			else
			{
				antCount.value = 0;
			}
			OnCountChanged?.Invoke();
		}
	}
}