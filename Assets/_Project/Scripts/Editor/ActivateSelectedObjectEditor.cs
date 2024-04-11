using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActivateSelectedObject))]
public class ActivateSelectedObjectEditor : Editor
{
    private ActivateSelectedObject selectedObject;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        selectedObject = (ActivateSelectedObject) target;
        
        selectedObject.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (selectedObject != null)
        {
            selectedObject.gameObject.SetActive(false);
        }
    }
}