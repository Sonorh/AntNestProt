using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.UsefulThings.Extensions;

namespace PM.Antnest.Gameplay
{
	[Serializable]
	public class AntSwarm
	{
		public static event System.Action<AntSwarm> OnCompositionChanged;

		public PlayerManager Owner { get; set; }
		public int AntCount { get; set; }
		public AbCellData Position { get; set; }
		public int Speed { get; private set; }

		public int HP { get; private set; }

		private Dictionary<AntTypes, int> _ants;
		public Dictionary<AntTypes, int> Ants
		{
			get
			{
				if (_ants == null)
				{
					_ants = new Dictionary<AntTypes, int>();
				}

				return _ants;
			}
			private set
			{
				_ants = value;
			}
		}

		public AntSwarm(PlayerManager owner, AntTypes type, int quantity) : this(owner)
		{
			_ants.Add(type, quantity);
			AntCount = quantity;
			RecalcSpeed();
		}

		public AntSwarm(PlayerManager owner, AntTypes type) : this(owner, type, 0) { }

		public AntSwarm(PlayerManager owner = null)
		{
			_ants = new Dictionary<AntTypes, int>();

			this.Owner = owner;
		}

		public void Add(AntTypes type, int quantity)
		{
			if (Ants.ContainsKey(type))
			{
				Ants[type] += quantity;
			}
			else
			{
				Ants.Add(type, quantity);
			}
			AntCount += quantity;
			RecalcSpeed();
			OnCompositionChanged?.Invoke(this);
		}

		public void Remove(AntTypes type, int quantity)
		{
			if (Ants.ContainsKey(type))
			{
				Ants[type] = Mathf.Max(Ants[type] - quantity, 0);

				AntCount = 0;
				foreach (var amount in Ants.Values)
				{
					AntCount += amount;
				}

				RecalcSpeed();
				OnCompositionChanged?.Invoke(this);
			}
		}

		private void RecalcSpeed()
		{
			Speed = 1;
		}

		public void Camp()
		{
			var env = Position as EnviromentCellData;
			if (env != null)
			{
				if (env.IsOccupied && env.Occupant != this)
				{
					Debug.LogError("Trying to camp in occupied cell");
				}
				env.Occupant = this;
			}
		}

		public void Pack()
		{
			var env = Position as EnviromentCellData;
			if (env != null)
			{
				if (env.IsOccupied && env.Occupant == this)
				{
					env.Occupant = null;
				}
			}
		}

		public void CalculateHP()
		{
			HP = 0;
			foreach (var item in Ants)
			{
				HP += Owner.Anter.AntConfigs[item.Key].HP * item.Value;
			}
		}

		public void GetHit(int damage)
		{
			HP = Mathf.Max(0, HP - damage);

			if (HP == 0)
			{
				Ants.Clear();
				AntCount = 0;
			}
			else
			{
				var configs = new List<AntConfig>();
				foreach (var item in Ants)
				{
					if (item.Value > 0)
					{
						configs.Add(Owner.Anter.AntConfigs[item.Key]);
					}
				}

				var temp = Ants;
				Ants = new Dictionary<AntTypes, int>();
				AntCount = 0;

				configs.SortByDescending(x => x.HP);

				var currHp = 0;

				var quantity = 0;
				for (int i = 0; i < configs.Count; i++)
				{
					quantity = Mathf.Min((HP - currHp) / configs[i].HP, temp[configs[i].Type]);
					currHp += configs[i].HP * quantity;
					Ants.Add(configs[i].Type, quantity);
					AntCount += quantity;
				}

				for (int i = configs.Count - 1; i >= 0; i--)
				{
					if (Ants[configs[i].Type] < temp[configs[i].Type])
					{
						Ants[configs[i].Type] += 1;
						AntCount += 1;
					}
				}
			}

			RecalcSpeed();
			OnCompositionChanged?.Invoke(this);
		}

		public void Die()
		{
			Ants.Clear();
			AntCount = 0;
			RecalcSpeed();
			OnCompositionChanged?.Invoke(this);
		}
	}
}