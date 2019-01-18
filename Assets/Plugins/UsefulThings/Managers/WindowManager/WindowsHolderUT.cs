using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PM.UsefulThings
{
	//[CreateAssetMenu(fileName = "WindowsHolderUT", menuName = "Holders/WindowsHolderUT", order = 100)]
	public class WindowsHolderUT : ScriptableObject
	{
		[SerializeField]
		public MonoBehaviour[] Windows;

		public void FindWindows()
		{
			Windows = Resources.LoadAll<MonoBehaviour>("UIWindows").Where(x => x is IWindowUT).ToArray();
		}
	}
}