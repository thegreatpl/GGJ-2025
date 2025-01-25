using Codice.CM.Client.Differences;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StructureHandler : MonoBehaviour
{
    [MenuItem("MapGeneration/ClearMap")]
    public static void ClearMap()
    {
        var map = SceneAsset.FindAnyObjectByType<MapController>();
        map.Foreground.ClearAllTiles();
        map.BackGround.ClearAllTiles();
        map.Walls.ClearAllTiles();
        map.Detail.ClearAllTiles();
    }


    [MenuItem("MapGeneration/Save Structure")]
    public static void SaveStructure()
    {
        var structureDesign = SceneAsset.FindAnyObjectByType<StructureDesign>();


        var structure = new StructureDef();
        structure.name = structureDesign.StructureName;
        structure.Biomes = structureDesign.Biomes;
        structure.Type = structureDesign.Type.ToString();
        structure.prefabs = new System.Collections.Generic.List<StructurePrefab>(); 

        structure.tiles = new System.Collections.Generic.List<StructureTile>(); 

        var map = SceneAsset.FindAnyObjectByType<MapController>();


        var foreBounds = map.Foreground.cellBounds;
        var wallBounds = map.Walls.cellBounds;
        var backBounds = map.BackGround.cellBounds;
        var detailBounds = map.Detail.cellBounds;
        int minX, minY, maxX, maxY;
        minX = foreBounds.xMin;
        minY = foreBounds.yMin;
        maxX = foreBounds.xMax;
        maxY = foreBounds.yMax; 

        if (minX > wallBounds.xMin) minX = wallBounds.xMin;
        if (minX > backBounds.xMin) minX = backBounds.xMin;
        if (minX > detailBounds.xMin) minX = detailBounds.xMin;
        if (minY > wallBounds.yMin) minY = wallBounds.yMin;
        if (minY > backBounds.yMin) minY = backBounds.yMin;
        if (maxY < backBounds.yMax) maxY = backBounds.yMax;
        if (minY > detailBounds.yMin) minY = detailBounds.yMin;
        if (maxY < detailBounds.yMax) maxY = detailBounds.yMax;
        if (maxY < wallBounds.yMax) maxY = wallBounds.yMax;
        if (maxX < wallBounds.xMax) maxX = wallBounds.xMax;
        if (maxX < backBounds.xMax) maxX = backBounds.xMax;
        if (maxX < detailBounds.xMax) maxX = detailBounds.xMax;

        var bounds = new BoundsInt(minX, minY, foreBounds.zMin, maxX - minX, maxY - minY, foreBounds.zMax - foreBounds.zMin);

        var foregroundTiles = map.Foreground.GetTilesBlock(bounds);
        var wallTiles = map.Walls.GetTilesBlock(bounds);
        var backTiles = map.BackGround.GetTilesBlock(bounds);
        var detailTiles = map.Detail.GetTilesBlock(bounds);

        for (int xdx = 0; xdx < bounds.size.x; xdx++)
            for (int ydx = 0; ydx < bounds.size.y; ydx++)
            {
                var foregroundtile = foregroundTiles[xdx + ydx * bounds.size.x]; 
                var wallTile = wallTiles[xdx + ydx * bounds.size.x];
                var backTile = backTiles[xdx + ydx * bounds.size.x];
                var detailTile = detailTiles[xdx + ydx* bounds.size.x];

                if (foregroundtile != null)
                    structure.tiles.Add(new StructureTile()
                    {
                        x = xdx,
                        y = ydx,
                        tilename = foregroundtile.name, 
                        Layer = "Foreground"
                    });
                if (wallTile != null)
                    structure.tiles.Add(new StructureTile()
                    {
                        x = xdx,
                        y = ydx ,
                        tilename = wallTile.name, 
                        Layer = "Wall"
                    }); 
                if (backTile != null)
                    structure.tiles.Add(new StructureTile() { x = xdx, y = ydx , tilename = backTile.name, Layer = "Background" });

                if (detailTile != null)
                    structure.tiles.Add(new StructureTile()
                    {
                        x = xdx,
                        y = ydx,
                        tilename = detailTile.name,
                        Layer = "Detail"
                    });
            }

        structure.Size = (Vector2Int)bounds.size;

        var objects =  SceneManager.GetActiveScene().GetRootGameObjects();

        var baseloc = map.Foreground.CellToWorld(new Vector3Int(minX, minY)); 

            //SceneAsset.FindObjectsByType<GameObject>(FindObjectsSortMode.None); 
        foreach (var obj in objects)
        {
            if (obj.name == "Map")
                continue;

            structure.prefabs.Add(new StructurePrefab()
            {
                name = obj.name.Split(' ')[0],
                location = obj.transform.position - baseloc
            }); 


        }

        var json = EditorJsonUtility.ToJson(structure, true);

        File.WriteAllText($"Assets/Resources/Structures/{structure.name}.json", json);

        AssetDatabase.Refresh();
    }
}
