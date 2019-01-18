using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	[CreateAssetMenu(fileName = "BugConfig", menuName = "Config/BugConfig", order = 10)]
	public class BugConfig : ScriptableObject
	{
		public BugTypes Type;
		public int HP;
		public int Damage;
		public int Reward;
		public float CountCoef;
	}
}