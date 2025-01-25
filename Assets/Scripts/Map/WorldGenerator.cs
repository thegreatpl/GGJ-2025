using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;
using Random = System.Random; 

public class WorldGenerator : MonoBehaviour
{

   // Random Random; 

    public MapController map;

    public TileManager tileManager;

    public PrefabManager PrefabManager; 

    public TileBase BubbleWallTile;

    public string BubbleWallTileName; 

    public List<BiomeDef> BiomeDefs;

    public int Seed; 

    public int SectorSize = 100;

    public int MaxBubblesPerSector = 5;

    public int MinBubbleSize = 10; 

    public int MaxBubbleSize = 100;


    public Dictionary<Vector2Int, Sector> Sectors;


    private bool generated = false; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PrefabManager = GetComponent<PrefabManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!generated)
            return;

        var playerloc = GameManager.instance.Player.transform.position; 

        var sector = GetSector((int)playerloc.x / SectorSize, (int)playerloc.y / SectorSize);
        if (sector.Level < 6)
            StartCoroutine(RaiseToLevel6(sector));
    }


    public IEnumerator GenerateWorld(int seed = 0)
    {
        if (seed == 0)
        {
            Seed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        else
        {
            Seed = seed; 
        }

        
        BubbleWallTile = tileManager.GetTile(BubbleWallTileName);
        Sectors = new Dictionary<Vector2Int, Sector>();
        //load the biome definitions. 
        foreach (var def in BiomeDefs)
        {
            var deftiles = new List<TileBase>();

            foreach (var tile in def.Tilenames)
                deftiles.Add(tileManager.GetTile(tile));


            def.Tiles = deftiles; 

        }
        yield return null;
        yield return StartCoroutine(RaiseToLevel6(GetSector(0, 0))); 
        yield return null;

        generated = true; 
    }


    Sector GetSector(int x, int y)
    {
        if (Sectors.ContainsKey(new Vector2Int(x, y)))
        {
            return Sectors[new Vector2Int(x, y)];
        }

        var sector = new Sector(x, y, Seed);
        Sectors.Add(new Vector2Int(x, y), sector);  
        return sector;
    }

    List<Sector> GetNeighbourSectors(Vector2Int sectorLocation)
    {
        var retval = new List<Sector>();
        retval.Add(GetSector(sectorLocation.x, sectorLocation.y -1));
        retval.Add(GetSector(sectorLocation.x, sectorLocation.y + 1));
        retval.Add(GetSector(sectorLocation.x -1, sectorLocation.y - 1));
        retval.Add(GetSector(sectorLocation.x + 1, sectorLocation.y + 1));
        retval.Add(GetSector(sectorLocation.x + 1, sectorLocation.y));
        retval.Add(GetSector(sectorLocation.x -1, sectorLocation.y));
        retval.Add(GetSector(sectorLocation.x + 1, sectorLocation.y - 1));
        retval.Add(GetSector(sectorLocation.x - 1, sectorLocation.y + 1));
        return retval;
    }

    IEnumerator RaiseSectorToLevel1(Sector sector)
    {
        if (sector.Level >= 1)
            yield break; 

        //initial placement of bubbles. will adjust this later. 
        for (int i = 0; i < sector.Random.Next(3, MaxBubblesPerSector); i++)
        {
            var newBubble = new Bubble(); 
            newBubble.CentrePoint = sector.RandomPointInSector(SectorSize);
            newBubble.Radius = sector.Random.Next(MinBubbleSize, MaxBubbleSize); 
            newBubble.Sector = sector;
            sector.Bubbles.Add(newBubble);
        }
        sector.Level = 1;
        yield return null; 
    }

    IEnumerator RaiseSectorToLevel2(Sector sector)
    {
        //finalise the bubble locations. 
        if (sector.Level >= 2)
            yield break;

        yield return StartCoroutine( RaiseSectorToLevel1 (sector));

        var neighbours = GetNeighbourSectors(sector.Location);
        foreach(var neighbour in neighbours)
            yield return StartCoroutine(RaiseSectorToLevel1(neighbour)); //make sure neighbours are at level 1. 

        foreach(var point in sector.Bubbles)
        {
            var otherpoints = sector.Bubbles.Where(x => x != point).ToList();
            foreach (var neighbbour in neighbours)
                otherpoints.AddRange(neighbbour.Bubbles);
            bool isFarEnoughAway = false; 
            do
            {
                isFarEnoughAway = true; 

                foreach(var otherpoint in otherpoints)
                {
                    if (Vector2Int.Distance(otherpoint.CentrePoint, point.CentrePoint) < otherpoint.Radius + point.Radius)
                    {
                        isFarEnoughAway = false; break;
                    }
                }

                if (!isFarEnoughAway)
                {
                    point.CentrePoint = sector.RandomPointInSector(SectorSize); 
                }
                yield return null;

            }while (!isFarEnoughAway);


            yield return null; 
        }


        sector.Level = 2;
    }

    IEnumerator RaiseToLevel3(Sector sector)
    {
        if (sector.Level >= 3)
            yield break;

        yield return StartCoroutine(RaiseSectorToLevel2(sector));

        var neighbours = GetNeighbourSectors(sector.Location);
        foreach (var neighbour in neighbours)
            yield return StartCoroutine(RaiseSectorToLevel2(neighbour)); //make sure neighbours are at level 2. 


        //connections go here.
        //
        if (sector.Bubbles.Count > 1)
        {
            //ensure all bubbles internally are connected. 
            foreach (var bubble in sector.Bubbles)
            {
                int connectionNo = sector.Random.Next(1, sector.Bubbles.Count);
                bool enoughConnections = false;
                while (enoughConnections)
                {
                    var currentConnections = bubble.ConnectedBubbles.Where(x => x.Sector == sector).ToList();

                    if (currentConnections.Count >= connectionNo)
                    {
                        enoughConnections = true; 
                        break;
                    }

                    var otherbubble = sector.Bubbles.Where(x => x != bubble && !bubble.ConnectedBubbles.Contains(x)).RandomElement(sector.Random); 
                    bubble.ConnectedBubbles.Add(otherbubble);
                    otherbubble.ConnectedBubbles.Add(bubble);  
                }
            }
        }
        yield return null; 
        //ensure that all sectors are connected. 
        foreach (var neighbour in neighbours.Where(x => x.Level < 3))
        {
            var thisBubble = sector.Bubbles.RandomElement(sector.Random); 
            var otherbubble = neighbour.Bubbles.RandomElement(neighbour.Random);

            thisBubble.ConnectedBubbles.Add(otherbubble);
            otherbubble.ConnectedBubbles.Add(thisBubble);

        }

        sector.Level = 3;
    }


    IEnumerator RaiseToLevel4(Sector sector)
    {
        //The base of the bubble, plus outer walls. 
        if (sector.Level >= 4)
            yield break;

        yield return StartCoroutine(RaiseToLevel3(sector));

        var neighbours = GetNeighbourSectors(sector.Location);
        foreach (var neighbour in neighbours)
            yield return StartCoroutine(RaiseToLevel3(neighbour)); //make sure neighbours are at level 3. 

        foreach(var bubble in sector.Bubbles)
        {
            var biome = BiomeDefs.RandomElement(sector.Random);
            bubble.BiomeName = biome.Name;
            GenerateBubble(bubble, biome.Tiles); 
        }

        sector.Level = 4;
    }

    IEnumerator RaiseToLevel5(Sector sector)
    {
        if (sector.Level >= 5) yield break;

        yield return StartCoroutine(RaiseToLevel4(sector));

        var neighbours = GetNeighbourSectors(sector.Location);
        foreach (var neighbour in neighbours)
            yield return StartCoroutine(RaiseToLevel4(neighbour)); //make sure neighbours are at level 4. 


        //sector detail goes here, plus warp points placement.

        //generate portals first. 
        var portalPrefab = PrefabManager.GetPrefab("Portal"); 
        foreach(var bubble in  sector.Bubbles)
        {
            foreach (var otherbubble in bubble.ConnectedBubbles)
            {
                if (bubble.PortalsGenerated.Contains(otherbubble.CentrePoint))
                    continue;
                bool foundloc = false;
                Vector2Int portalLoc;
                do
                {
                    portalLoc = bubble.AvailableSpace.RandomElement(sector.Random);
                    if (bubble.IsAvailable(portalLoc, new Vector2Int(1, 2)))
                    {
                        foundloc = true;
                        bubble.RemoveAvailable(portalLoc, new Vector2Int(1, 2)); 
                    }

                } while (!foundloc);
                foundloc = false;
                Vector2Int otherportalLoc;
                do
                {
                    otherportalLoc = otherbubble.AvailableSpace.RandomElement(sector.Random);
                    if (otherbubble.IsAvailable(otherportalLoc, new Vector2Int(1, 2)))
                    {
                        foundloc = true;
                        otherbubble.RemoveAvailable(portalLoc, new Vector2Int(1, 2));
                    }

                } while (!foundloc);

                var portal1 = Instantiate(portalPrefab, map.CellToWorld(new Vector2Int(portalLoc.x, portalLoc.y+ 1)), portalPrefab.transform.rotation);
                var portal1Scr = portal1.GetComponent<PortalScript>();
                portal1Scr.PortalTo = map.CellToWorld(otherportalLoc);

                var portal2 = Instantiate(portalPrefab, map.CellToWorld(new Vector2Int(otherportalLoc.x, otherportalLoc.y + 1)), portalPrefab.transform.rotation);
                var portal2Scr = portal2.GetComponent<PortalScript>();
                portal2Scr.PortalTo = map.CellToWorld(portalLoc);

                bubble.PortalsGenerated.Add(otherbubble.CentrePoint);
                otherbubble.PortalsGenerated.Add(bubble.CentrePoint);

            }
            yield return null;
        }
        yield return null;


        sector.Level = 5;
    }


    IEnumerator RaiseToLevel6(Sector sector)
    {
        if (sector.Level >= 6) yield break;
        yield return StartCoroutine(RaiseToLevel5(sector));

        var neighbours = GetNeighbourSectors(sector.Location);
        foreach (var neighbour in neighbours)
            yield return StartCoroutine(RaiseToLevel5(neighbour)); //make sure neighbours are at level 3. 

        sector.Level = 6;
    }


    void GenerateBubble(Bubble bubble, List<TileBase> tiles)
    {
        var centerTile = bubble.CentrePoint;
        for (int xdx = centerTile.x - bubble.Radius; xdx < centerTile.x + bubble.Radius; xdx++)
        {
            for (int ydx = centerTile.y - bubble.Radius; ydx < centerTile.y + bubble.Radius; ydx++)
            {
                if (Vector2Int.Distance(centerTile, new Vector2Int(xdx, ydx)) < bubble.Radius)
                {
                    map.SetTile(new Vector3Int(xdx, ydx), tiles.RandomElement(bubble.Sector.Random), TileLayer.Background);
                    bubble.AvailableSpace.Add(new Vector2Int(xdx, ydx));
                }
            }
        }

        for (int r = 0; r <= Mathf.Floor(bubble.Radius * Mathf.Sqrt(0.5f)); r++)
        {
            int d = (int)Mathf.Floor(Mathf.Sqrt(bubble.Radius * bubble.Radius - r * r));
            map.SetTile(new Vector3Int(centerTile.x - d, centerTile.y + r), BubbleWallTile, TileLayer.Walls);
            map.SetTile(new Vector3Int (centerTile.x + d, centerTile.y + r),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x - d, centerTile.y - r),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x + d, centerTile.y - r),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x + r, centerTile.y - d),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x + r, centerTile.y + d),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x - r, centerTile.y - d),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x - r, centerTile.y + d),BubbleWallTile, TileLayer.Walls );
        }

    }



}
