using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class ForageActionPanel : AbPanel
	{
		public System.Action<AntSwarm> OnSend;

		private readonly StringProperty title = new StringProperty();
		private readonly ListProperty antList = new ListProperty();
		private readonly BoolProperty isSendCheckPassed = new BoolProperty();

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
			title.value = "Send Ants".Localized();

			UpdateAntList(true);
		}

		private void UpdateAntList(bool isDefault = false)
		{
			var elements = new List<BaseListElementData>();
			elements.AddRange(antList.value);

			for (int i = 0; i < elements.Count; i++)
			{
				if (!(elements[i] as ForageActionAntsData).IsTypeChosen)
				{
					(elements[i] as ForageActionAntsData).OnTypeChanged -= AntElementOnTypeChangedHandler;
					elements.RemoveAt(i);
					i--;
				}
			}

			while (elements.Count == 0 || (elements.Last() as ForageActionAntsData).IsTypeChosen)
			{
				var types = new List<AntTypes>(availableTypes);

				foreach (var data in antList.value)
				{
					if ((data as ForageActionAntsData).IsTypeChosen)
					{
						types.Remove((data as ForageActionAntsData).ChosenType);
					}
				}

				var elem = new ForageActionAntsData(types, isDefault && elements.Count == 0, defaultType, defaultCount);
				elem.OnTypeChanged += AntElementOnTypeChangedHandler;
				elem.OnCountChanged += AntElementOnCountChangedHandler;
				elements.Add(elem);
			}

			antList.value = elements;

			AntElementOnCountChangedHandler();
		}

		private void AntElementOnCountChangedHandler()
		{
			var swarm = new AntSwarm();
			ForageActionAntsData data;
			foreach (var elem in antList.value)
			{
				data = elem as ForageActionAntsData;
				if (data.IsTypeChosen && data.Count > 0)
				{
					swarm.Add(data.ChosenType, data.Count);
				}
			}
			isSendCheckPassed.value = (sendCheck != null ? sendCheck(swarm) : true) && swarm.AntCount > 0;
		}

		private void AntElementOnTypeChangedHandler()
		{
			UpdateAntList();
		}

		public void SetParameters(List<AntTypes> types, AntTypes? defaultType = null, int defaultCount = -1, Func<AntSwarm, bool> sendCheck = null)
		{
			this.availableTypes = types;
			this.defaultType = defaultType;
			this.defaultCount = defaultCount;
			this.sendCheck = sendCheck;
		}

		public void Send()
		{
			var swarm = new AntSwarm(GameplayManager.Instance.Player);
			ForageActionAntsData data;
			foreach (var elem in antList.value)
			{
				data = elem as ForageActionAntsData;
				if (data.IsTypeChosen && data.Count > 0)
				{
					swarm.Add(data.ChosenType, data.Count);
				}
			}

			if (swarm.AntCount > 0)
			{
				OnSend?.Invoke(swarm);
			}
			Close();
		}

		public void Cancel()
		{
			Close();
		}
	}
}