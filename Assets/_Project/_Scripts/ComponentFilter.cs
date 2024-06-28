using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ComponentFilter : MonoBehaviour
{
    // Dictionary to store the visibility state of each component
    private Dictionary<Component, bool> componentVisibility = new Dictionary<Component, bool>();

    // Method to set the visibility state for a component
    public void SetComponentVisibility(Component component, bool isVisible)
    {
        if (componentVisibility.ContainsKey(component))
        {
            componentVisibility[component] = isVisible;
        }
        else
        {
            componentVisibility.Add(component, isVisible);
        }
    }

    // Method to get the visibility state for a component
    public bool GetComponentVisibility(Component component)
    {
        if (componentVisibility.TryGetValue(component, out bool isVisible))
        {
            return isVisible;
        }
        return true; // Default to visible if not found in the dictionary
    }

    // Method to get all components except this ComponentFilter
    public Component[] GetAllComponents()
    {
        Component[] components = GetComponents<Component>();
        List<Component> componentList = new List<Component>();

        foreach (Component component in components)
        {
            if (component != this) // Skip the ComponentFilter itself
            {
                componentList.Add(component);
            }
        }

        return componentList.ToArray();
    }
}