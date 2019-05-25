#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;
using System.Collections.Generic;


namespace AIBehaviorEditor
{
	[CustomEditor(typeof(AISkillStates))]
	public class SkillStatesEditor : Editor
	{
		SerializedObject m_Object;
		SerializedProperty animationStatesProp;
		SerializedProperty m_AnimationStatesCount;

		const string kArraySize = "states.Array.size";
		const string kArrayData = "states.Array.data[{0}]";

		Transform transform;
        AISkillStates animStates;
		GameObject statesGameObject;

		AIBehaviorsStyles styles;


		void OnEnable()
		{
			styles = new AIBehaviorsStyles();

			m_Object = new SerializedObject(target);
			animationStatesProp = m_Object.FindProperty("states");
			m_AnimationStatesCount = m_Object.FindProperty(kArraySize);

			animStates = m_Object.targetObject as AISkillStates;
			transform = animStates.transform;

			InitStatesGameObject();
		}


		void InitStatesGameObject()
		{
			SerializedProperty m_Prop = m_Object.FindProperty("skillStatesGameObject");

			statesGameObject = m_Prop.objectReferenceValue as GameObject;

			if ( statesGameObject == null )
			{
				statesGameObject = new GameObject("SkillStates");
				m_Prop.objectReferenceValue = statesGameObject;

				statesGameObject.transform.parent = transform;
				statesGameObject.transform.localPosition = Vector3.zero;

				m_Object.ApplyModifiedProperties();
			}
		}


		public override void OnInspectorGUI()
		{
			m_Object.Update();

			int arraySize = m_AnimationStatesCount.intValue;
            AISkillState[] states = new AISkillState[arraySize+1];

			for ( int i = 0; i < arraySize; i++ )
			{
				string stateNameLabel = "";
				bool oldEnabled = GUI.enabled;

				if ( m_Object.FindProperty(string.Format(kArrayData, i)) == null )
				{
					AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
					continue;
				}

				states[i] = m_Object.FindProperty(string.Format(kArrayData, i)).objectReferenceValue as AISkillState;

				if ( states[i] == null )
				{
					AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
					continue;
				}

				GUILayout.BeginHorizontal();

				if ( string.IsNullOrEmpty(states[i].name) )
					stateNameLabel = "Untitled skill";
				else
					stateNameLabel = states[i].name;

				states[i].foldoutOpen = EditorGUILayout.Foldout(states[i].foldoutOpen, stateNameLabel, EditorStyles.foldoutPreDrop);

				GUI.enabled = i > 0;
				if ( GUILayout.Button(styles.blankContent, styles.upStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
				{
					animationStatesProp.MoveArrayElement(i, i-1);
				}

				GUI.enabled = i < arraySize-1;
				if ( GUILayout.Button(styles.blankContent, styles.downStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
				{
					animationStatesProp.MoveArrayElement(i, i+1);
				}

				GUI.enabled = true;
				if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
				{
					animationStatesProp.InsertArrayElementAtIndex(i);
					animationStatesProp.GetArrayElementAtIndex(i+1).objectReferenceValue = statesGameObject.AddComponent<AISkillState>();
				}

				GUI.enabled = arraySize > 1;
				if ( GUILayout.Button(styles.blankContent, styles.removeStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
				{
					AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
					DestroyImmediate(m_Object.targetObject as AISkillState);
					break;
				}
				GUI.enabled = oldEnabled;

				GUILayout.Space(10);

				GUILayout.EndHorizontal();

				GUILayout.Space(2);

				if ( states[i].foldoutOpen )
				{
					DrawAnimProperties(states[i]);
				}
				else
				{
					SerializedObject serializedAnimState = new SerializedObject(states[i]);
					serializedAnimState.ApplyModifiedProperties();
				}
			}

			if ( arraySize == 0 )
			{
				m_Object.FindProperty(kArraySize).intValue++;
				animationStatesProp.GetArrayElementAtIndex(0).objectReferenceValue = statesGameObject.AddComponent<AISkillState>();
			}

			EditorGUILayout.Separator();

			m_Object.ApplyModifiedProperties();
		}


		public static void DrawAnimProperties(AISkillState animState)
		{
			SerializedObject m_animState = new SerializedObject(animState);
			m_animState.Update();

            SerializedProperty m_animName = m_animState.FindProperty("name");
            SerializedProperty m_Id = m_animState.FindProperty("Id");
            SerializedProperty m_Icon = m_animState.FindProperty("Icon");
            SerializedProperty m_Desc = m_animState.FindProperty("Desc");
            SerializedProperty m_currentCoolDown = m_animState.FindProperty("currentCoolDown");

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_animName);
            EditorGUILayout.PropertyField(m_Id);
            EditorGUILayout.PropertyField(m_Icon);
            EditorGUILayout.PropertyField(m_Desc);
            EditorGUILayout.PropertyField(m_currentCoolDown);


            EditorGUILayout.Separator();

			m_animState.ApplyModifiedProperties();
		}
	}
}
#endif