using System.Collections.Generic;
using UnityEngine;

public class GraphDrawer : MonoBehaviour
{
    public float ZoomLevel = 1.0f;            // Used to track zoom level
    public Vector2 Drag;
    public float ScreenWidth;
    public float ScreenHeight;

    [System.Serializable]
    public class Node
    {
        public Vector2 Position;
        public Vector2 Size = new Vector2(100, 50); // Size of the node rectangle
        public string Name;
        public List<int> ConnectionIndexes = new List<int>(); // List of connected nodes
        public Rect Rect; // Position and size of the node rectangle
    }

    public List<Node> Nodes = new List<Node>();
}