using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class ResourceManager
	{
		public event System.Action<ResourceTypes, int> OnResourceChanged;

		public int StorageMaxCapacity { get; set; } = int.MaxValue;
		public int Meat { get; private set; }
		public int Fruit { get; private set; }

		public bool IsEnough(ResourceTypes type, int quantity)
		{
			switch (type)
			{
				case ResourceTypes.Meat:
					return Meat >= quantity;
				case ResourceTypes.Fruit:
					return Fruit >= quantity;
				default:
					throw new System.Exception("There is no such resource in manager: " + type);
			}
		}

		public bool Spend(ResourceTypes type, int quantity)
		{
			if (IsEnough(type, quantity))
			{
				switch (type)
				{
					case ResourceTypes.Meat:
						Meat -= quantity;
						OnResourceChanged?.Invoke(ResourceTypes.Meat, Meat);
						return true;
					case ResourceTypes.Fruit:
						Fruit -= quantity;
						OnResourceChanged?.Invoke(ResourceTypes.Fruit, Fruit);
						return true;
					default:
						throw new System.Exception("There is no such resource in manager: " + type);
				}
			}
			Debug.Log("Not enough resources: " + type);
			return false;
		}

		public void Store(ResourceTypes type, int quantity)
		{
			switch (type)
			{
				case ResourceTypes.Meat:
					Meat = Mathf.Min(Meat + quantity, StorageMaxCapacity);
					OnResourceChanged?.Invoke(ResourceTypes.Meat, Meat);
					break;
				case ResourceTypes.Fruit:
					Fruit = Mathf.Min(Fruit + quantity, StorageMaxCapacity);
					OnResourceChanged?.Invoke(ResourceTypes.Fruit, Fruit);
					break;
				default:
					throw new System.Exception("There is no such resource in manager: " + type);
			}
		}

		public void Update()
		{
			var temp = Meat;
			Meat = Mathf.Min(Meat, StorageMaxCapacity);
			if (temp != Meat)
			{
				OnResourceChanged?.Invoke(ResourceTypes.Meat, Meat);
			}
			temp = Fruit;
			Fruit = Mathf.Min(Fruit, StorageMaxCapacity);
			if (temp != Fruit)
			{
				OnResourceChanged?.Invoke(ResourceTypes.Fruit, Fruit);
			}
		}
	}
}