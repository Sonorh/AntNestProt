using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class Caterpillar : AbBug
	{
		public Caterpillar(int count) : base(count)
		{
			this.Type = BugTypes.Caterpillar;
		}

		public override bool Fight(AntSwarm swarm)
		{
			swarm.CalculateHP();

			while(swarm.AntCount > 0 && this.isAlive)
			{
				this.GetHit(swarm.Owner.Anter.CalculateFightPower(swarm));

				if (this.isAlive)
				{
					swarm.GetHit(this.Damage * currentCount);
				}
			}
			return swarm.AntCount > 0;
		}
	}
}