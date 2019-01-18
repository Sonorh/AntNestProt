using UnityEditor;

using UIBinding.Base;

[CustomEditor(typeof(BaseBinding))]
public class BaseBindingEditor : Editor
{
	protected SerializedProperty m_path;

	protected virtual void OnEnable()
	{
		m_path = serializedObject.FindProperty("m_path");
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(m_path);
	}
}
