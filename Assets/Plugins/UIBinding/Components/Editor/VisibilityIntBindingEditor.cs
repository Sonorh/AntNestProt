using UnityEditor;

using UIBinding.Components;

[CustomEditor(typeof(VisibilityIntBinding)), CanEditMultipleObjects]
public class VisibilityIntBindingEditor : Editor
{
	private SerializedProperty m_enumType;

	private void OnEnable()
	{
		m_enumType = serializedObject.FindProperty("m_checkType");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var checkType = (VisibilityIntBinding.CheckType)m_enumType.enumValueIndex;
		var info = "";
		switch (checkType)
		{
			case VisibilityIntBinding.CheckType.Equal:
				info = "1, 2, 3";
				break;
			case VisibilityIntBinding.CheckType.Greater:
				info = "1";
				break;
			case VisibilityIntBinding.CheckType.Less:
				info = "2";
				break;
			case VisibilityIntBinding.CheckType.InRange:
				info = "1, 3";
				break;
		}
		EditorGUILayout.HelpBox(string.Format("Example value: {0}", info), MessageType.Info);
	}
}