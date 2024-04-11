using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxDrawer))]
public class BoxDrawerEditor : Editor
{
    void OnSceneGUI()
    {
        BoxDrawer boxDrawer = (BoxDrawer)target;

        if(!boxDrawer.DrawDebug) return;
        // Calculate world space positions of the corners based on offsets
        Vector3 worldBottomLeft = boxDrawer.transform.position + boxDrawer.transform.TransformDirection(boxDrawer.bottomLeft);
        Vector3 worldOppositeCorner = boxDrawer.transform.position + boxDrawer.transform.TransformDirection(boxDrawer.oppositeCorner);

        EditorGUI.BeginChangeCheck();
        float handleSize = HandleUtility.GetHandleSize(boxDrawer.bottomLeft) * 0.1f;
        float oppositeHandleSize = HandleUtility.GetHandleSize(boxDrawer.oppositeCorner) * 0.1f;


        // Custom handle cap function
        Handles.CapFunction cap = Handles.SphereHandleCap;
        // Use handles to manipulate the corners in world space
        var fmh_22_78_638474394275121225 = Quaternion.identity; Vector3 newWorldBottomLeft = Handles.FreeMoveHandle(worldBottomLeft, handleSize, Vector3.zero, cap);
        var fmh_23_87_638474394275141751 = Quaternion.identity; Vector3 newWorldOppositeCorner =  Handles.FreeMoveHandle(worldOppositeCorner, oppositeHandleSize, Vector3.zero, cap);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(boxDrawer, "Move Box Corners");
            // Convert back to offsets from the center after moving
            boxDrawer.bottomLeft = boxDrawer.transform.InverseTransformDirection(newWorldBottomLeft - boxDrawer.transform.position);
            boxDrawer.oppositeCorner = boxDrawer.transform.InverseTransformDirection(newWorldOppositeCorner - boxDrawer.transform.position);
            EditorUtility.SetDirty(boxDrawer);
        }
        
        Vector3[] corners = new Vector3[8];

        Vector3 bottomLeft = newWorldBottomLeft;
        Vector3 topRight = newWorldOppositeCorner;
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
        
        corners[0] = bottomLeft; // Bottom-left front
        corners[1] = new Vector3(bottomRight.x, bottomLeft.y, bottomLeft.z); // Bottom-right front
        corners[2] = new Vector3(topRight.x, topRight.y, bottomLeft.z); // Top-right front
        corners[3] = new Vector3(topLeft.x, topRight.y, bottomLeft.z); // Top-left front
        corners[4] = new Vector3(bottomLeft.x, bottomLeft.y, topRight.z); // Bottom-left back
        corners[5] = new Vector3(bottomRight.x, bottomLeft.y, topRight.z); // Bottom-right back
        corners[6] = topRight; // Top-right back
        corners[7] = new Vector3(topLeft.x, topRight.y, topRight.z); // Top-left back

        // Draw the box edges
        Handles.color = Color.green;
        // Bottom
        Handles.DrawLine(corners[0], corners[1]);
        Handles.DrawLine(corners[1], corners[5]);
        Handles.DrawLine(corners[5], corners[4]);
        Handles.DrawLine(corners[4], corners[0]);
        // Top
        Handles.DrawLine(corners[3], corners[2]);
        Handles.DrawLine(corners[2], corners[6]);
        Handles.DrawLine(corners[6], corners[7]);
        Handles.DrawLine(corners[7], corners[3]);
        // Sides
        Handles.DrawLine(corners[0], corners[3]);
        Handles.DrawLine(corners[1], corners[2]);
        Handles.DrawLine(corners[4], corners[7]);
        Handles.DrawLine(corners[5], corners[6]);
    }
    
}
