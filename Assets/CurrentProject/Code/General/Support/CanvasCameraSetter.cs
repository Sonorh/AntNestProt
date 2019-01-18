using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.Antnest.General
{
	public class CanvasCameraSetter : MonoBehaviour
	{
		private Canvas canvas;

		private void Start()
		{
			canvas = this.GetComponent<Canvas>();
		}

		private void Update()
		{
			if (canvas.worldCamera == null)
			{
				canvas.worldCamera = Camera.main;
			}
		}
	}
}