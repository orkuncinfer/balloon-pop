using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentFilter))]
public class ComponentFilterEditor : UnityEditor.Editor
{
    // Reference to the ComponentFilter script
    private ComponentFilter filter;

    private void OnEnable()
    {
        filter = (ComponentFilter)target;
    }

    public override void OnInspectorGUI()
    {
        // Manually draw the ComponentFilter fields to ensure they stay visible
        serializedObject.Update();

        EditorGUILayout.LabelField("Component Filter", EditorStyles.boldLabel);

        // Add buttons for collapsing and expanding all components
        if (GUILayout.Button("Collapse All Components"))
        {
            SetAllComponentsVisibility(false);
        }

        if (GUILayout.Button("Expand All Components"))
        {
            SetAllComponentsVisibility(true);
        }

        // Apply any changes to the serializedObject
        serializedObject.ApplyModifiedProperties();

        // Ensure the Inspector is redrawn after toggling visibility
        EditorUtility.SetDirty(target);
        Repaint();
    }

    // Method to set visibility for all components
    private void SetAllComponentsVisibility(bool isVisible)
    {
        Component[] components = filter.GetAllComponents();

        foreach (Component component in components)
        {
            // Skip the ComponentFilter itself
            if (component != filter)
            {
                filter.SetComponentVisibility(component, isVisible);
                ChangeVisibility(component, isVisible);
            }
        }
    }

    // Method to change the visibility of a single component
    private void ChangeVisibility(Component component, bool isVisible)
    {
        if (isVisible)
        {
            component.hideFlags &= ~HideFlags.HideInInspector; // Remove the HideInInspector flag
            CreateEditor(component); // Refresh the editor view for the component
        }
        else
        {
            component.hideFlags |= HideFlags.HideInInspector; // Add the HideInInspector flag
        }

        EditorUtility.SetDirty(component); // Mark the component as dirty to refresh the inspector
    }
}
