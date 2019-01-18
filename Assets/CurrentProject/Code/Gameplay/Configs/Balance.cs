using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	//[CreateAssetMenu(fileName = "Balance", menuName = "Config/Balance", order = 1)]
	public class Balance : ScriptableObject
	{
		public int ResourceChance;
		[Space]
		public int ResourceChance2;
	}
}