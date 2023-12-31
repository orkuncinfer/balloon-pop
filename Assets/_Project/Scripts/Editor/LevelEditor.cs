using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LevelAssetReference
{
    public LevelDataSO LevelDataAsset;
}

public enum BrushType
{
    Drop,
    Cell
}
public class LevelEditor : OdinEditorWindow
{
    private static int _screenWidth = 450;
    private static int _screenHeight = 700;
   
    private float cellSize = 50;
    [HorizontalGroup("Row1")]
    public int gridWidth = 8;
    [HorizontalGroup("Row1")]
    public int gridHeight = 8;
    [ListDrawerSettings(NumberOfItemsPerPage = 4,DefaultExpandedState = true)]
    public  List<GenericKey> LevelDropTypes = new List<GenericKey>();
    private HashSet<Vector2Int> clickedCells = new HashSet<Vector2Int>();

    public AllTileTypesSO _allAllTilesData;
    
    [SerializeField] [OnValueChanged("OnLevelDataAssetChanged")]
    private LevelDataSO LevelDataAsset;
    
    [ShowInInspector][HideLabel][ReadOnly]
    private string info = "Empty drop tiles will spawn randomly";
    [ShowInInspector][HideLabel][ReadOnly]
    private string info2 = "Right click to empty tile";
    [ShowInInspector][HideLabel][ReadOnly]
    private string info3 = "It can be initial match if you paint colored drop!!";
    [ValueDropdown("GetDropdownValues")][HorizontalGroup]
    public GenericKey Brush;
    [HorizontalGroup]
    public BrushType BrushType;
    
    private  GenericKey[] GetDropdownValues()
    {
        if (BrushType == BrushType.Drop)
        {
            GenericKey[] array = new GenericKey[_allAllTilesData.TileDropTypes.Count];
            for (int i = 0; i < _allAllTilesData.TileDropTypes.Count; i++)
            {
                array[i] = _allAllTilesData.TileDropTypes[i].MachItemRepKey;
            }
            return array;
        }
        else
        {
            GenericKey[] array = new GenericKey[_allAllTilesData.TileCellTypes.Count];
            for (int i = 0; i < _allAllTilesData.TileCellTypes.Count; i++)
            {
                array[i] = _allAllTilesData.TileCellTypes[i].MachItemRepKey;
            }
            return array;
        }
       
    }
    [MenuItem("LevelEditor/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>().Show();
        LevelEditor window = GetWindow<LevelEditor>("Level Editor");
        window.minSize = new Vector2(_screenWidth, _screenHeight);
        window.maxSize = window.minSize;
    }

    private void OnEnable()
    {
        string assetPath = EditorPrefs.GetString("LevelDataAssetPath", "");
        if (!string.IsNullOrEmpty(assetPath))
        {
            LevelDataAsset = AssetDatabase.LoadAssetAtPath<LevelDataSO>(assetPath);
        }

        if (_allAllTilesData == null)
        {
            _allAllTilesData = FindAssetByType<AllTileTypesSO>();
        }
        
        FetchLevelDataAsset();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SaveLevelToAsset();
    }

    
    void OnGUI()
    {
        base.OnImGUI();
        
        if (GUI.changed && LevelDataAsset != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(LevelDataAsset);
            EditorPrefs.SetString("LevelDataAssetPath", assetPath);
        }
        if(LevelDataAsset == null) return;
        HandleMouseInput();
        DrawGrid();
        
    }
  
    private void SaveLevelToAsset()
    {
        if (LevelDataAsset == null) return;
        LevelDataAsset.BoardHeight = gridHeight;
        LevelDataAsset.BoardWidth = gridWidth;
        LevelDataAsset.LevelDropTypeKeys = LevelDropTypes;
        EditorUtility.SetDirty(LevelDataAsset);
    }
    void HandleMouseInput()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Vector2Int cellPosition = GetCellPosition(mousePosition);

            if (cellPosition.x >= 0 && cellPosition.x < gridWidth &&
                cellPosition.y >= 0 && cellPosition.y < gridHeight)
            {
                if (BrushType == BrushType.Drop)
                {
                    LevelDataAsset.BoardDropsDictionary.Set(new Vector2Int(cellPosition.x, cellPosition.y), Brush);
                }
                else if (BrushType == BrushType.Cell)
                {
                    LevelDataAsset.BoardCellsDictionary.Set(new Vector2Int(cellPosition.x, cellPosition.y), Brush);
                }
                
                Repaint();
            }
        }
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Vector2Int cellPosition = GetCellPosition(mousePosition);

            if (cellPosition.x >= 0 && cellPosition.x < gridWidth &&
                cellPosition.y >= 0 && cellPosition.y < gridHeight)
            {
                if (BrushType == BrushType.Drop)
                {
                    LevelDataAsset.BoardDropsDictionary.Set(new Vector2Int(cellPosition.x, cellPosition.y), null);
                }
                else if (BrushType == BrushType.Cell)
                {
                    GenericKey defaultCell = _allAllTilesData.TileCellTypes[0].MachItemRepKey;
                    LevelDataAsset.BoardCellsDictionary.Set(new Vector2Int(cellPosition.x, cellPosition.y), defaultCell);
                }
                
                Repaint();
            }
        }
    }

    Vector2Int GetCellPosition(Vector2 mousePosition)
    {
           float windowWidth = position.width;
                float difference = (windowWidth - (cellSize * gridWidth)) / 2;
        int x = Mathf.FloorToInt((mousePosition.x - difference) / cellSize);
        int y = Mathf.FloorToInt((position.height - mousePosition.y) / cellSize);
        return new Vector2Int(x, y);
    }
    void DrawGrid()
    {
        Handles.BeginGUI();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float windowWidth = position.width;
                float difference = windowWidth - (cellSize * gridWidth);
                Rect cellRect = new Rect(x * cellSize + difference / 2, position.height - (y + 1) * cellSize - 5, cellSize, cellSize);

                if (LevelDataAsset.BoardDropsDictionary.Contains(new Vector2Int(x, y)) == false)
                {
                    LevelDataAsset.BoardDropsDictionary.Add(new Vector2Int(x,y),null);
                }
                if (LevelDataAsset.BoardCellsDictionary.Contains(new Vector2Int(x, y)) == false)
                {
                    GenericKey defaultCell = _allAllTilesData.TileCellTypes[0].MachItemRepKey;
                    LevelDataAsset.BoardCellsDictionary.Add(new Vector2Int(x,y),defaultCell);
                    
                }
               
            
                if (BrushType == BrushType.Drop)
                {
                    GenericKey dropKey = LevelDataAsset.BoardDropsDictionary.Get(new Vector2Int(x, y));
                    if (dropKey != null)
                    {
                        Sprite refSprite = _allAllTilesData.GetTileDropType(dropKey)?.Visual;
                        Texture2D spriteTexture = refSprite ? refSprite.texture : null;
                        Rect spriteRect = refSprite ? refSprite.textureRect : new Rect();
                        if (spriteTexture)
                        {
                            GUI.DrawTextureWithTexCoords(cellRect, spriteTexture, new Rect(spriteRect.x / spriteTexture.width, spriteRect.y / spriteTexture.height, spriteRect.width / spriteTexture.width, spriteRect.height / spriteTexture.height));
                        }
                    }
                }else if (BrushType == BrushType.Cell)
                {
                    GenericKey cellKey = LevelDataAsset.BoardCellsDictionary.Get(new Vector2Int(x, y));
                    if (cellKey != null)
                    {
                        Sprite refSprite = _allAllTilesData.GetTileCellType(cellKey)?.Visual;
                        Texture2D spriteTexture = refSprite ? refSprite.texture : null;
                        Rect spriteRect = refSprite ? refSprite.textureRect : new Rect();
                        if (spriteTexture)
                        {
                            GUI.DrawTextureWithTexCoords(cellRect, spriteTexture, new Rect(spriteRect.x / spriteTexture.width, spriteRect.y / spriteTexture.height, spriteRect.width / spriteTexture.width, spriteRect.height / spriteTexture.height));
                        }
                    }
                }
                
                
                Handles.DrawWireCube(cellRect.center, new Vector3(cellSize, cellSize, 0));
            }
        }

        Handles.EndGUI();
    }

    void OnLevelDataAssetChanged()
    {
        if (LevelDataAsset == null) return;
        FetchLevelDataAsset();
        
        for (int x = 0; x < LevelDataAsset.BoardWidth; x++)
        {
            for (int y = 0; y < LevelDataAsset.BoardHeight; y++)
            {
                if (LevelDataAsset.BoardDropsDictionary.Contains(new Vector2Int(x,y))) continue;
               LevelDataAsset.BoardDropsDictionary.Add(new Vector2Int(x,y),null);
            }
        }
        
    }
    void FetchLevelDataAsset()
    {
        if(LevelDataAsset == null) return;
        gridWidth = LevelDataAsset.BoardWidth;
        gridHeight = LevelDataAsset.BoardHeight;
        LevelDropTypes = LevelDataAsset.LevelDropTypeKeys;
    }
    
    public static T FindAssetByType<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); 
        if (guids.Length == 0)
        {
            Debug.LogWarning("No ScriptableObject of type " + typeof(T).Name + " found.");
            return null;
        }

        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]); 
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath); 
        return asset;
    }
}