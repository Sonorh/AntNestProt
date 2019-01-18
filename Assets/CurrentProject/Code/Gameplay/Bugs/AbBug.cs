using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public abstract class AbBug
	{
		public static AbBug Create(BugTypes type, int count)
		{
			var bug = new Caterpillar(count);
			bug.Init();

			return bug;
		}

		public bool isAlive { get { return HP > 0; } }

		public BugTypes Type;
		public int HP;
		public int Damage;
		public int Reward;
		public int StartCount { get; protected set; } = 1;

		protected int maxHP;
		protected int currentCount { get { return isAlive ? HP / maxHP + 1 : 0; } }

		protected BugConfig config;

		public AbBug(int count)
		{
			this.StartCount = count;
		}

		public abstract bool Fight(AntSwarm swarm);

		protected void GetHit(int damage)
		{
			HP = Mathf.Max(0, HP - damage);
		}

		protected virtual void Init()
		{
			config = GameplayHolderManager.Instance.Configs.BugConfigs.Find((x) => x.Type == this.Type);

			if (config == null)
			{
				Debug.LogError("THERE IS NO CONFIG FOR BUG: " + Type.ToString());
				return;
			}

			this.maxHP = config.HP;
			this.Damage = config.Damage;
			this.Reward = config.Reward;
			this.HP = config.HP * StartCount;
		}
	}
}