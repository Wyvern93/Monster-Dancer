using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEventInspector : Editor
{
    Cutscene cutscene;
    SerializedProperty cutsceneEvents;
    private System.Type action;
    private bool removeaction;
    private int indexofAction;
    private bool doAction;
    public void OnEnable()
    {
        cutscene = target as Cutscene;
        cutsceneEvents = serializedObject.FindProperty("entries");
    }

    private string GetEventName(CutsceneEvent cutsceneEvent)
    {
        if (cutsceneEvent is DialogueEntry)
        {
            DialogueEntry entry = (DialogueEntry)cutsceneEvent;
            return $"Dialogue ({entry.name})";
        }
        else if (cutsceneEvent is ActorTeleportEvent)
        {
            ActorTeleportEvent entry = (ActorTeleportEvent)cutsceneEvent;
            if (entry.actor == null) return "Teleport Actor";
            return $"Teleport ({entry.actor.name})";
        }
        else if (cutsceneEvent is ActorMoveEvent)
        {
            ActorMoveEvent entry = (ActorMoveEvent)cutsceneEvent;
            if (entry.actor == null) return "Move Actor";
            return $"Move ({entry.actor.name})";
        }
        else if (cutsceneEvent is ActorFacingEvent)
        {
            ActorFacingEvent entry = (ActorFacingEvent)cutsceneEvent;
            if (entry.actor == null) return "Facing Actor";
            string right = entry.facingRight ? "right" : "left";
            return $"Actor ({entry.actor.name}) facing {right}";
        }
        else if (cutsceneEvent is WaitForActorToEndEvent)
        {
            WaitForActorToEndEvent entry = (WaitForActorToEndEvent)cutsceneEvent;
            if (entry.actor == null) return "Wait for Actor";
            return $"Wait for ({entry.actor.name})";
        }
        else if (cutsceneEvent is PlayAnimationEvent)
        {
            PlayAnimationEvent entry = (PlayAnimationEvent)cutsceneEvent;
            return $"Animation: {entry.animation}";
        }
        return "Event";
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
            cutscene.entries.RemoveAt(indexofAction);
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
                Debug.Log($"LoopIndex: {i} | Entries: {cutscene.entries.Count}");
                removeaction = true;
                indexofAction = i;
                //cutsceneEvents.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
            
            SerializedProperty eventProperty = cutsceneEvents.GetArrayElementAtIndex(i);
            CutsceneEvent cutsceneEvent = cutscene.entries[i];

            // Create a custom label for the event
            string eventName = GetEventName(cutsceneEvent); // Use the event's class name as the label
            GUIContent customLabel = new GUIContent(eventName);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Display the type of the event and handle specific types
            EditorGUILayout.PropertyField(eventProperty, customLabel, true);

            // Remove button
            EditorGUILayout.EndVertical();
            eventProperty.serializedObject.ApplyModifiedProperties();
        }

        if (cutsceneEvents.arraySize == 0)
        {
            if (GUILayout.Button("Add Down", GUILayout.Width(100)))
            {
                ShowAddEventMenu(0);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    void ShowAddEventMenu(int index)
    {

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Dialogue Entry"), false, () => SetAction(index, typeof(DialogueEntry)));
        menu.AddItem(new GUIContent("Move Actor"), false, () => SetAction(index, typeof(ActorMoveEvent)));
        menu.AddItem(new GUIContent("Facing Dir Actor"), false, () => SetAction(index, typeof(ActorFacingEvent)));
        menu.AddItem(new GUIContent("Teleport Actor"), false, () => SetAction(index, typeof(ActorTeleportEvent)));
        menu.AddItem(new GUIContent("Wait Actor"), false, () => SetAction(index, typeof(WaitForActorToEndEvent)));
        menu.AddItem(new GUIContent("Play Animation"), false, () => SetAction(index, typeof(PlayAnimationEvent)));
        menu.ShowAsContext();
    }

    private void SetAction(int index, System.Type eventType)
    {
        action = eventType;
        indexofAction = index;
        doAction = true;
    }
    void AddNewEvent(int index, System.Type eventType)
    {
        Debug.Log(index);
        CutsceneEvent ev;
        if (eventType == typeof(DialogueEntry)) ev = new DialogueEntry();
        else if (eventType == typeof(ActorMoveEvent)) ev = new ActorMoveEvent();
        else if (eventType == typeof(ActorTeleportEvent)) ev = new ActorTeleportEvent();
        else if (eventType == typeof(WaitForActorToEndEvent)) ev = new WaitForActorToEndEvent();
        else if (eventType == typeof(ActorFacingEvent)) ev = new ActorFacingEvent();
        else if (eventType == typeof(PlayAnimationEvent)) ev = new PlayAnimationEvent();
        else ev = new DialogueEntry();

        SerializedProperty newEventProperty;
        
        if (index == 0 && cutscene.entries.Count == 0)
        {
            cutscene.entries.Add(ev);
            cutsceneEvents.InsertArrayElementAtIndex(0);
            newEventProperty = cutsceneEvents.GetArrayElementAtIndex(0);
        }
        else if (index >= cutscene.entries.Count)
        {
            cutscene.entries.Insert(cutscene.entries.Count - 1, ev);
            cutsceneEvents.InsertArrayElementAtIndex(cutscene.entries.Count - 1);
            newEventProperty = cutsceneEvents.GetArrayElementAtIndex(cutscene.entries.Count - 1);
        }
        else if (index <= 0)
        {
            cutscene.entries.Insert(0, ev);
            cutsceneEvents.InsertArrayElementAtIndex(0);
            newEventProperty = cutsceneEvents.GetArrayElementAtIndex(0);
        }
        else
        {
            cutscene.entries.Insert(index, ev);
            cutsceneEvents.InsertArrayElementAtIndex(index);
            newEventProperty = cutsceneEvents.GetArrayElementAtIndex(index);
        }

        newEventProperty.managedReferenceValue = System.Activator.CreateInstance(eventType);
        serializedObject.ApplyModifiedProperties();
    }
}
