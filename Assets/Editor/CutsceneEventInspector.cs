using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dialogue))]
public class CutsceneEventInspector : Editor
{
    Dialogue dialogue;
    SerializedProperty cutsceneEvents;
    public void OnEnable()
    {
        dialogue = target as Dialogue;
        cutsceneEvents = serializedObject.FindProperty("entries");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Cutscene Events", EditorStyles.boldLabel);

        // Iterate over each event in the list
        for (int i = 0; i < cutsceneEvents.arraySize; i++)
        {
            SerializedProperty eventProperty = cutsceneEvents.GetArrayElementAtIndex(i);
            CutsceneEvent cutsceneEvent = dialogue.entries[i];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Display the type of the event and handle specific types
            if (cutsceneEvent is DialogueEntry dialogueEntry)
            {
                dialogueEntry.name = EditorGUILayout.TextField("Speaker Name", dialogueEntry.name);
                dialogueEntry.leftPortrait = EditorGUILayout.TextField("Left Portrait", dialogueEntry.leftPortrait);
                dialogueEntry.rightPortrait = EditorGUILayout.TextField("Right Portrait", dialogueEntry.rightPortrait);
                dialogueEntry.text = EditorGUILayout.TextField("Dialogue Text", dialogueEntry.text);
            }
            else if (cutsceneEvent is PlayAnimationEvent playAnimation)
            {
                playAnimation.animation = EditorGUILayout.TextField("Animation Name", playAnimation.animation);
            }

            // Remove button
            if (GUILayout.Button("Remove Event"))
            {
                cutsceneEvents.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndVertical();
        }

        // Add new event
        if (GUILayout.Button("Add New Event"))
        {
            ShowAddEventMenu();
        }

        serializedObject.ApplyModifiedProperties();
    }

    void ShowAddEventMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Dialogue Entry"), false, () => AddNewEvent(typeof(DialogueEntry)));
        menu.AddItem(new GUIContent("Play Animation"), false, () => AddNewEvent(typeof(PlayAnimationEvent)));
        menu.ShowAsContext();
    }

    void AddNewEvent(System.Type eventType)
    {
        CutsceneEvent ev;
        if (eventType == typeof(DialogueEntry)) ev = new DialogueEntry();
        else if (eventType == typeof(PlayAnimationEvent)) ev = new PlayAnimationEvent();
        else ev = new DialogueEntry();

        dialogue.entries.Add(ev);
        cutsceneEvents.InsertArrayElementAtIndex(cutsceneEvents.arraySize);
        SerializedProperty newEventProperty = cutsceneEvents.GetArrayElementAtIndex(cutsceneEvents.arraySize - 1);
        //fieldInfo.SetValue(currentProperty.serializedObject.targetObject, new List<QuestStep>() { new KillStep() });
        newEventProperty.managedReferenceValue = System.Activator.CreateInstance(dialogue.entries[dialogue.entries.Count - 1].GetType());
        serializedObject.ApplyModifiedProperties();
    }
}
