using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphDrawer))]
public class GraphDrawerEditor : Editor
{
    private const float GridSize = 20f;        // Base size of each cell in the grid
    private Vector2 _drag;                     // Track drag offset
    private Vector2 _rightClickPos;            // Track right-click position for adding nodes
    private GraphDrawer.Node _selectedNode;    // Currently selected node for dragging
    private bool _isDraggingNode = false;      // Flag for dragging state
    private GraphDrawer.Node _transitionStartNode = null;  // Node from which a transition starts

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();              // Draws the default inspector properties
        
        // Set up a resizable area in the inspector
        EditorGUILayout.Space();
        Rect graphRect = GUILayoutUtility.GetRect(300, 300, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUI.Box(graphRect, GUIContent.none);  // Background box for visual clarity

        // Start masking content to the graph area
        GUI.BeginClip(graphRect);
        
        // Offset all drawing by the position of the graph area
        Rect clippedRect = new Rect(0, 0, graphRect.width, graphRect.height);
        DrawGrid(clippedRect);
        DrawConnections(clippedRect);
        DrawNodes(clippedRect);

        // End masking
        GUI.EndClip();

        // Handle user inputs for dragging, zooming, and right-click to add nodes
        ProcessEvents(graphRect);
    }

    private void DrawGrid(Rect rect)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        float zoomedGridSize = GridSize * graphDrawer.ZoomLevel;
        
        // Offset the grid based on zoom level and drag position
        Vector2 offset = graphDrawer.GridOffset + _drag;
        
        Handles.BeginGUI();

        // Draw vertical lines
        for (float x = 0; x < rect.width; x += zoomedGridSize)
        {
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(x + offset.x % zoomedGridSize, 0), new Vector2(x + offset.x % zoomedGridSize, rect.height));
        }

        // Draw horizontal lines
        for (float y = 0; y < rect.height; y += zoomedGridSize)
        {
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(0, y + offset.y % zoomedGridSize), new Vector2(rect.width, y + offset.y % zoomedGridSize));
        }

        Handles.EndGUI();
    }

    private void DrawConnections(Rect rect)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        
        // Draw all persistent connections between nodes
        foreach (var node in graphDrawer.Nodes)
        {
            foreach (var connectedNode in node.Connections)
            {
                DrawBezierCurve(rect, node, connectedNode, Color.white);
            }
        }

        // Draw transition preview if a transition is in progress
        if (_transitionStartNode != null)
        {
            DrawTransitionPreview(rect);
        }
    }

    private void DrawNodes(Rect rect)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        
        Handles.BeginGUI();
        
        for (int i = 0; i < graphDrawer.Nodes.Count; i++)
        {
            var node = graphDrawer.Nodes[i];
            Vector2 nodePos = new Vector2(node.Position.x * graphDrawer.ZoomLevel + graphDrawer.GridOffset.x + _drag.x,
                                          node.Position.y * graphDrawer.ZoomLevel + graphDrawer.GridOffset.y + _drag.y);
            Rect nodeRect = new Rect(nodePos, node.Size * graphDrawer.ZoomLevel);
            
            // Draw node as a gray rectangle
            EditorGUI.DrawRect(nodeRect, Color.gray);
            GUI.Label(new Rect(nodePos.x + 10, nodePos.y + 10, node.Size.x * graphDrawer.ZoomLevel - 20, 20), node.Name);

            // Check if the current event is a click on this node for selection or context menu
            ProcessNodeEvents(node, nodeRect, i);
        }
        
        Handles.EndGUI();
    }

    private void ProcessNodeEvents(GraphDrawer.Node node, Rect nodeRect, int nodeIndex)
    {
        Event e = Event.current;
        GraphDrawer graphDrawer = (GraphDrawer)target;

        // Only process node events if we are not in transition mode
        if (_transitionStartNode == null)
        {
            // Handle node dragging
            if (e.type == EventType.MouseDown && e.button == 0 && nodeRect.Contains(e.mousePosition))
            {
                _selectedNode = node;
                _isDraggingNode = true;
                e.Use();
            }

            if (e.type == EventType.MouseUp && e.button == 0)
            {
                _isDraggingNode = false;
                _selectedNode = null;
            }

            if (_isDraggingNode && _selectedNode == node && e.type == EventType.MouseDrag && e.button == 0)
            {
                node.Position += e.delta / graphDrawer.ZoomLevel;
                e.Use();
                Repaint();
            }
        }

        // Handle right-click context menu for adding a transition or deleting a node
        if (e.type == EventType.ContextClick && nodeRect.Contains(e.mousePosition))
        {
            _rightClickPos = node.Position;
            ShowNodeContextMenu(nodeIndex);
            e.Use();
        }

        // Finalize transition if clicking on another node
        if (_transitionStartNode != null && e.type == EventType.MouseDown && e.button == 0 && nodeRect.Contains(e.mousePosition))
        {
            if (_transitionStartNode != node)
            {
                // Create a new connection between the start node and this node
                _transitionStartNode.Connections.Add(node);
                EditorUtility.SetDirty(graphDrawer);  // Save the connection change
            }
            _transitionStartNode = null;  // Reset transition
            e.Use();
            Repaint();
        }
    }

    private void ProcessEvents(Rect graphRect)
    {
        Event e = Event.current;
        GraphDrawer graphDrawer = (GraphDrawer)target;

        // Handle panning
        if (e.type == EventType.MouseDrag && e.button == 0 && graphRect.Contains(e.mousePosition) && !_isDraggingNode && _transitionStartNode == null)
        {
            _drag += e.delta;
            e.Use();
            Repaint();
        }

        // Handle zooming
        if (e.type == EventType.ScrollWheel && graphRect.Contains(e.mousePosition))
        {
            float zoomChange = -e.delta.y * 0.01f;
            graphDrawer.ZoomLevel = Mathf.Clamp(graphDrawer.ZoomLevel + zoomChange, 0.1f, 5f);
            e.Use();
            Repaint();
        }

        // Handle right-click to add a node in an empty area if not in transition mode
        if (e.type == EventType.ContextClick && graphRect.Contains(e.mousePosition) && _transitionStartNode == null)
        {
            _rightClickPos = e.mousePosition - graphRect.position - _drag - graphDrawer.GridOffset;
            ShowAddNodeContextMenu();
            e.Use();
        }

        // Cancel transition if clicking on an empty area
        if (_transitionStartNode != null && e.type == EventType.MouseDown && e.button == 0 && graphRect.Contains(e.mousePosition))
        {
            _transitionStartNode = null;
            e.Use();
            Repaint();
        }
    }

    private void ShowAddNodeContextMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Node"), false, AddNode);
        menu.ShowAsContext();
    }

    private void ShowNodeContextMenu(int nodeIndex)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Transition"), false, () => StartTransition(nodeIndex));
        menu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(nodeIndex));
        menu.ShowAsContext();
    }

    private void StartTransition(int nodeIndex)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        _transitionStartNode = graphDrawer.Nodes[nodeIndex];
    }

    private void AddNode()
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        
        GraphDrawer.Node newNode = new GraphDrawer.Node
        {
            Position = _rightClickPos / graphDrawer.ZoomLevel,
            Name = "Node " + (graphDrawer.Nodes.Count + 1)
        };
        
        graphDrawer.Nodes.Add(newNode);
        EditorUtility.SetDirty(graphDrawer);  // Mark as dirty to save changes
        Repaint();
    }

    private void DeleteNode(int nodeIndex)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        
        graphDrawer.Nodes.RemoveAt(nodeIndex);
        EditorUtility.SetDirty(graphDrawer);  // Mark as dirty to save changes
        Repaint();
    }

    private void DrawBezierCurve(Rect rect, GraphDrawer.Node startNode, GraphDrawer.Node endNode, Color color)
    {
        Vector2 startPos = new Vector2(startNode.Position.x * ((GraphDrawer)target).ZoomLevel + ((GraphDrawer)target).GridOffset.x + _drag.x + startNode.Size.x / 2,
                                       startNode.Position.y * ((GraphDrawer)target).ZoomLevel + ((GraphDrawer)target).GridOffset.y + _drag.y + startNode.Size.y / 2);
        Vector2 endPos = new Vector2(endNode.Position.x * ((GraphDrawer)target).ZoomLevel + ((GraphDrawer)target).GridOffset.x + _drag.x + endNode.Size.x / 2,
                                     endNode.Position.y * ((GraphDrawer)target).ZoomLevel + ((GraphDrawer)target).GridOffset.y + _drag.y + endNode.Size.y / 2);

        Handles.DrawBezier(
            startPos,
            endPos,
            startPos + Vector2.right * 50f,
            endPos + Vector2.left * 50f,
            color,
            null,
            6f
        );
    }

    private void DrawTransitionPreview(Rect rect)
    {
        Vector2 startPos = new Vector2(_transitionStartNode.Position.x * ((GraphDrawer)target).ZoomLevel + ((GraphDrawer)target).GridOffset.x + _drag.x + _transitionStartNode.Size.x / 2,
                                       _transitionStartNode.Position.y * ((GraphDrawer)target).ZoomLevel + ((GraphDrawer)target).GridOffset.y + _drag.y + _transitionStartNode.Size.y / 2);
        Vector2 endPos = Event.current.mousePosition - rect.position;

        Handles.DrawBezier(
            startPos,
            endPos,
            startPos + Vector2.right * 50f,
            endPos + Vector2.left * 50f,
            Color.white,
            null,
            6f
        );
        Repaint();
    }
}
