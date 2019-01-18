using UnityEngine;
using UnityEditor;

using UIBinding.Components;

[CustomEditor(typeof(GraphicFixedColorIntBinding))]
public class GraphicFixedColorIntBindingEditor : BaseBindingEditor
{
	private GUIStyle buttonMiddleStyle { get { return m_buttonMiddleStyle ?? (m_buttonMiddleStyle = new GUIStyle("Button") { fixedWidth = 20f, fixedHeight = 14f, alignment = TextAnchor.MiddleCenter }); } }
	private GUIStyle buttonLowerStyle { get { return m_buttonLowerStyle ?? (m_buttonLowerStyle = new GUIStyle("Button") { fixedWidth = 20f, fixedHeight = 14f, alignment = TextAnchor.LowerCenter }); } }
	private GUIStyle m_buttonMiddleStyle;
	private GUIStyle m_buttonLowerStyle;

	private SerializedProperty m_elements;

	private static bool m_expanded = true;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_elements = serializedObject.FindProperty("m_elements");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.BeginHorizontal();
		m_expanded = EditorGUILayout.Foldout(m_expanded, string.Format("Elements ({0})", m_elements.arraySize));
		if (m_expanded)
		{
			if (GUILayout.Button("+", buttonMiddleStyle))
			{
				m_elements.InsertArrayElementAtIndex(Mathf.Max(m_elements.arraySize - 1, 0));
			}
			if (GUILayout.Button("-", buttonMiddleStyle))
			{
				if (m_elements.arraySize > 0)
				{
					m_elements.DeleteArrayElementAtIndex(m_elements.arraySize - 1);
				}
			}
		}
		GUILayout.EndHorizontal();

		if (m_expanded)
		{
			EditorGUI.indentLevel++;
			var index = 0;
			while (index < m_elements.arraySize)
			{
				var item = m_elements.GetArrayElementAtIndex(index);

				GUILayout.BeginHorizontal();
				var value = item.FindPropertyRelative("m_value");
				var color = item.FindPropertyRelative("m_color");
				EditorGUILayout.LabelField("Value", GUILayout.Width(50f));
				value.intValue = EditorGUILayout.IntField(value.intValue, GUILayout.Width(100f));
				EditorGUILayout.LabelField("Color", GUILayout.Width(50f));
				color.colorValue = EditorGUILayout.ColorField(color.colorValue);
				if (GUILayout.Button("↑", buttonLowerStyle))
				{
					if (index > 0)
					{
						m_elements.MoveArrayElement(index, index - 1);
					}
				}
				if (GUILayout.Button("↓", buttonLowerStyle))
				{
					if (index < m_elements.arraySize - 1)
					{
						m_elements.MoveArrayElement(index, index + 1);
					}
				}
				if (GUILayout.Button("x", buttonMiddleStyle))
				{
					m_elements.DeleteArrayElementAtIndex(index);
					continue;
				}
				GUILayout.EndHorizontal();

				index++;
			}
			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
		}
	}
}