using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Antnest.Gameplay
{
	public class AntCounterListItem : MonoBehaviour
	{
		public Image Picture;
		[SerializeField]
		private Text nameText;
		[SerializeField]
		private Text count;

		public string Name
		{
			get
			{
				return nameText.text;
			}
			set
			{
				nameText.text = value;
			}
		}
		public int Count
		{
			get
			{
				return int.Parse(count.text);
			}
			set
			{
				count.text = value.ToString();
			}
		}
	}
}