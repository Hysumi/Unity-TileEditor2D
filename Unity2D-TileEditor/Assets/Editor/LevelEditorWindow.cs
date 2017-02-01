using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class LevelEditorWindow : EditorWindow
{
    const int scrollDifference = 185;

    #region Layers and Tags Variables

    string[] //backgroundLayerNames = new string[Constants.maxBackgroundLayerLenght], 
             allLayerNames = new string[Constants.maxLayerLenght],
             editableLayerNames = new string[Constants.editableLayerLenght],
             //frontLayerNames = new string[Constants.maxFrontLayerLenght],
             tagNames = new string[Constants.maxTagLenght];
             //layerTab = new string[] { "Back Layer", "Main Layer", "Front Layer"};

    public string ln = "", ln2 = "", tn = "", tn2 = "";
    public int currentTab, indexLayer = 0,// indexF = 0, 
               indexLayers = 0, indexT = 0, indexTag = 0;
              
    public Vector2 maxScroll;
    public LayersAndTags layersandtags;

    #endregion

    bool isScrollView = false, isAdd = false;
    public Rect windowRect;
    Vector2 scrollPos, mousePos;
    Tool lastTool = Tool.None;

    #region Grid Variables

    //public GameObject grid;
    //public Grid g;
    //public GridEditor gridEditor;

    #endregion

    #region Tile Variables

    public LoadFromAssets loadFromAssets;
    public int indexTileSet, indexTileList;
    public bool tileList = true, tileIcons, sameTileLayer;
    GameObject _gTile;
    GameObject[] tileHit;
    public RaycastHit2D[] layerRay;

    #endregion

    #region Input Variables

    bool mouseDownRight = false, mouseDownLeft = false;

    #endregion

    #region Gizmos Variables

    public GameObject gTile;
    bool isTileSelected = false;

    #endregion

    void Update()
    {
        if (isTileSelected)
        {
            Tools.current = Tool.None;

            if(gTile)
                gTile.transform.position = new Vector2(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));


            //EditorApplication.MarkSceneDirty();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        else
            Tools.current = lastTool;

        
    }

    void OnInspectorUpdate()
    {
        if (!loadFromAssets)
            loadFromAssets = ScriptableObject.CreateInstance<LoadFromAssets>();

        //loadFromAssets.LoadGrid();
        //loadFromAssets.LoadPathFolders("Assets/Resources");
        loadFromAssets.LoadTileSets();

        if (isTileSelected)
        {
            if (!gTile)
            {
                gTile = (GameObject)Instantiate(_gTile);
                gTile.transform.name = "tile";
                SpriteRenderer[] tempTile;
                tempTile = gTile.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer sRen in tempTile)
                {
                    sRen.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4f); //40% transparent
                }
                //gTile.hideFlags = HideFlags.HideInHierarchy;
            }
        }
        else
            DestroyImmediate(GameObject.Find("tile"));
    }

    void OnEnable()
    {
        #region Enable Scene View

        SceneView.onSceneGUIDelegate += SceneGUI;
        lastTool = Tools.current;

        #endregion

        if (!loadFromAssets)
            loadFromAssets = ScriptableObject.CreateInstance<LoadFromAssets>();

        //loadFromAssets.LoadGrid();
        loadFromAssets.LoadTileSets();

        if (!layersandtags)
            layersandtags = ScriptableObject.CreateInstance<LayersAndTags>();
        allLayerNames = layersandtags.editLayers(allLayerNames, ln2, indexLayers);
        tagNames = layersandtags.getTags();

        /* if (!grid && !GameObject.Find("Grid(Clone)"))
         {
            // grid = Instantiate(loadFromAssets.grid);
             g = grid.GetComponent<Grid>();
         }
         else if (!grid)
         {
             grid = GameObject.Find("Grid(Clone)");
             g = grid.GetComponent<Grid>();
         }*/
    }
    /*
    void OnHierarchyChange()
    {
        if (!GameObject.Find("Grid(Clone)") && Application.isEditor && !Application.isPlaying)
        {
            //loadFromAssets.LoadGrid();
           // grid = Instantiate(loadFromAssets.grid);
            g = grid.GetComponent<Grid>();
        }
    }
    */
    void SceneGUI(SceneView sceneView)
    {
        #region Setup Mouse Coordinates

        Event e = Event.current;
        Ray worldRays = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        mousePos = worldRays.origin;

        #endregion

        #region User Input on Scene

        if (gTile != null)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (e.type == EventType.layout)
            {
                HandleUtility.AddDefaultControl(controlID);
            }
            switch (e.type)
            {
                case EventType.mouseDown:
                    if (e.button == 0)
                    {
                        mouseDownLeft = true;

                        ObtainTileBelow();

                        //if(tileHit)
                       //     Debug.Log(tileHit.name);
                        //if (tileHit != null)
                        //    CheckTileLayer(tileHit, sameTileLayer);
                        //else if (tileHit == null)
                        //    isTileSelected = false;
                    }
                    if (e.button == 1) 
                    {
                        mouseDownRight = true;
                    }
                    break;
                case EventType.mouseUp:
                    if (e.button == 0) //LEFT CLICK UP
                        mouseDownLeft = false;
                    if (e.button == 1) //RIGHT CLICK UP
                        mouseDownRight = false;
                    break;
                case EventType.keyDown:
                    if (e.keyCode == KeyCode.Escape)
                        isTileSelected = false;
                    Event.current.Use();    // if you don't use the event, the default action will still take place.
                    break;
            }
        }
        #endregion

        #region Mouse Click Actions
        /*

        if (mouseDown)
        {
            if (eraseTool && !holdingControl)
            {
                removingTile = true;
            }
            else
            {
                //Add Tile On Mouse Down
                ObtainTileBelow();
                CheckTile(lowerTileHit, middleTileHit, aboveTileHit, currentTabList[tileSelectedID].transform.FindChild("Editor Hitbox").gameObject.layer);
                removingTile = false;
            }
        }
        else
        {
            if (eraseTool)
            {
                removingTile = false;
            }
            dragDirection = 0;
        }
        if (removingTile)
        {
            ObtainTileBelow();
            if (lowerTileHit != null || middleTileHit != null || aboveTileHit != null)
            {
                CheckTile(lowerTileHit, middleTileHit, aboveTileHit, -1);
            }
        }
        */
        #endregion
    }

    void OnGUI()
    {
        windowRect = new Rect(this.position.x, this.position.y, this.position.width, this.position.height); //Pega um Rect do tamanho da janela

        scrollPos = GUI.BeginScrollView(new Rect(0, 0, windowRect.width, windowRect.height - 10),
                   scrollPos, new Rect(0, 0, maxScroll.x, maxScroll.y), false, false);

        Event e = Event.current;

        int _scrollDifference;

        if (isScrollView)
            _scrollDifference = scrollDifference - 10;
        else
            _scrollDifference = scrollDifference;

        #region Layer Options

        for (int i = 0; i < editableLayerNames.Length; i++) //Preenche o vedor de Layers Editáveis
            editableLayerNames[i] = allLayerNames[i + 8];

        GUI.Box(new Rect(0, 0, windowRect.width, windowRect.height), "");
        GUI.Label(new Rect(5, 3, 200, 200), "Layer Options", EditorStyles.boldLabel);
        GUI.Label(new Rect(5, 20, 200, 200), "Select Layer: ");
        indexLayer = EditorGUI.Popup(new Rect(180, 19, windowRect.width - _scrollDifference, 15), indexLayer, editableLayerNames);
        GUI.Label(new Rect(5, 38, 200, 200), "Edit Layer Name: ");
        ln = allLayerNames[indexLayer + 8];
        ln2 = EditorGUI.TextField(new Rect(180, 38, windowRect.width - _scrollDifference, 15), ln);
        indexLayers = 8 + indexLayer;
        allLayerNames = layersandtags.editLayers(allLayerNames, ln2, indexLayers);

        #endregion

        #region Tag Options

        GUI.Label(new Rect(5, 65, 200, 200), "Tag Options", EditorStyles.boldLabel);
        GUI.Label(new Rect(5, 82, 200, 200), "Select Tag: ");
        indexT = EditorGUI.Popup(new Rect(180, 81, windowRect.width - _scrollDifference, 15), indexT, tagNames);
        GUI.Label(new Rect(5, 100, 200, 200), "Add/Edit/Remove Tag: ");
        tn = tn2;
        tn2 = EditorGUI.TextField(new Rect(180, 100, windowRect.width - _scrollDifference, 15), tn);

        if (GUI.Button(new Rect(5, 118, (windowRect.width) / 3 - 17, 20), "Add"))
            tagNames = layersandtags.addTags(tn2);
        if (GUI.Button(new Rect(106 + ((windowRect.width - Constants.minWindowWidth) / 3), 118, (windowRect.width) / 3 - 17, 20), "Edit"))
            tagNames = layersandtags.editTag(tagNames, tn2, indexT);
        if (GUI.Button(new Rect(205 + (2 * (windowRect.width - Constants.minWindowWidth) / 3), 118, (windowRect.width) / 3 - 17, 20), "Remove"))
            tagNames = layersandtags.removeTag(tn2);

        #endregion

        #region Tile Set Options

        GUI.Label(new Rect(5, 153, 200, 200), "Tile Set Options", EditorStyles.boldLabel);
        GUI.Label(new Rect(5, 170, 200, 200), "Select Tile Set: ");
        indexTileSet = EditorGUI.Popup(new Rect(180, 169, windowRect.width - _scrollDifference, 15), indexTileSet, loadFromAssets.tileSetNames);

        string[] prefabNames = new string[loadFromAssets.tileSet[indexTileSet].prefabs.Length];

        TileSet ts = loadFromAssets.tileSet[indexTileSet];

        GUI.Label(new Rect(5, 187, 200, 200), "View: ");

        tileList = GUI.Toggle(new Rect(180, 186, 40, 15), tileList, "List");
        if (tileList == tileIcons)
        {
            if (!tileList)
                tileList = true;
            tileIcons = !tileList;
        }

        tileIcons = GUI.Toggle(new Rect(233, 186, 55, 15), tileIcons, "Icons");
        if (tileList == tileIcons)
        {
            if (!tileIcons)
                tileIcons = true;
            tileList = !tileIcons;
        }

        GUI.Label(new Rect(5, 204, 100, 15), "Tiles: ");

        if (tileList)
        {

            if (indexTileList > prefabNames.Length)
                indexTileList = prefabNames.Length;

            GetPrefabNamesInTileSet(indexTileSet, ref prefabNames);
            int _indexTileList = indexTileList;

            indexTileList = EditorGUI.Popup(new Rect(180, 204, windowRect.width - _scrollDifference, 15), indexTileList, prefabNames);

            if (indexTileList != _indexTileList)
                isTileSelected = false;
            else if (ts.prefabs.Length != 0)
            {
                _gTile = ts.prefabs[indexTileList];
                isTileSelected = true;
            }

            if (EditorWindow.focusedWindow != this && EditorWindow.focusedWindow != EditorWindow.GetWindow<SceneView>())
                isTileSelected = false;

            switch (e.type)
            {
                case EventType.keyDown:
                     {
                        if (e.keyCode == KeyCode.Escape) 
                            isTileSelected = false;
                        e.Use();    
                    }
                    break;
            }
        }
        else
        {

        }

        #endregion

        maxScroll = new Vector2(270, 357); //MUDAR ISSO AQUI DEPOIS

        GUI.EndScrollView();

        #region DESCARTADO

        #region Abas 

        //currentTab = GUILayout.Toolbar(currentTab, layerTab); //Cria a toolbar com as 3 abas


        //switch (currentTab)
        //{
        //    case 0:

        #endregion

        // LayersOptions(ref indexLayer, editableLayerNames, 8,_scrollDifference);

        //        break;
        //    case 1:
        //        GUI.Box(new Rect(0, 0, windowRect.width, 119), "");
        //        GUI.Label(new Rect(5, 5, 200, 200), "Main Layer Selected", EditorStyles.boldLabel);
        //        GUI.TextField(new Rect(180, 7, 100, 15), "Main Layer");
        //        break;
        //    case 2:
        //        LayersOptions("Front", ref indexF, frontLayerNames, 21);
        //        break;
        //}

        // if (currentTab == 1) //Se for Main Layer
        //     TagsAndResetOptions(0);
        // else
        // TagsOptions(_scrollDifference);


        #region Grid Configuration
        /*
        if (currentTab == 1) //MainLayer
            GridOptions(0);
        else
            GridOptions(Constants.mainLayerHeightDifference);
        */
        #endregion

        #region Tile Set Configuration

        // if (currentTab == 1)
        //     TileSetOptions(0);
        // else
        //TileSetOptions(_scrollDifference);

        #endregion

        #endregion

    }

    void GetPrefabNamesInTileSet(int index, ref string[] prefabNames)
    {
        for (int i = 0; i < prefabNames.Length; i++)
        {
            if (loadFromAssets.tileSet[index].prefabs[i])
                prefabNames[i] = loadFromAssets.tileSet[index].prefabs[i].name;
            else
                prefabNames[i] = "Null " + i;
        }
    }

    void ObtainTileBelow()
    {
        //var lowerLayerBitMask = 1 << (int)indexLayer + 8; //BitShift into Layer needed to detect.
        //var lowerLayerBitMask = (int)indexLayer + 8;
        //if (gTile != null)
           // layerRay = Physics2D.RaycastAll(gTile.transform.position,);
        if (layerRay.Length > 1)
        {
            GameObject[] _tileHit = new GameObject[layerRay.Length - 1];
            for (int i = 0; i < _tileHit.Length; i++)
            {
                if (layerRay[i + 1].collider && layerRay[i + 1].collider.gameObject.layer == indexLayer + 8) //Verifica se o hit é no mesmo Layer
                {
                    _tileHit[i] = layerRay[i + 1].collider.gameObject;
                    Debug.Log("entrou: " + _tileHit[i].name);
                }
            }

            for (int i = 0; i < _tileHit.Length; i++) //Ordena para deixar os null no final
            {
                if (!_tileHit[i])
                    for (int j = i; j < _tileHit.Length; j++)
                    {
                        if (_tileHit[j])
                        {
                            _tileHit[i] = _tileHit[j];
                            _tileHit[j] = null;
                            break;
                        }
                    }
            }
            tileHit = _tileHit;
            sameTileLayer = true;
        }
        else
        {
            tileHit = null;
            sameTileLayer = false;
        }       
    }

    void CheckTileLayer(GameObject tile, bool isSameLayer)
    {
        if (isSameLayer)
        {
            if (tile != null)
                DestroyImmediate(tile);

            if (!isAdd)
            {
                ObtainTileBelow();
                AddTile();
            }
        }
    }

    void AddTile()
    {
        #region Add Tile to scene
        /*
        if (!isAdd)
        {
            isAdd = true;
            if (tileHit.layer == indexLayer + 8)
                sameTileLayer = true;
            else
                sameTileLayer = false;

            CheckTileLayer(tileHit, sameTileLayer);
        }

        GameObject metaTile = (GameObject)Instantiate(_gTile);
        metaTile.transform.position = gTile.transform.position;
        metaTile.layer = indexLayer + 8;
        metaTile.tag = tagNames[indexTag];
        //metaTile.gameObject.AddComponent<EditorHitbox>();

        if (autoGroup)
        {
            metaTile.transform.parent = GameObject.Find("CUSTOM GROUP EDIT").transform;
        }
        //metaTile.transform.FindChild("Editor Hitbox").gameObject.SetActive(true);
        isAdd = false;
        */
        #endregion
    }

    #region Funções Descartadas

    /*
    #region Preenche os Background e Front Layers


    void FillLayers()
    {
        //for (int i = 0; i < backgroundLayerNames.Length; i++)
        //    backgroundLayerNames[i] = layerNames[i + 8];
        //for (int i = 0; i < frontLayerNames.Length; i++)
        //    frontLayerNames[i] = layerNames[i + 21];
        for (int i = 0; i < editableLayerNames.Length; i++)
            editableLayerNames[i] = allLayerNames[i + 8];
        
    }
    

    #endregion

    #region Layers

    void LayersOptions(ref int index, string[] names, int pos, int scrollDif)
    {
        FillLayers();

        GUI.Box(new Rect(0, 0, windowRect.width, windowRect.height), "");
        GUI.Label(new Rect(5, 3, 200, 200), "Layer Options", EditorStyles.boldLabel);
        GUI.Label(new Rect(5, 20, 200, 200), "Select " + " Layer: ");
        index = EditorGUI.Popup(new Rect(180, 19, windowRect.width-scrollDif, 15), index, names);
        GUI.Label(new Rect(5, 38, 200, 200), "Edit " + " Layer Name: ");
        ln = allLayerNames[index + pos];
        ln2 = EditorGUI.TextField(new Rect(180, 38, windowRect.width-scrollDif, 15), ln);
        indexLayers = pos + index;
        allLayerNames = layersandtags.editLayers(allLayerNames, ln2, indexLayers);
    }

    #endregion

    #region Tags and Reset
    
     
    void TagsOptions(int scrollDif)
    {
        GUI.Label(new Rect(5, 65, 200, 200), "Tag Options", EditorStyles.boldLabel);
        GUI.Label(new Rect(5, 82, 200, 200), "Select Tag: ");
        indexT = EditorGUI.Popup(new Rect(180, 81, windowRect.width - scrollDif, 15), indexT, tagNames);
        GUI.Label(new Rect(5, 100, 200, 200), "Add/Edit/Remove Tag: ");
        tn = tn2;
        tn2 = EditorGUI.TextField(new Rect(180, 100, windowRect.width - scrollDif, 15), tn);

        if (GUI.Button(new Rect(5, 118, (windowRect.width)/3 - 17, 20), "Add"))
            tagNames = layersandtags.addTags(tn2);

        if (GUI.Button(new Rect(106 + ((windowRect.width - Constants.minWindowWidth) / 3) , 118, (windowRect.width) / 3 - 17, 20), "Edit"))
            tagNames = layersandtags.editTag(tagNames, tn2, indexT);
        if (GUI.Button(new Rect(205 + (2*(windowRect.width - Constants.minWindowWidth) / 3), 118, (windowRect.width) / 3 - 17, 20), "Remove"))
            tagNames = layersandtags.removeTag(tn2);

        //if (GUI.Button(new Rect(13, 92 + height, 260, 20), "Reset Tags and Layers"))
        //{
        //    bool resposta = EditorUtility.DisplayDialog(Constants.warningString, "Do you really want to delete all of your layers and tags?", "Just do it!", "OH GOD, NO!");
        //    if (resposta)
        //        layersandtags.resetLayersAndTags();
        //}
    }

    #endregion


    #region Grid
    /*
    void GridOptions(float height)
    {
        GUI.Box(new Rect(0, 122 + height, windowRect.width, 89), "");
        GUI.Label(new Rect(5, 127 + height, 200, 200), "Grid Options", EditorStyles.boldLabel);

        #region Sliders Width Height

        GUI.Label(new Rect(5, 147 + height, 200, 200), "Grid Width: ");
        g.width = EditorGUI.Slider(new Rect(80, 147 + height, 200, 17), g.width, 1f, 100f);
        GUI.Label(new Rect(5, 167 + height, 200, 200), "Grid Height: ");
        g.height = EditorGUI.Slider(new Rect(80, 167 + height, 200, 17), g.height, 1f, 100f);

        #endregion

        #region Grid Color

        GUI.Label(new Rect(5, 187 + height, 200, 200), "Grid Color: ");
        g.color = EditorGUI.ColorField(new Rect(180, 187 + height, 100, 17), g.color);

        #endregion

    }
    
    #endregion

    #region Tile Set

    void TileSetOptions(int scrollDif)
    {
        //GUI.Box(new Rect(0, 130+ height, windowRect.width, windowRect.height), "");
        GUI.Label(new Rect(5, 153, 200, 200), "Tile Set Options", EditorStyles.boldLabel);
        GUI.Label(new Rect(5, 170, 200, 200), "Select Tile Set: ");
        indexTileSet = EditorGUI.Popup(new Rect(180, 169, windowRect.width - scrollDif, 15), indexTileSet, loadFromAssets.tileSetNames);
        string[] prefabNames = new string[loadFromAssets.tileSet[indexTileSet].prefabs.Length];
        GetPrefabNamesInTileSet(indexTileSet, ref prefabNames);
        GUI.Label(new Rect(5, 187, 200, 200), "View: ");

        tileList = GUI.Toggle(new Rect(180, 186, 40, 15), tileList, "List");
        if (tileList == tileIcons)
        {
            if (!tileList)
                tileList = true;
            tileIcons = !tileList;
        }

        tileIcons = GUI.Toggle(new Rect(233, 186, 55, 15), tileIcons, "Icons");
        if (tileList == tileIcons)
        {
            if (!tileIcons)
                tileIcons = true;
            tileList = !tileIcons;
        }

        GUI.Label(new Rect(5, 204, 100, 15), "Tiles: ");
        if(tileList)
        {
            indexTileList = EditorGUI.Popup(new Rect(180, 204, windowRect.width - scrollDif, 15), indexTileList, prefabNames);
        }
        else
        {

        }
    }
    #endregion
    
    */

    #endregion

}
