using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(GraphDrawer))]
public class GraphDrawerEditor : Editor
{

    private const float GridSize = 20f;        // Base size of each cell in the grid
    private Vector2 _drag;                     // Track drag offset
    private Vector2 _rightClickPos;            // Track right-click position for adding nodes
    private GraphDrawer.Node _selectedNode;    // Currently selected node for dragging
    private bool _isDraggingNode = false;      // Flag for dragging state
    private GraphDrawer.Node _transitionStartNode = null;  // Node from which a transition starts
    
    private GenericStateMachine _genericStateMachine;
    private GraphDrawer _graphDrawer;
    private Vector2 _lastMousePositin;

    private bool _cancelCenter;
    private Vector2 _targetFocusPos;
    private void OnEnable()
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        _genericStateMachine = graphDrawer.GetComponent<GenericStateMachine>();
        _graphDrawer = graphDrawer;
        _cancelCenter = true;
     
    }

   

    private void OnDisable()
    {
        _cancelCenter = true;
    }
    
    private void CenterOnNode(GraphDrawer.Node node)
    {
        _targetFocusPos = new Vector2((Screen.width / 2) - (node.Position.x * _graphDrawer.ZoomLevel) - (node.Rect.width ) + (node.Rect.width / 2) ,
            300 / 2f - (node.Position.y * _graphDrawer.ZoomLevel) - node.Rect.height / 2);
        _drag = _targetFocusPos;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();              // Draws the default inspector properties
        _lastMousePositin = Event.current.mousePosition;
        // Set up a resizable area in the inspector
        if (_genericStateMachine == null)
        {
            GraphDrawer graphDrawer = (GraphDrawer)target;
            _genericStateMachine = graphDrawer.GetComponent<GenericStateMachine>();
            return;
        }
        if(_graphDrawer.ZoomLevel <= 0.1f) _graphDrawer.ZoomLevel = 0.1f;
       
            
        if (_genericStateMachine.IsRunning)
        {
            string runningName = _genericStateMachine.CurrentState.name;
            GraphDrawer.Node runningNode = _graphDrawer.Nodes.FirstOrDefault(n => n.Name == runningName);
            CenterOnNode(runningNode);
        }
        else
        {
            if (_cancelCenter)
            {
                if(_graphDrawer.Nodes.Count > 0)
                    CenterOnNode(_graphDrawer.Nodes[0]);
            }
        }
            
   
        _graphDrawer.Drag = _drag;
        _graphDrawer.ScreenWidth = Screen.width;
        _graphDrawer.ScreenHeight = Screen.height;


        foreach (var state in _genericStateMachine.States)
        {
            if(state.State == null) continue;
            if(_graphDrawer.Nodes.Count < _genericStateMachine.States.Count)
            {
                Vector2 randomPos = new Vector2(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100));
                GraphDrawer.Node newNode = new GraphDrawer.Node
                {
                    Position = randomPos,
                    Name = state.State.name
                };
                _graphDrawer.Nodes.Add(newNode);
            }
        }
        
        EditorGUILayout.Space();
        Rect graphRect = GUILayoutUtility.GetRect(300, 300, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUI.Box(graphRect, GUIContent.none);
        // Start masking content to the graph area
        GUI.BeginClip(graphRect);
        
        // Offset all drawing by the position of the graph area
        Rect clippedRect = new Rect(0, 0, graphRect.width, graphRect.height);
        DrawGrid(clippedRect);
        HighlightRunningNodes(clippedRect);
        DrawConnections(clippedRect);
        DrawNodes(clippedRect);

        // End masking
        GUI.EndClip();

        // Handle user inputs for dragging, zooming, and right-click to add nodes
        ProcessEvents(graphRect);
    }

    private void HighlightRunningNodes(Rect clippedRect)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        for (int i = 0; i < _genericStateMachine.States.Count; i++)
        {
            if(_genericStateMachine.States[i].State == null)
                continue;
            var node = graphDrawer.Nodes[i];
            Vector2 nodePos = new Vector2(node.Position.x * graphDrawer.ZoomLevel  + _drag.x,
                                          node.Position.y * graphDrawer.ZoomLevel  + _drag.y);
            Rect nodeRect = new Rect(nodePos, node.Size * graphDrawer.ZoomLevel);
            nodeRect.width += 10;
            nodeRect.height += 10;
            nodeRect.x -= 5;
            nodeRect.y -= 5;
            if (_genericStateMachine.States[i].State.IsRunning)
            {
                EditorGUI.DrawRect(nodeRect, Color.green);
            }
        }
    }

    private void DrawGrid(Rect rect)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        float zoomedGridSize = GridSize * graphDrawer.ZoomLevel;
        
        // Offset the grid based on zoom level and drag position
        Vector2 offset = _drag;
        
        Handles.BeginGUI();

        // Draw vertical lines
        for (float x = 0; x < rect.width; x += zoomedGridSize)
        {
            Handles.color = new Color(0.161f, 0.169f, 0.169f);
            Handles.DrawLine(new Vector2(x + offset.x % zoomedGridSize, 0), new Vector2(x + offset.x % zoomedGridSize, rect.height));
        }

        // Draw horizontal lines
        for (float y = 0; y < rect.height; y += zoomedGridSize)
        {
            Handles.color = new Color(0.161f, 0.169f, 0.169f);
            Handles.DrawLine(new Vector2(0, y + offset.y % zoomedGridSize), new Vector2(rect.width, y + offset.y % zoomedGridSize));
        }

        Handles.EndGUI();
    }

    private void DrawConnections(Rect rect)
    {
        Handles.BeginGUI();
        Handles.color = Color.white;
        GraphDrawer graphDrawer = (GraphDrawer)target;
        // Draw all persistent connections between nodes
        /*foreach (var node in graphDrawer.Nodes)
        {
            for (int i = 0; i < node.ConnectionIndexes.Count; i++)
            {
                DrawBezierCurve(rect, node, graphDrawer.Nodes[node.ConnectionIndexes[i]], Color.white);
                //DrawConditions(node);
            }
        }*/

        foreach (var state in _genericStateMachine.States)
        {
            for (int i = 0; i < state.Transitions.Count; i++)
            {
                if(state.Transitions[i].ToState == null) continue;
                string fromStateName = state.State.name;
                string toStateName = state.Transitions[i].ToState.name;
                
                GraphDrawer.Node fromNode = graphDrawer.Nodes.FirstOrDefault(n => n.Name == fromStateName);
                GraphDrawer.Node toNode = graphDrawer.Nodes.FirstOrDefault(n => n.Name == toStateName);
                if(fromNode == null || toNode == null) continue;
                DrawBezierCurve(rect, fromNode, toNode, Color.white);
                DrawConditions(state, fromNode, toNode,state.Transitions[i]);
            }
        }

        // Draw transition preview if a transition is in progress
        if (_transitionStartNode != null)
        {
            DrawTransitionPreview(rect);
        }
        Handles.EndGUI();
    }

    private void DrawConditions(StateField fromState, GraphDrawer.Node fromNode, GraphDrawer.Node toNode, StateTransition transition)
    {
        if (transition != null)
        {
            float yOffset = 0;
            foreach (var stateCondition in transition.Conditions)
            {
                if (stateCondition != null)
                {
                    // Define a position to draw the label and symbol
                    Vector2 labelPosition = (fromNode.Rect.position + toNode.Rect.position) / 2 + new Vector2(0, yOffset);
                    string label = stateCondition.GetType().Name;
                    float labelWidth = label.Length * 10;
                    Rect labelRect = new Rect(labelPosition, new Vector2(labelWidth, 20));
                    EditorGUI.LabelField(labelRect, label);

                    yOffset += 20;
                    
                    // Draw the "tick" (✓) or "X" (✗) symbol based on a condition
                    if (_genericStateMachine.IsRunning)
                    {
                        string symbol;
                        symbol = stateCondition.CheckCondition() ? "✓" : "✗"; // Assuming `CheckCondition()` returns a boolean
                        GUIStyle symbolStyle = new GUIStyle(GUI.skin.label)
                        {
                            fontSize = 20,
                            normal = { textColor = stateCondition.CheckCondition() ? Color.green : Color.red }
                        };

                        // Draw the symbol
                        Handles.Label(labelRect.position + new Vector2(-20, labelRect.height / 2), symbol, symbolStyle);
                        
                        
                        //Draw watching values
                        Event e = Event.current;
                        if (labelRect.Contains(e.mousePosition))
                        {
                            float yOffsetValue = 0;
                            foreach (var watchValue in stateCondition.GetWatchingValues())
                            {
                            
                                Handles.Label(labelRect.position + new Vector2(labelRect.width, yOffsetValue + labelRect.height / 2), watchValue);
                                yOffsetValue += 20;
                            }
                        }

                        
                    }
                    else
                    {
                        string symbol;
                        symbol = "✗"; 
                        GUIStyle symbolStyle = new GUIStyle(GUI.skin.label)
                        {
                            fontSize = 20,
                            normal = { textColor = Color.red }
                        };

                        // Draw the symbol
                        Handles.Label(labelRect.position + new Vector2(-20, labelRect.height / 2), symbol, symbolStyle);
                    }
                }
            }
        }
    }

    private void DrawNodes(Rect rect)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        Handles.BeginGUI();

        for (int i = 0; i < _genericStateMachine.States.Count; i++)
        {
            if (_genericStateMachine.States[i].State == null)
                continue;

            var node = graphDrawer.Nodes[i];
            Vector2 nodePos = new Vector2(node.Position.x * graphDrawer.ZoomLevel + _drag.x,
                node.Position.y * graphDrawer.ZoomLevel + _drag.y);
            Rect nodeRect = new Rect(nodePos, node.Size * graphDrawer.ZoomLevel);
            node.Rect = nodeRect;

            // Draw node as a gray rectangle
            EditorGUI.DrawRect(nodeRect, Color.gray);

            // Define the GUIStyle for the label
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,  // Center the text
                wordWrap = false
            };

            // Measure the size of the label text
            string nodeName = _genericStateMachine.States[i].State.name;
            Vector2 textSize = labelStyle.CalcSize(new GUIContent(nodeName));

            // Create a centered label position
            Rect labelRect = new Rect(
                nodePos.x + (nodeRect.width - textSize.x) / 2,  // Center X
                nodePos.y + (nodeRect.height - textSize.y) / 2, // Center Y
                textSize.x,                                     // Width of the text
                textSize.y                                      // Height of the text
            );

            // Draw the centered label
            GUI.Label(labelRect, nodeName, labelStyle);

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
                _transitionStartNode.ConnectionIndexes.Add(nodeIndex);
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
            _cancelCenter = false;
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
            _rightClickPos = e.mousePosition - graphRect.position - _drag;
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
        // Open the name input window and pass a callback to handle the node creation
        NodeNameInputWindow.ShowWindow(name =>
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("Node creation was cancelled or invalid name provided.");
                return; // Cancel creation if name is not provided
            }

            GraphDrawer graphDrawer = (GraphDrawer)target;

            // Create a new GameObject as a child of the _betterStateMachine
            GameObject newNodeObject = new GameObject(name);
            newNodeObject.transform.parent = _genericStateMachine.transform;

            // Add the MonoState component to the new GameObject
            MultiStateComponent newState = newNodeObject.AddComponent<MultiStateComponent>();

            // Create a new StateField and add it to the _states list in _betterStateMachine
            StateField newStateField = new StateField { State = newState };
            _genericStateMachine.States.Add(newStateField);

            // Add the new node to the GraphDrawer's node list for visualization
            GraphDrawer.Node newNode = new GraphDrawer.Node
            {
                Position = _rightClickPos / graphDrawer.ZoomLevel,
                Name = name
            };
            graphDrawer.Nodes.Add(newNode);

            // Mark the graph drawer and better state machine as dirty to save the changes
            EditorUtility.SetDirty(graphDrawer);
            EditorUtility.SetDirty(_genericStateMachine);
            Repaint();
        }, _lastMousePositin);
    }


    private void DeleteNode(int nodeIndex)
    {
        GraphDrawer graphDrawer = (GraphDrawer)target;
        
        graphDrawer.Nodes.RemoveAt(nodeIndex);
        EditorUtility.SetDirty(graphDrawer);  // Mark as dirty to save changes
        Repaint();
    }
    public static void DrawBezier(Vector3 startPos, Vector3 endPos, bool left)
    {
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;

        Color shadow = new Color(1, 1, 1, 0.1f);
        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos,endPos,startTan,endTan,shadow,null,(i+1)* 2);
        }
        Handles.DrawBezier(startPos,endPos,startTan,endTan,Color.white, null,2); 
    }
    private void DrawBezierCurve(Rect rect, GraphDrawer.Node startNode, GraphDrawer.Node endNode, Color color)
    {
        Vector2 startPos = new Vector2(startNode.Position.x * ((GraphDrawer)target).ZoomLevel + (_drag.x + startNode.Size.x / 2),
                                       startNode.Position.y * ((GraphDrawer)target).ZoomLevel + (_drag.y + startNode.Size.y / 2));

        startPos = startNode.Rect.position + new Vector2(endNode.Rect.width/ 2, endNode.Rect.height / 2);
        
        Vector2 endPos = new Vector2(endNode.Position.x * ((GraphDrawer)target).ZoomLevel + (_drag.x + endNode.Size.x / 2),
                                     endNode.Position.y * ((GraphDrawer)target).ZoomLevel + ( _drag.y + endNode.Size.y / 2));
        endPos = endNode.Rect.position + new Vector2(endNode.Rect.width/ 2, endNode.Rect.height / 2);
        
        
        DrawBezier(startPos, endPos, true);
    }

    private void DrawTransitionPreview(Rect rect)
    {
        Vector2 startPos = new Vector2(_transitionStartNode.Position.x * ((GraphDrawer)target).ZoomLevel + (_drag.x + _transitionStartNode.Size.x / 2),
                                       _transitionStartNode.Position.y * ((GraphDrawer)target).ZoomLevel + (_transitionStartNode.Size.y / 2));
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
    
#endif
