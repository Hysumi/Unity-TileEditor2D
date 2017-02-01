using UnityEditor;
using UnityEngine;
//using System.Collections.Generic;
//using System.IO;

public class LoadFromAssets : Editor
{
   // public GameObject grid;
    public TileSet[] tileSet;
    public string[] tileSetNames;
   // public string[] pathFolderNames;
   // public List<string> pathFoldersList = new List<string>();

    /*  
      public void LoadGrid()
      {
          if (!Directory.Exists(Constants.resourcePath))
              EditorUtility.DisplayDialog(Constants.errorString, "Failed to load the grid because the folder 'Resource' does not exists in the 'Assets'.", Constants.okString);
          else if (!File.Exists(Constants.resourcePath + "Grid.prefab"))
              EditorUtility.DisplayDialog(Constants.errorString, "Failed to load the grid because the prefab 'Grid' does not exists in the 'Resources' folder. " +
                                                                 "Create a empty GameObject in the scene and put the 'Grid.cs' script on it. Click and drag the object to the 'Resources' folder and rename it to 'Grid'.", Constants.okString);
          else if (!grid)
              grid = Resources.Load<GameObject>("Grid");
      }
      */

    #region Pega todos os caminhos de pastas da Unity
    /*
    public void LoadPathFolders(string path)
    {
        string[] assetsFolders;

        assetsFolders = AssetDatabase.GetSubFolders(path);

        if (assetsFolders.Length == 0)
            return;
        for (int i = 0; i < assetsFolders.Length; i++)
        {
            pathFoldersList.Add(assetsFolders[i]);
            LoadPathFolders(assetsFolders[i]);
        }

        string[] _pathnames = new string[pathFoldersList.Count];
        int k = 0;

        foreach (string names in pathFoldersList)
        {
            _pathnames[k] = names;
            k++;
        }

        pathFolderNames = _pathnames;
        pathFoldersList.Clear();
    }
    */
    #endregion

    public void LoadTileSets()
    {
        tileSet = Resources.LoadAll<TileSet>(""); //Pega todos os TileSets na pasta Resources       
        tileSetNames = new string[tileSet.Length];
        
        #region Verifica se tem arquivos com nomes iguais e renomeia

        for (int i = 0; i < tileSet.Length; i++)
        {
            for (int j = 0; j < tileSet.Length; j++)
                if (tileSet[i].name == tileSet[j].name && j != i)
                    tileSet[j].name = tileSet[j].name + "NameAlreadyTaken" + Random.Range(1,1000000);
            tileSetNames[i] = tileSet[i].name;
        }

        #endregion
  
    }
}
