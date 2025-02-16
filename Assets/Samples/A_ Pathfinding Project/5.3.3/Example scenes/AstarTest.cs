using System;
using System.Collections;
using System.Collections.Generic;
using ECM2;
using Pathfinding;
using Pathfinding.Util;
using UnityEngine;

public class AstarTest : MonoBehaviour
{
    public AIPath AIPath;
    public Seeker Seeker;
    public Character Character;

    public Vector3 Direction;
    public Transform MovementTransform;
    public float EndReachDistance;

    public List<Vector3> RemainingPathBuffer = new List<Vector3>();
    public List<PathPartWithLinkInfo> RemainingPathLinkBuffer = new List<PathPartWithLinkInfo>();

    private void Start()
    {
        //Character.rotationMode = Character.RotationMode.OrientRotationToMovement;
        Seeker.pathCallback += PathCallback;
    }

    private void PathCallback(Path p)
    {
        Vector3 direction;
        float distance = Vector3.Distance(MovementTransform.position, AIPath.destination);
        if (p.vectorPath.Count > 0 && distance > EndReachDistance && p.vectorPath.Count > 1)
        {
            direction = p.vectorPath[1] - MovementTransform.position;
            direction = direction.normalized;
            direction = new Vector3(direction.x, direction.z,0);
            Direction = direction;
        }
        else
        {
            Direction = Vector2.zero;
        }
    }

    private void Update()
    {
        Vector3 movementDirection =  Vector3.zero;
            
        movementDirection += Vector3.right * Direction.x;
        movementDirection += Vector3.forward * Direction.y;

        Character.SetMovementDirection(movementDirection);
    }
}
