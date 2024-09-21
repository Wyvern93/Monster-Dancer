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
        EditorGUI.BeginChangeCheck();
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
                EditorGUILayout.PropertyField(eventProperty, true);
                /*
                dialogueEntry.name = EditorGUILayout.TextField("Name", dialogueEntry.name);
                dialogueEntry.leftSide = EditorGUILayout.Toggle("LeftSide", dialogueEntry.leftSide);
                
                dialogueEntry.text = EditorGUILayout.TextField("Dialogue Text", dialogueEntry.text);
                dialogueEntry.leftPortrait = EditorGUILayout.TextField("Left Portrait", dialogueEntry.leftPortrait);
                dialogueEntry.rightPortrait = EditorGUILayout.TextField("Right Portrait", dialogueEntry.rightPortrait);*/
            }
            else if (cutsceneEvent is PlayAnimationEvent playAnimation)
            {
                //playAnimation.animation = EditorGUILayout.TextField("Animation Name", playAnimation.animation);
                EditorGUILayout.PropertyField(eventProperty, true);
            }
            if (GUILayout.Button("Add New Event"))
            {
                ShowAddEventMenuHere(i);
            }
            // Remove button
            if (GUILayout.Button("Remove Event"))
            {
                cutsceneEvents.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndVertical();
            eventProperty.serializedObject.ApplyModifiedProperties();
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

    void ShowAddEventMenuHere(int index)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Dialogue Entry"), false, () => AddNewEventHere(index, typeof(DialogueEntry)));
        menu.AddItem(new GUIContent("Play Animation"), false, () => AddNewEventHere(index, typeof(PlayAnimationEvent)));
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
    void AddNewEventHere(int index, System.Type eventType)
    {
        CutsceneEvent ev;
        if (eventType == typeof(DialogueEntry)) ev = new DialogueEntry();
        else if (eventType == typeof(PlayAnimationEvent)) ev = new PlayAnimationEvent();
        else ev = new DialogueEntry();

        dialogue.entries.Add(ev);
        cutsceneEvents.InsertArrayElementAtIndex(index);
        SerializedProperty newEventProperty = cutsceneEvents.GetArrayElementAtIndex(index - 1);
        //fieldInfo.SetValue(currentProperty.serializedObject.targetObject, new List<QuestStep>() { new KillStep() });
        newEventProperty.managedReferenceValue = System.Activator.CreateInstance(dialogue.entries[index - 1].GetType());
        serializedObject.ApplyModifiedProperties();
    }
}
