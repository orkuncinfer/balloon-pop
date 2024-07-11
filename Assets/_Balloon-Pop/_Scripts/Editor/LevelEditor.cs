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
    [FormerlySerializedAs("LevelDataAsset")] public WaveDataSO _waveDataAsset;
}

public enum BrushType
{
    Drop,
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
    public  List<ItemBaseDefinition> LevelDropTypes = new List<ItemBaseDefinition>();
    private HashSet<Vector2Int> clickedCells = new HashSet<Vector2Int>();

    public AllTileTypesSO _allAllTilesData;
    
    [FormerlySerializedAs("LevelDataAsset")] [SerializeField] [OnValueChanged("OnLevelDataAssetChanged")]
    private WaveDataSO _waveDataAsset;
    
    [ShowInInspector][HideLabel][ReadOnly]
    private string info = "Empty drop tiles will spawn randomly";
    [ShowInInspector][HideLabel][ReadOnly]
    private string info2 = "Right click to empty tile";
    [ShowInInspector][HideLabel][ReadOnly]
    private string info3 = "It can be initial match if you paint colored drop!!";
    [ValueDropdown("GetDropdownValues")][HorizontalGroup]
    public ItemBaseDefinition Brush;
    [HorizontalGroup]
    public BrushType BrushType;
    
    private  ItemBaseDefinition[] GetDropdownValues()
    {
        if (_allAllTilesData == null) return null;
        if (BrushType == BrushType.Drop)
        {
            ItemBaseDefinition[] array = new ItemBaseDefinition[_allAllTilesData.TileDropTypes.Count];
            for (int i = 0; i < _allAllTilesData.TileDropTypes.Count; i++)
            {
                array[i] = _allAllTilesData.TileDropTypes[i].MachItemRepKey;
            }
            return array;
        }
        else
        {
            return null;
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
            _waveDataAsset = AssetDatabase.LoadAssetAtPath<WaveDataSO>(assetPath);
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

    protected override void OnImGUI()
    {
        base.OnImGUI();
        if (GUI.changed && _waveDataAsset != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(_waveDataAsset);
            EditorPrefs.SetString("LevelDataAssetPath", assetPath);
        }
        if(_waveDataAsset == null) return;
        HandleMouseInput();
        DrawGrid();
    }
  
    private void SaveLevelToAsset()
    {
        if (_waveDataAsset == null) return;
        _waveDataAsset.BoardHeight = gridHeight;
        _waveDataAsset.BoardWidth = gridWidth;
        _waveDataAsset.LevelDropTypeKeys = LevelDropTypes;
        EditorUtility.SetDirty(_waveDataAsset);
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
                    _waveDataAsset.BoardDropsDictionary.Set(new Vector2Int(cellPosition.x, cellPosition.y), Brush);
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
                    _waveDataAsset.BoardDropsDictionary.Set(new Vector2Int(cellPosition.x, cellPosition.y), null);
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

                if (_waveDataAsset.BoardDropsDictionary.Contains(new Vector2Int(x, y)) == false)
                {
                    _waveDataAsset.BoardDropsDictionary.Add(new Vector2Int(x,y),null);
                }
               
            
                if (BrushType == BrushType.Drop)
                {
                    ItemBaseDefinition dropKey = _waveDataAsset.BoardDropsDictionary.Get(new Vector2Int(x, y));
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
                }
                
                
                Handles.DrawWireCube(cellRect.center, new Vector3(cellSize, cellSize, 0));
            }
        }

        Handles.EndGUI();
    }

    void OnLevelDataAssetChanged()
    {
        if (_waveDataAsset == null) return;
        FetchLevelDataAsset();
        
        for (int x = 0; x < _waveDataAsset.BoardWidth; x++)
        {
            for (int y = 0; y < _waveDataAsset.BoardHeight; y++)
            {
                if (_waveDataAsset.BoardDropsDictionary.Contains(new Vector2Int(x,y))) continue;
               _waveDataAsset.BoardDropsDictionary.Add(new Vector2Int(x,y),null);
            }
        }
        
    }
    void FetchLevelDataAsset()
    {
        if(_waveDataAsset == null) return;
        gridWidth = _waveDataAsset.BoardWidth;
        gridHeight = _waveDataAsset.BoardHeight;
        LevelDropTypes = _waveDataAsset.LevelDropTypeKeys;
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