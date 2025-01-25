using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Bubble
{
    public Vector2Int CentrePoint;

    public int Radius;

    public string BiomeName;

    public Sector Sector; 

    public List<Bubble> ConnectedBubbles = new List<Bubble>();

    public List<Vector2Int> PortalsGenerated = new List<Vector2Int>();

    public List<Vector2Int> AvailableSpace = new List<Vector2Int>();


    public bool IsAvailable (Vector2Int loc)
    {
        return AvailableSpace.Contains (loc);
    }

    public bool IsAvailable(Vector2Int loc, Vector2Int Size)
    {
        for (int idx = loc.x; idx < loc.x + Size.x; idx++)
        {
            for (int ydx = loc.y; ydx < loc.y + Size.y; ydx++)
            {
                if (!IsAvailable(new Vector2Int(idx, ydx)))
                    return false;
            }
        }
        return true;
    }

    public void RemoveAvailable(Vector2Int loc, Vector2Int Size)
    {
        for (int idx = loc.x; idx < loc.x + Size.x; idx++)
        {
            for (int ydx = loc.y; ydx < loc.y + Size.y; ydx++)
            {
                AvailableSpace.Remove(new Vector2Int(idx, ydx));
            }
        }
    }
}

