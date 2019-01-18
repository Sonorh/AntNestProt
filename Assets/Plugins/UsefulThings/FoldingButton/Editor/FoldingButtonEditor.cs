using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PM.UsefulThings.Editor
{
	[UnityEditor.CustomEditor(typeof(FoldingButton))]
	public class FoldingButtonEditor : UnityEditor.UI.ButtonEditor
	{

		public override void OnInspectorGUI()
		{
			FoldingButton t = (FoldingButton)target;

			t.Content = (GameObject)EditorGUILayout.ObjectField("Content", t.Content, typeof(GameObject), true);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			base.OnInspectorGUI();
		}
	}
}