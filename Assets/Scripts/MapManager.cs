using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<MapTile> mapTiles;

    public GameObject mapTilePrefab;
    public int mapWidth;
    public int mapHeight;

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


                MapTile newTile = newObject.GetComponent<MapTile>();
                mapTiles.Add(newTile);
            }
        }
    }

    public void ClearMap()
    {

        //Delete the existing map by destroying all children of the map manager
        //It is done in two steps due to the odd interaction of DestroyImmediate with iteration
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


}
