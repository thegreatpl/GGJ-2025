using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TileLayer
{
    Background, 
    Walls, 
    Foreground
}

public class MapController : MonoBehaviour
{



    public Tilemap BackGround;

    public Tilemap Walls;

    public Tilemap Foreground; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Walls.transform.position = Vector3.zero; //dirty hack to stop the jumping walls. 
    }


    public void SetTile(Vector3Int location, TileBase tile, TileLayer layer)
    {
        switch (layer)
        {

            case TileLayer.Background:
                BackGround.SetTile(location, tile);
                break;
            case TileLayer.Walls:
                Walls.SetTile(location, tile); break;
            case TileLayer.Foreground:
                Foreground.SetTile(location, tile);
                break;
        }
    }


    public Vector3 CellToWorld(Vector2Int cell)
    {
        return BackGround.CellToWorld((Vector3Int)cell); 
    }
}
