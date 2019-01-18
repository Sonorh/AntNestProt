using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class AntManager
	{
		public event System.Action OnCompositionChanged;

		public PlayerManager Owner { get; set; }
		public Dictionary<AntTypes, AntConfig> AntConfigs = new Dictionary<AntTypes, AntConfig>();
		public Dictionary<AntTypes, int> AntMaxCapacity = new Dictionary<AntTypes, int>();

		public AntManager()
		{
			foreach (var item in Enum.GetValues(typeof(AntTypes)))
			{
				var type = (AntTypes)item;
				AntMaxCapacity.Add(type, int.MaxValue);
			}
		}

		public int Count
		{
			get
			{
				var result = 0;
				foreach (var type in AntsInNest)
				{
					if (type.Key != AntTypes.Larva)
					{
						result += type.Value;
					}
				}
				foreach (var swarm in AntsOnFields)
				{
					result += swarm.AntCount;
				}
				return result;
			}
		}

		public int LarvaCount
		{
			get
			{
				if (AntsInNest.ContainsKey(AntTypes.Larva))
				{
					return AntsInNest[AntTypes.Larva];
				}
				else
				{
					return 0;
				}
			}
		}

		public Dictionary<AntTypes, int> AntsInNest = new Dictionary<AntTypes, int>();
		public List<AntSwarm> AntsOnFields = new List<AntSwarm>();

		//adds ants from nowhere to the nest
		public void AntIncome(AntTypes type, int quantity)
		{
			if (AntsInNest.ContainsKey(type))
			{
				AntsInNest[type] += quantity;
			}
			else
			{
				AntsInNest.Add(type, quantity);
			}
			FixOvergrow();
			OnCompositionChanged?.Invoke();
		}

		//adds swarm from nowhere to the nest
		public void AntIncome(AntSwarm swarm)
		{
			foreach (var type in swarm.Ants)
			{
				if (AntsInNest.ContainsKey(type.Key))
				{
					AntsInNest[type.Key] += type.Value;
				}
				else
				{
					AntsInNest.Add(type.Key, type.Value);
				}
			}
			FixOvergrow();
			OnCompositionChanged?.Invoke();
		}

		//removes ants from the nest to nowhere
		public void AntDecline(AntTypes type, int quantity)
		{
			if (AntsInNest.ContainsKey(type))
			{
				AntsInNest[type] -= quantity;

				if (AntsInNest[type] < 0)
				{
					AntsInNest[type] = 0;
				}

				OnCompositionChanged?.Invoke();
			}
		}

		//removes ants from the nest to nowhere
		public void AntDecline(AntSwarm swarm)
		{
			foreach (var type in swarm.Ants)
			{
				if (AntsInNest.ContainsKey(type.Key))
				{
					AntsInNest[type.Key] = Mathf.Max(AntsInNest[type.Key] - type.Value, 0);
				}
			}

			OnCompositionChanged?.Invoke();
		}

		//sends ants from nest to the field
		public void ToTheField(AntTypes type, int quantity)
		{
			if (HasFreeAnts(type, quantity))
			{
				AntsInNest[type] -= quantity;

				var swarm = new AntSwarm(Owner, type, quantity);
				AntsOnFields.Add(swarm);

				OnCompositionChanged?.Invoke();
			}
		}

		//sends ants from nest to the field
		public void ToTheField(AntSwarm swarm)
		{
			if (HasFreeAnts(swarm))
			{
				AntDecline(swarm);

				AntsOnFields.Add(swarm);

				OnCompositionChanged?.Invoke();
			}
		}

		//sends ants to the nest from the fields
		public void ComeHome(AntSwarm swarm)
		{
			if (AntsOnFields.Contains(swarm))
			{
				AntsOnFields.Remove(swarm);

				AntIncome(swarm);
			}
		}

		public int GetFreeAnts(AntTypes type)
		{
			if (AntsInNest.ContainsKey(type))
				return AntsInNest[type];
			else
				return 0;
		}

		public bool HasFreeAnts(AntTypes type, int amount)
		{
			return GetFreeAnts(type) >= amount;
		}

		public bool HasFreeAnts(AntSwarm swarm)
		{
			foreach (var type in swarm.Ants)
			{
				if (GetFreeAnts(type.Key) < type.Value)
				{
					return false;
				}
			}
			return true;
		}



		public int CalculateGatherPower(AntSwarm swarm)
		{
			var result = 0;

			foreach (var item in swarm.Ants)
			{
				if (swarm.Ants.ContainsKey(item.Key))
				{
					result += AntConfigs[item.Key].Gather * item.Value;
				}
			}

			return result;
		}

		public int CalculateFightPower(AntSwarm swarm)
		{
			var result = 0;

			foreach (var item in swarm.Ants)
			{
				if (swarm.Ants.ContainsKey(item.Key))
				{
					result += AntConfigs[item.Key].Damage * item.Value;
				}
			}

			return result;
		}

		public int CalculateScoutPower(AntSwarm swarm)
		{
			var result = 0;

			foreach (var item in swarm.Ants)
			{
				if (swarm.Ants.ContainsKey(item.Key))
				{
					result += AntConfigs[item.Key].Gather * item.Value;
				}
			}

			return result;
		}

		public bool HasPlaceForAnts(AntTypes type, int quantity)
		{
			return AntMaxCapacity[type] - GetFreeAnts(type) >= quantity;
		}

		public bool HasPlaceForAnts(AntSwarm swarm)
		{
			foreach (var type in swarm.Ants)
			{
				if (!HasPlaceForAnts(type.Key, type.Value))
				{
					return false;
				}
			}
			return true;
		}

		private void FixOvergrow()
		{
			foreach (var item in AntMaxCapacity)
			{
				if (AntsInNest.ContainsKey(item.Key))
				{
					AntsInNest[item.Key] = Mathf.Min(AntsInNest[item.Key], AntMaxCapacity[item.Key]);
				}
			} 
		}

		public void Update()
		{
			FixOvergrow();
			OnCompositionChanged?.Invoke();
		}
	}
}