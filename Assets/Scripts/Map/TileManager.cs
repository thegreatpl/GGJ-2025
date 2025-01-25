using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public int LoadTileThreshold = 20; 

    public Dictionary<string, TileBase> Tiles = new Dictionary<string, TileBase>();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public IEnumerator LoadTiles()
    {
        var tiles = Resources.LoadAll("Tilemaps", typeof(Sprite)) as Sprite[];
        Tiles = new Dictionary<string, TileBase>();

        int idx = 0;

        foreach (var tile in tiles)
        {

            var tileobj = ScriptableObject.CreateInstance<Tile>(); 
            tileobj.name = tile.name;
            tileobj.sprite = tile;

            Tiles.Add(tile.name, tileobj);

            if (idx >= LoadTileThreshold)
            {
                yield return null;
                idx = 0;
            }

            idx++;
        }
    }

    public TileBase GetTile(string name)
    {
        if (Tiles.ContainsKey(name))
            return Tiles[name];
        return null;
    }

}
