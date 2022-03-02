using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public int tileHeight;
    int _previousHeight;

    public Unit occupant;

    public GameObject tileHighlight;
    private float highlightOffset = .01f;

    public int x;
    public int z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public void UpdateHeight()
    {
        //Update the height of the model in the scene
        this.transform.localScale = new Vector3(1, tileHeight, 1);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, (tileHeight / 2f)-.5f, this.transform.localPosition.z);

        //Move the highlight to the correct position
        //tileHighlight.transform.position = GetSurfacePosition() + Vector3.up * highlightOffset;
    }

    private void OnValidate()
    {
        //check if the tile height changed
        if(tileHeight != _previousHeight)
        {
            _previousHeight = tileHeight;
            UpdateHeight();
        }
    }

    private void OnMouseOver()
    {
        InputManager.instance.TileHovered(this);
    }

    private void OnMouseDown()
    {
        InputManager.instance.TileClicked(this);
    }

    /// <summary>
    /// Calculates a Vector3 position that would be 'on top' of this map tile. 
    /// Used for positioning cursor, units, and other GameObjects that occupy tiles.
    /// </summary>
    /// <returns>Returns the position on 'top' of this tile</returns>
    public Vector3 GetSurfacePosition()
    {
        //The position should be half the tile's height above it's center point
        Vector3 position = this.transform.position + (Vector3.up * this.transform.localScale.y / 2f);
        return position;
    }

    /// <summary>
    /// Checks whether or not anything is currently occupying this tile (units, objects, etc)
    /// </summary>
    /// <returns>Returns whether this tile is occupied or not</returns>
    public bool IsOccupied()
    {
        return (occupant != null);
    }

    public void SetHighlight(Color color)
    {
        tileHighlight.GetComponent<MeshRenderer>().material.color = color;
        tileHighlight.SetActive(true);
    }

    public void ClearHighlight()
    {
        tileHighlight.SetActive(false);
    }

    public List<MapTile> GetNeighbors()
    {
        return MapManager.instance.GetTilesInRange(x, z, 1, 1);
    }

    public float GetDistance(MapTile otherTile)
    {
        int xDist = Mathf.Abs(otherTile.x - this.x);
        int zDist = Mathf.Abs(otherTile.z - this.z);

        return Mathf.Sqrt(xDist * xDist + zDist * zDist);
    }
}
