using System;
using UnityEngine;

public class BoxDrawer : MonoBehaviour
{
    public Vector3 bottomLeft;
    public Vector3 oppositeCorner;
    public bool DrawDebug;
    public LayerMask LayerMask;
    public BoxCollider Collider;

    public Vector3 GetBoxCenter()
    {
        // Since bottomLeft and oppositeCorner are in local space, convert their midpoint to world space
        Vector3 centerLocal = (bottomLeft + oppositeCorner) * 0.5f;
        return transform.TransformPoint(centerLocal);
    }

    // Returns the half extents of the box in world space
    public Vector3 GetBoxHalfExtents()
    {
        // The scale of the Transform component should be considered to get the correct size in world space
        Vector3 sizeLocal = (oppositeCorner - bottomLeft) * 0.5f; // Local space half extents
        Vector3 sizeWorld = Vector3.Scale(sizeLocal, transform.lossyScale); // Adjusted by the GameObject's scale
        return sizeWorld;
    }

   

    private void FixedUpdate()
    {
        PerformBoxCast();
    }

    public void PerformBoxCast()
    {
        Vector3 worldCenter = Collider.transform.TransformPoint(Collider.center);
        Vector3 worldHalfExtents = Collider.size * 0.5f; // only necessary when collider is scaled by non-uniform transform
        Vector3 direction = transform.forward; // Assuming you're casting in the GameObject's forward direction
        float maxDistance = 10f; // Max distance for the cast
        RaycastHit hitInfo;

        // Perform the BoxCast
       
        
        Collider[] hitColliders = Physics.OverlapBox(worldCenter, worldHalfExtents, transform.rotation,LayerMask,QueryTriggerInteraction.Collide);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            Debug.Log("Hit : " + hitColliders[i].name + i);
            //Increase the number of Colliders in the array
            i++;
        }
        // ExtDebug.DrawBox(center,halfExtents,transform.rotation,Color.red);
    
    }

    public void DrawBoxForDebug(float duration)
    {
        Vector3 center = GetBoxCenter();
        Vector3 halfExtents = GetBoxHalfExtents();

        Quaternion orientation = transform.rotation;
        Vector3[] directions = {
            orientation * Vector3.up,
            orientation * Vector3.down,
            orientation * Vector3.left,
            orientation * Vector3.right,
            orientation * Vector3.forward,
            orientation * Vector3.back
        };

        // Points relative to the center
        Vector3[] points = {
            center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z),
            center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z),
            center + orientation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z),
            center + orientation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z),
            center + orientation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z),
            center + orientation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z),
            center + orientation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z),
            center + orientation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z)
        };

        // Draw bottom and top face
        Debug.DrawLine(points[0], points[1], Color.red, duration);
        Debug.DrawLine(points[1], points[5], Color.red, duration);
        Debug.DrawLine(points[5], points[4], Color.red, duration);
        Debug.DrawLine(points[4], points[0], Color.red, duration);

        Debug.DrawLine(points[2], points[3], Color.red, duration);
        Debug.DrawLine(points[3], points[7], Color.red, duration);
        Debug.DrawLine(points[7], points[6], Color.red, duration);
        Debug.DrawLine(points[6], points[2], Color.red, duration);

        // Draw sides
        Debug.DrawLine(points[0], points[2], Color.red, duration);
        Debug.DrawLine(points[1], points[3], Color.red, duration);
        Debug.DrawLine(points[4], points[6], Color.red, duration);
        Debug.DrawLine(points[5], points[7], Color.red, duration);
    }
}