using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<MapTile> mapTiles;

    public GameObject mapTilePrefab;
    public int mapWidth;
    public int mapHeight;

    public MapData mapData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMap(int width, int height)
    {
        GameObject newObject;

        for (int z = 0; z<height; z++)
        {
            for (int x=0; x<width; x++)
            {
                //Create the game object, move it, and rename it
                newObject = GameObject.Instantiate(mapTilePrefab, this.transform);
                newObject.transform.position = new Vector3(x, 0, z);
                newObject.name = "MapTile (" + x + ", " + z + ")";

                //Initialize the map tile
                MapTile newTile = newObject.GetComponent<MapTile>();
                newTile.Initialize(x, z);
                mapTiles.Add(newTile);
            }
        }
    }

    public void ClearMap()
    {

        //Delete the existing map by destroying all children of the map manager
        //It is done in two steps due to the odd interaction of DestroyImmediate with iteration
        mapTiles.Clear();
        List<Transform> children = new List<Transform>();
        foreach (Transform child in this.transform)
        {
            children.Add(child);
        }
        foreach(Transform child in children)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public void SaveMap()
    {
        if(mapData == null)
        {
            Debug.LogError("Cannot save without a map file");
            return;
        }

        //Save map bounds
        mapData.height = mapHeight;
        mapData.width = mapWidth;

        //Save the heights of all the tiles
        mapData.tileHeights = new List<int>();
        mapTiles.ForEach(tile => mapData.tileHeights.Add(tile.tileHeight));

        //Actually save the asset file
        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();

    }

    public void LoadMap()
    {
        if (mapData == null)
        {
            Debug.LogError("Cannot load without a map file");
            return;
        }

        //Load map bounds
        this.mapWidth = mapData.width;
        this.mapHeight = mapData.height;

        //Clear the current map and create all the new objects
        ClearMap();
        GenerateMap(mapWidth, mapHeight);

        //Update each of the new tiles with the correct height
        for(int i = 0; i<mapData.tileHeights.Count; i++)
        {
            mapTiles[i].tileHeight = mapData.tileHeights[i];
            mapTiles[i].UpdateHeight();
        }
    }

}
