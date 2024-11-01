using System.Collections.Generic;
using UnityEngine;

public class GraphDrawer : MonoBehaviour
{
    public Vector2 GridOffset = Vector2.zero; // Used to track panning
    public float ZoomLevel = 1.0f;            // Used to track zoom level

    [System.Serializable]
    public class Node
    {
        public Vector2 Position;
        public Vector2 Size = new Vector2(100, 50); // Size of the node rectangle
        public string Name;
        public List<Node> Connections = new List<Node>(); // List of connected nodes
    }

    public List<Node> Nodes = new List<Node>();
}