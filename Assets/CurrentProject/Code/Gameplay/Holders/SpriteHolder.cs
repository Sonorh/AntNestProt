using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	//[CreateAssetMenu(fileName = "SpriteHolder", menuName = "SpriteHolder", order = 1)]
	public class SpriteHolder : ScriptableObject
	{
		public Sprite[] WallSprites;
		[Space]
		public Sprite[] FieldCellSprites;
	}
}