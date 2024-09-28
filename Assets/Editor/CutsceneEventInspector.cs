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
    private System.Type action;
    private bool removeaction;
    private int indexofAction;
    private bool doAction;
    public void OnEnable()
    {
        dialogue = target as Dialogue;
        cutsceneEvents = serializedObject.FindProperty("entries");
    }
    public override void OnInspectorGUI()
    {
        // Add new event
        if (doAction)
        {
            AddNewEvent(indexofAction, action);
            doAction = false;
        }

        if (removeaction)
        {
            dialogue.entries.RemoveAt(indexofAction);
            cutsceneEvents.DeleteArrayElementAtIndex(indexofAction);
            removeaction = false;
        }
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Cutscene Events", EditorStyles.boldLabel);
        // Iterate over each event in the list
        for (int i = 0; i < cutsceneEvents.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Up", GUILayout.Width(100)))
            {
                ShowAddEventMenu(i);
            }
            if (GUILayout.Button("Add Down", GUILayout.Width(100)))
            {
                ShowAddEventMenu(i + 1);
            }
            if (GUILayout.Button("X", GUILayout.Width(32)))
            {
                Debug.Log($"LoopIndex: {i} | Entries: {dialogue.entries.Count}");
                removeaction = true;
                indexofAction = i;
                //cutsceneEvents.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
            
            SerializedProperty eventProperty = cutsceneEvents.GetArrayElementAtIndex(i);
            CutsceneEvent cutsceneEvent = dialogue.entries[i];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Display the type of the event and handle specific types
            EditorGUILayout.PropertyField(eventProperty, true);
            /*
            if (cutsceneEvent is DialogueEntry dialogueEntry)
            {
                EditorGUILayout.PropertyField(eventProperty, true);
            }
            else if (cutsceneEvent is PlayAnimationEvent playAnimation)
            {
                EditorGUILayout.PropertyField(eventProperty, true);
            }
            */
            // Remove button
            EditorGUILayout.EndVertical();
            eventProperty.serializedObject.ApplyModifiedProperties();
        }
        serializedObject.ApplyModifiedProperties();
    }

    void ShowAddEventMenu(int index)
    {/*
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Dialogue Entry"), false, () => AddNewEvent(index, typeof(DialogueEntry)));
        menu.AddItem(new GUIContent("Play Animation"), false, () => AddNewEvent(index, typeof(PlayAnimationEvent)));
        menu.ShowAsContext();*/

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Dialogue Entry"), false, () => SetAction(index, typeof(DialogueEntry)));
        menu.AddItem(new GUIContent("Play Animation"), false, () => SetAction(index, typeof(PlayAnimationEvent)));
        menu.ShowAsContext();
    }

    private void SetAction(int index, System.Type eventType)
    {
        this.action = eventType;
        this.indexofAction = index;
        doAction = true;
    }
    void AddNewEvent(int index, System.Type eventType)
    {
        Debug.Log(index);
        CutsceneEvent ev;
        if (eventType == typeof(DialogueEntry)) ev = new DialogueEntry();
        else if (eventType == typeof(PlayAnimationEvent)) ev = new PlayAnimationEvent();
        else ev = new DialogueEntry();

        SerializedProperty newEventProperty;
        
        if (index >= dialogue.entries.Count)
        {
            dialogue.entries.Insert(dialogue.entries.Count - 1, ev);
            cutsceneEvents.InsertArrayElementAtIndex(dialogue.entries.Count - 1);
            newEventProperty = cutsceneEvents.GetArrayElementAtIndex(dialogue.entries.Count - 1);
        }
        else if (index <= 0)
        {
            dialogue.entries.Insert(0, ev);
            cutsceneEvents.InsertArrayElementAtIndex(0);
            newEventProperty = cutsceneEvents.GetArrayElementAtIndex(0);
        }
        else
        {
            dialogue.entries.Insert(index, ev);
            cutsceneEvents.InsertArrayElementAtIndex(index);
            newEventProperty = cutsceneEvents.GetArrayElementAtIndex(index);
        }

        newEventProperty.managedReferenceValue = System.Activator.CreateInstance(eventType);
        serializedObject.ApplyModifiedProperties();
    }
}
