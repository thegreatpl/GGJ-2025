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
}

