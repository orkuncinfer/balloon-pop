using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(BoringSO))]
public class BoringEditor : Editor
{
    private PreviewRenderUtility _previewRenderUtility;
    private MeshFilter _targetMeshFilter;
    private MeshRenderer _targetMeshRenderer;

    //Fetch all the relevent information
    private void ValidateData()
    {
        if (_previewRenderUtility == null)
        {
            _previewRenderUtility = new PreviewRenderUtility();
            
            //We set the previews camera to 6 units back, look towards the middle of the 'scene'
            _previewRenderUtility.camera.transform.position = new Vector3(0, 0, -6);
            _previewRenderUtility.camera.transform.rotation = Quaternion.identity;
        }

        //We'll need the GO's mesh filter and renderer
        //to be able to render a preview of the mesh!

        if (_targetMeshFilter == null)
        {
            GameObject goToSpawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _targetMeshFilter = goToSpawn.GetComponent<MeshFilter>();
            _targetMeshRenderer = goToSpawn.GetComponent<MeshRenderer>();
        }
        BoringSO boringSO = (BoringSO)target;
    }


    public override bool HasPreviewGUI()
    {
        //Validate data - this is always called before OnPreviewGUI
        ValidateData();

        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        // Only render our 3D 'preview' when the UI is 'repainting'.
        if (Event.current.type == EventType.Repaint)
        {
            if (_targetMeshFilter == null || _targetMeshRenderer == null)
            {
                return;
            }

            // Draw the grid background before rendering the 3D preview
            DrawGridBackground(r);

            // Tell the PRU to prepare itself - we pass along the
            // rect of the preview area so the PRU knows what size 
            // of a preview to render.
            _previewRenderUtility.BeginPreview(r, background);

            // We draw our mesh manually - it is not attached to any 'gameobject' in the preview 'scene'.
            _previewRenderUtility.DrawMesh(_targetMeshFilter.sharedMesh, Matrix4x4.identity, _targetMeshRenderer.sharedMaterial, 0);
        
            // Tell the camera to render the preview.
            _previewRenderUtility.camera.Render();

            // End the preview and get the rendered texture.
            Texture resultRender = _previewRenderUtility.EndPreview();
        
            // Draw the rendered texture in the preview GUI area.
            GUI.DrawTexture(r, resultRender, ScaleMode.StretchToFill, false);
        }
    }
    private void DrawGridBackground(Rect area)
    {
        // Define grid spacing and color
        int gridSpacing = 20;
        Color gridColor = Color.yellow;

        // Create a temporary texture for the grid
        Texture2D gridTexture = new Texture2D(1, 1);
        gridTexture.SetPixel(0, 0, gridColor);
        gridTexture.Apply();

        // Calculate the number of vertical and horizontal lines based on the rect
        for (int x = 0; x < area.width; x += gridSpacing)
        {
            // Draw vertical grid lines
            GUI.DrawTexture(new Rect(area.x + x, area.y, 1, area.height), gridTexture);
        }

        for (int y = 0; y < area.height; y += gridSpacing)
        {
            // Draw horizontal grid lines
            GUI.DrawTexture(new Rect(area.x, area.y + y, area.width, 1), gridTexture);
        }
    
        // Clean up the texture to avoid memory leaks
        Object.DestroyImmediate(gridTexture);
    }
    
    void OnDestroy()
    {
        //Gotta clean up after yourself!
        if(_previewRenderUtility!=null)        _previewRenderUtility.Cleanup();
    }
}
