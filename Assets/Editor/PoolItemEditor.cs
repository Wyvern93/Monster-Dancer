using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoolData))]
public class PoolItemEditor : Editor
{
    private int selectedIndex = -1;

    public override void OnInspectorGUI()
    {
        PoolData poolItem = (PoolData)target;

        // Display Prefab field
        poolItem.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", poolItem.prefab, typeof(GameObject), false);

        // Display number of instances
        poolItem.count = EditorGUILayout.IntField("Number of Instances", poolItem.count);

        // Only show component dropdown if prefab is assigned
        if (poolItem.prefab != null)
        {
            // Get all components attached to the prefab
            Component[] components = poolItem.prefab.GetComponents<Component>();

            // Filter out any base MonoBehaviour classes (like Transform, etc.)
            var validComponents = components
                .Where(comp => !(comp is Transform))  // Exclude Transform (common on all GameObjects)
                .Select(comp => comp) // Get the component itself
                .ToArray();

            // Display the dropdown for selecting a component
            if (validComponents.Length > 0)
            {
                // Create a dropdown menu from the valid component types
                string[] componentNames = validComponents.Select(comp => comp.GetType().Name).ToArray();
                selectedIndex = Mathf.Max(0, ArrayUtility.IndexOf(componentNames, poolItem.component?.GetType().Name));
                selectedIndex = EditorGUILayout.Popup("Component Type", selectedIndex, componentNames);

                // Set the selected component reference
                poolItem.component = validComponents[selectedIndex];
            }
            else
            {
                EditorGUILayout.HelpBox("No valid components found on this prefab.", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign a prefab first.", MessageType.Info);
        }

        // Mark the object as dirty so changes are saved
        EditorUtility.SetDirty(target);
    }
}