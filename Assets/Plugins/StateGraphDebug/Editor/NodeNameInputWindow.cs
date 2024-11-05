using UnityEditor;
using UnityEngine;

public class NodeNameInputWindow : EditorWindow
{
    private static string _nodeName = "New Node";
    private static System.Action<string> _onNameSet;
    private bool _shouldFocusTextField = true;

    public static void ShowWindow(System.Action<string> onNameSet,Vector2 mousePosition)
    {
        _onNameSet = onNameSet;
        NodeNameInputWindow window = GetWindow<NodeNameInputWindow>("Create Node");
        window.minSize = new Vector2(300, 80);
        window.maxSize = new Vector2(300, 80);
        window.position = new Rect(mousePosition.x + (Screen.width * 3), mousePosition.y + Screen.height / 2, 300, 80);
        window._shouldFocusTextField = true;
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Node Name", EditorStyles.boldLabel);
        GUI.SetNextControlName("NodeNameTextField");
        _nodeName = EditorGUILayout.TextField(_nodeName);

        if (_shouldFocusTextField)
        {
            _shouldFocusTextField = false;
            EditorGUI.FocusTextInControl("NodeNameTextField");
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create", GUILayout.Width(140)))
        {
            CreateNode();
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(140)))
        {
            Close();
        }
        GUILayout.EndHorizontal();

        // Detect "Enter" key to trigger "Create"
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
        {
            CreateNode();
        }
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
        {
            Close();
        }
    }

    private void CreateNode()
    {
        if (!string.IsNullOrEmpty(_nodeName))
        {
            _onNameSet?.Invoke(_nodeName);
            Close();
        }
        else
        {
            Debug.LogWarning("Please provide a valid name.");
        }
    }
}