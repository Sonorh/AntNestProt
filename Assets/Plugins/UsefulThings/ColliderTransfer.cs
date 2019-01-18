using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public class ColliderTransfer : MonoBehaviour
	{
		public event System.Action OnMouseClick;

		private void OnMouseUpAsButton()
		{
			OnMouseClick?.Invoke();
		}
	}
}