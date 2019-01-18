using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	[CreateAssetMenu(fileName = "AntConfig", menuName = "Config/AntConfig", order = 10)]
	public class AntConfig : ScriptableObject
	{
		public AntTypes Type;
		public int HP;
		public int Damage;
		public int Gather;
		public int Scout;
		public int Speed;
	}
}