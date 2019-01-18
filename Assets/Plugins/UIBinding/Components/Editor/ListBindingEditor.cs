using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using UIBinding.Elements;
using UIBinding.Components;

[CustomEditor(typeof(ListBinding))]
public class ListBindingEditor : BaseBindingEditor
{
	private SerializedProperty m_elementPrefab;
	private SerializedProperty m_autoScrollPivot;
	private SerializedProperty m_dragTolerance;
	private SerializedProperty m_scrollSpeedByDrag;
	private SerializedProperty m_elementAnimationDelay;
	private SerializedProperty m_showOnRebuild;
	private ScrollRect m_scrollRect;
	private Transform m_root;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_elementPrefab = serializedObject.FindProperty("m_elementPrefab");
		m_autoScrollPivot = serializedObject.FindProperty("m_autoScrollPivot");
		m_dragTolerance = serializedObject.FindProperty("m_dragTolerance");
		m_scrollSpeedByDrag = serializedObject.FindProperty("m_scrollSpeedByDrag");
		m_elementAnimationDelay = serializedObject.FindProperty("m_elementAnimationDelay");
		m_showOnRebuild = serializedObject.FindProperty("m_showOnRebuild");

		var monoBehaviour = target as MonoBehaviour;
		m_root = monoBehaviour.transform;

		TryToFindScrollRect(m_root);
	}

	public override void OnInspectorGUI()
	{
		if ((m_scrollRect != null) && (m_scrollRect.content != null))
		{
			EditorGUILayout.HelpBox(string.Format("Обнаружен ScrollRect. Пожалуйста, переместите данный компонент на объект {0}.", m_scrollRect.name), MessageType.Warning);
		}

		base.OnInspectorGUI();

		EditorGUILayout.PropertyField(m_elementPrefab);
		EditorGUILayout.PropertyField(m_autoScrollPivot);

		if (m_elementPrefab.objectReferenceValue is BaseDraggableListElement)
		{
			EditorGUILayout.PropertyField(m_dragTolerance);
			EditorGUILayout.PropertyField(m_scrollSpeedByDrag);
		}
		if (m_elementPrefab.objectReferenceValue is BaseAnimatedListElement)
		{
			EditorGUILayout.PropertyField(m_elementAnimationDelay);
			EditorGUILayout.PropertyField(m_showOnRebuild);
		}

		serializedObject.ApplyModifiedProperties();
	}

	private void TryToFindScrollRect(Transform transform)
	{
		var scrollRect = transform.GetComponent<ScrollRect>();
		if ((scrollRect != null) && (scrollRect.content == m_root))
		{
			m_scrollRect = scrollRect;
		}
		else if (transform.parent != null)
		{
			TryToFindScrollRect(transform.parent);
		}
	}
}