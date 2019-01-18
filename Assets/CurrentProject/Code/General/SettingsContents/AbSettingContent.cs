using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.General
{
	public abstract class AbSettingContent : MonoBehaviour
	{
		public Text Title;

		private void OnEnable()
		{
			UpdateElements();
		}

		public abstract void SetDefault();
		public abstract void UpdateElements();
	}
}