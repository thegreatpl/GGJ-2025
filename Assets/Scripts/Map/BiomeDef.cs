using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

[Serializable]
public class BiomeDef
{
    public string Name;

    public List<string> Tilenames; 

    public List<TileBase> Tiles;

    public float TreeMultiplier; 
}

