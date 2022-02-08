using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public int tileHeight;
    int _previousHeight;

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
}
