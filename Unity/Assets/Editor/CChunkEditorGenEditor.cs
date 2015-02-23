using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(CChunkEditorGen))]
public class CChunkEditorGenEditor : Editor
{
	// Undocumented ReoderableList to make things nice, could change without warning in future update.
	private ReorderableList m_lstLayers;

	public void OnEnable()
	{
		m_lstLayers = new ReorderableList(serializedObject, serializedObject.FindProperty("lstLayers"),
			true, true, true, true);
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		m_lstLayers.DoLayoutList();
		serializedObject.ApplyModifiedProperties();

		m_lstLayers.drawElementCallback = vDrawElements;
		m_lstLayers.onAddCallback = vAddElement;
		m_lstLayers.onRemoveCallback = vRemoveElement;
		m_lstLayers.onReorderCallback = vMoveElement;
	}

	private void vDrawElements(Rect p_rect, int p_index, bool p_fIsActive, bool p_fIsFocused)
	{
		SerializedProperty t_element = m_lstLayers.serializedProperty.GetArrayElementAtIndex(p_index);
		
		p_rect.y += 2;
		
		if (t_element.FindPropertyRelative("m_fEditable").boolValue)
		{
			EditorGUI.PropertyField(
				new Rect(p_rect.x, p_rect.y, p_rect.width - 60, EditorGUIUtility.singleLineHeight),
				t_element.FindPropertyRelative("m_strName"), GUIContent.none);
		}
		else
		{
			EditorGUI.LabelField(
				new Rect(p_rect.x, p_rect.y, p_rect.width - 60, EditorGUIUtility.singleLineHeight),
				t_element.FindPropertyRelative("m_strName").stringValue, GUIStyle.none);
		}
	}
	private void vAddElement(ReorderableList p_list)
	{
		int t_index = p_list.serializedProperty.arraySize;
		p_list.serializedProperty.arraySize++;
		p_list.index = t_index;
		
		SerializedProperty t_element = p_list.serializedProperty.GetArrayElementAtIndex(t_index);
		t_element.FindPropertyRelative("m_strName").stringValue = "Layer " + t_index;
		t_element.FindPropertyRelative("m_nLayer").intValue = t_index;
		t_element.FindPropertyRelative("m_fEditable").boolValue = true;
		t_element.FindPropertyRelative("m_fMoveable").boolValue = true;
		t_element.FindPropertyRelative("m_fTilesAllowed").boolValue = true;
	}
	private void vRemoveElement(ReorderableList p_list)
	{
		SerializedProperty t_element = m_lstLayers.serializedProperty.GetArrayElementAtIndex(p_list.index);

		if (t_element.FindPropertyRelative("m_fEditable").boolValue)
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(p_list);
		}
	}
	private void vMoveElement(ReorderableList p_list)
	{
		int t_nSize = p_list.serializedProperty.arraySize;

		for (int t_i = 0; t_i < t_nSize; ++t_i)
		{
			SerializedProperty t_element = p_list.serializedProperty.GetArrayElementAtIndex(t_i);

			if (t_element.FindPropertyRelative("m_nLayer").intValue != t_i && t_element.FindPropertyRelative("m_fMoveable").boolValue == false)
			{
				// This element was moved but it wasn't supposed to be
				p_list.serializedProperty.MoveArrayElement(t_i, t_element.FindPropertyRelative("m_nLayer").intValue);
			}
		}
	}
}
