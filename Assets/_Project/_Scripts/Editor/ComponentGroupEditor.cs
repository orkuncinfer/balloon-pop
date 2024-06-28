using UnityEngine;

#if UNITY_EDITOR
    namespace Editor
    {
        using UnityEditor;
 
        [CustomEditor(typeof(ComponentGrouper))]
        public class ComponentGroupEditor : UnityEditor.Editor
        {
            ComponentGrouper groups;
 
            private void OnEnable()
            {
                groups = (ComponentGrouper) target;
                foreach (var group in groups.groups)
                {
                    for (int i = group.Components.Count - 1; i >= 0; i--)
                    {
                        if (group.Components[i] == null)
                            group.Components.RemoveAt(i);
                        else
                        {
                            ChangeVisibility(group, group.IsVisible);
                        }
                    }
                }
            }
 
            private void ChangeVisibility(ComponentGroup group, bool aVisible)
            {
                for (int i = group.Components.Count - 1; i >= 0; i--)
                {
                    var c = group.Components[i];
                    if (c == null)
                    {
                        group.Components.RemoveAt(i);
                        continue;
                    }
 
                    if (aVisible)
                    {
                        c.hideFlags &= ~HideFlags.HideInInspector;
                        // required if the object was deselected in between
                        CreateEditor(c);
                    }
                    else
                        c.hideFlags |= HideFlags.HideInInspector;
                }
 
                group.IsVisible = aVisible;
 
                EditorUtility.SetDirty(target);
            }
            
            private void ChangeVisibility(Component component, bool aVisible)
            {
                if (aVisible)
                {
                    component.hideFlags &= ~HideFlags.HideInInspector;
                    // required if the object was deselected in between
                    CreateEditor(component);
                }
                else
                    component.hideFlags |= HideFlags.HideInInspector;
 
                EditorUtility.SetDirty(target);
            }
 
            public override void OnInspectorGUI()
            {
                var oldColor   = GUI.color;
                var oldEnabled = GUI.enabled;
 
                if (GUILayout.Button("Add Group"))
                {
                    groups.groups.Add(new ComponentGroup());
                }
 
                for (var i = 0; i < groups.groups.Count; i++)
                {
                    var group = groups.groups[i];
                    if (group.IsEditable)
                    {
                        var components = groups.gameObject.GetComponents<Component>();
                        GUILayout.BeginHorizontal();
                        group.Name = GUILayout.TextField(group.Name);
                        if (GUILayout.Button("done", GUILayout.Width(40)))
                            group.IsEditable = false;
                        GUILayout.EndHorizontal();
                        foreach (var comp in components)
                        {
                            string name = comp.GetType().Name;
                            if (comp is ComponentGrouper g)
                                name = "ComponentGroup";
                            bool           isInList         = group.Components.Contains(comp);
                            ComponentGroup componentInGroup = groups.FindGroup(comp);
 
                            GUI.color   = isInList ? Color.green : oldColor;
                            GUI.enabled = comp != groups && (componentInGroup == null || componentInGroup == group);
                            if ((comp.hideFlags & HideFlags.HideInInspector) != 0)
                                name += "(hidden)";
                            if (GUILayout.Toggle(isInList, name) != isInList)
                            {
                                if (isInList)
                                    group.Components.Remove(comp);
                                else
                                    group.Components.Add(comp);
                            }
                        }
 
                        GUI.enabled = oldEnabled;
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        if(group.IsVisible) GUI.color = Color.green;
                        
                        if (GUILayout.Button(group.Name))
                            ChangeVisibility(group, !group.IsVisible);
                        if (GUILayout.Button("hide", GUILayout.Width(40)))
                            ChangeVisibility(group, false);
                        GUI.color = Color.green;
                        if (GUILayout.Button("show", GUILayout.Width(40)))
                            ChangeVisibility(group, true);
                        GUI.color = oldColor;
                        if (GUILayout.Button("edit", GUILayout.Width(40)))
                        {
                            //ChangeVisibility(group, true);
                            group.IsEditable = true;
                        }
 
                        GUI.enabled = i > 0;
                        if (GUILayout.Button("^", GUILayout.Width(20)))
                        {
                            ComponentGroup moveGroup = groups.groups[i];
                            groups.groups.RemoveAt(i);
                            groups.groups.Insert(i - 1, moveGroup);
                            EditorUtility.SetDirty(target);
                        }
 
                        GUI.enabled = i < groups.groups.Count - 1;
                        if (GUILayout.Button("v", GUILayout.Width(20)))
                        {
                            ComponentGroup moveGroup = groups.groups[i];
                            groups.groups.RemoveAt(i);
                            groups.groups.Insert(i + 1, moveGroup);
                            EditorUtility.SetDirty(target);
                        }
 
                        GUI.enabled = true;
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            groups.groups.RemoveAt(i);
                        }
 
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
#endif