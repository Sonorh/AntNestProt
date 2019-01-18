using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.Gameplay
{
	public class AntAvatar : MonoBehaviour
	{
		public SpriteRenderer Avatar;

		public AntSwarm Swarm;

		private void Start()
		{
			Avatar.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
		}
	}
}