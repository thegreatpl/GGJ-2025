using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{

   // Random Random; 

    public MapController map;

    public TileManager tileManager;

    public PrefabManager PrefabManager; 

    public StructureManager StructureManager;

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


    public Vector3 Spawn = Vector3.zero;

    protected List<Sector> sectorsGenerating = new List<Sector>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PrefabManager = GetComponent<PrefabManager>();
        StructureManager = GetComponent<StructureManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!generated)
            return;
        if (GameManager.instance?.Player != null)
        {
            var playerloc = GameManager.instance.Player.transform.position;

            var sector = GetSector((int)playerloc.x / SectorSize, (int)playerloc.y / SectorSize);
            if (sector.Level < 6 && !sectorsGenerating.Contains(sector))
            {
                StartCoroutine(RaiseToLevel6(sector));
                sectorsGenerating.Add(sector);
            }
        }
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
        var firstsect = GetSector(0, 0);
        bool spawnPlaced = false;
        do
        {
            var bubble = firstsect.Bubbles.RandomElement(firstsect.Random);

            var structure = StructureManager.StructureDefs.FirstOrDefault(x => x.name == "ExitPortal");

            var basloc = bubble.AvailableSpace.RandomElement(bubble.Sector.Random);

            if (bubble.IsAvailable(basloc, structure.Size))
            {
                ApplyStructure(basloc, structure);
                bubble.RemoveAvailable(basloc, structure.Size);
                Spawn = map.CellToWorld(basloc);
                spawnPlaced = true;

            }

            } while (!spawnPlaced);

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
                foreach(var otherbubble in sector.Bubbles)
                {
                    if (bubble.ConnectedBubbles.Contains(otherbubble) || bubble== otherbubble)
                        continue;
                    bubble.ConnectedBubbles.Add(otherbubble);
                    otherbubble.ConnectedBubbles.Add(bubble);

                }

                continue; 
                //doesn't seem to work as intended. 
                int connectionNo = sector.Bubbles.Count;// sector.Random.Next(1, sector.Bubbles.Count);
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


        //try to generate structures. 
        foreach(var bubble in sector.Bubbles)
        {
            GenerateStructures(bubble);
        }
        yield return null; 
        //generate trees
        foreach (var bubble in sector.Bubbles)
        {
            GenerateTrees(bubble);
                }

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
            map.SetTile(new Vector3Int (centerTile.x - d, centerTile.y + r), BubbleWallTile, TileLayer.Walls);
            map.SetTile(new Vector3Int (centerTile.x + d, centerTile.y + r),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x - d, centerTile.y - r),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x + d, centerTile.y - r),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x + r, centerTile.y - d),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x + r, centerTile.y + d),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x - r, centerTile.y - d),BubbleWallTile, TileLayer.Walls );
            map.SetTile(new Vector3Int (centerTile.x - r, centerTile.y + d),BubbleWallTile, TileLayer.Walls );
            //walls not available for building on. 
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x - d, centerTile.y + r));
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x + d, centerTile.y + r));
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x - d, centerTile.y - r));
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x + d, centerTile.y - r));
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x + r, centerTile.y - d));
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x + r, centerTile.y + d));
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x - r, centerTile.y - d));
            bubble.AvailableSpace.Remove( new Vector2Int(centerTile.x - r, centerTile.y + d));
        }

    }

    void GenerateStructures(Bubble bubble)
    {
        var posibilities = StructureManager.StructureDefs.Where(x => x.Biomes.Contains(bubble.BiomeName) && x.Type == "Building").ToList();
        if (!posibilities.Any())
        {
            return;
        }

        var attempts = bubble.Radius;
        for (int idx = 0; idx < attempts; idx++)
        {
            var structure = posibilities.RandomElement(bubble.Sector.Random);

            var basloc = bubble.AvailableSpace.RandomElement(bubble.Sector.Random);

            if (bubble.IsAvailable(basloc, structure.Size))
            {
                ApplyStructure(basloc, structure);
                bubble.RemoveAvailable(basloc, structure.Size);

                //only one of each structure per bubble. 
                posibilities.Remove(structure);
                if (!posibilities.Any())
                {
                    break;
                }
            }
        }
    }

    void GenerateTrees(Bubble bubble)
    {
        var treeposibilities = StructureManager.StructureDefs.Where(x => x.Biomes.Contains(bubble.BiomeName) && x.Type == "Tree");

        if (!treeposibilities.Any())
        {
            return; //no tress for biome
        }

        var biome = BiomeDefs.FirstOrDefault(y => y.Name == bubble.BiomeName);

        var attempts = bubble.Radius * biome.TreeMultiplier;

        for (int id = 0; id < attempts; id++)
        {
            var tree = treeposibilities.RandomElement(bubble.Sector.Random);

            var basloc = bubble.AvailableSpace.RandomElement(bubble.Sector.Random);

            if (bubble.IsAvailable(basloc, tree.Size))
            {
                ApplyStructure(basloc, tree);
                bubble.RemoveAvailable(basloc, tree.Size);
            }

        }

    }

    public void ApplyStructure (Vector2Int location, StructureDef structure)
    {
        foreach (var tile in structure.tiles)
        {
            TileLayer layer; 
            switch(tile.Layer)
            {
                case "Background":
                    layer = TileLayer.Background; break;
                case "Wall": layer = TileLayer.Walls; break;
                case "Foreground": layer = TileLayer.Foreground; break;
                case "Detail": layer= TileLayer.Detail; break;
                default:
                    layer = TileLayer.Background; break; 
            }
            map.SetTile(new Vector3Int(tile.x + location.x, tile.y + location.y), tileManager.GetTile(tile.tilename), layer);
        }

        foreach (var prefab in structure.prefabs)
        {
            var pre = PrefabManager.GetPrefab(prefab.name);
            if (pre != null)
            {
                Instantiate(pre, map.CellToWorld(location) + prefab.location, pre.transform.rotation);
            }
        }
    }

}
