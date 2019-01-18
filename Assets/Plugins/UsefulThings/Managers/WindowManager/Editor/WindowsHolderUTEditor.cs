using UnityEditor;
using UnityEngine;

namespace PM.UsefulThings.Editor
{
	[CustomEditor(typeof(WindowsHolderUT))]
	public class WindowsHolderUTEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var trgt = (WindowsHolderUT)target;

			if (GUILayout.Button("Find Windows"))
			{
				trgt.FindWindows();
			}
		}

	}
}