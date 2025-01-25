using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random; 

public class Sector
{

    public Vector2Int Location; 

    public int SectorX; 

    public int SectorY;

    public List<Bubble> Bubbles;

    public int Level;


    public Random Random; 


    public Sector(int x, int y, int seed)
    {
        Bubbles = new List<Bubble>();
        SectorX = x;
        SectorY = y;
        Location = new Vector2Int(x, y);

        Random = new Random(x * y * seed);
        Level = 0;
    }


    public Vector2Int RandomPointInSector(int sectorSize)
    {
        int minX = SectorX == 0 ? 0 : SectorX * sectorSize;
        int maxX = SectorX == 0 ? 0 + sectorSize : SectorX * sectorSize + sectorSize;

        int minY = SectorY == 0 ? 0 : SectorY * sectorSize;
        int maxY = SectorY == 0 ? 0 + sectorSize : SectorY * sectorSize + sectorSize;
        return new Vector2Int(Random.Next(minX, maxX), Random.Next(minY, maxY));
    }
}

