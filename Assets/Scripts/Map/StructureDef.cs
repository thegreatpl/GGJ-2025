using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class StructureDef
{
    public string name; 


    public List<StructureTile> tiles;

    public List<string> Biomes;

    public string Type; 

    public List<StructurePrefab> prefabs;

    public Vector2Int Size; 

}



[Serializable]
public class StructureTile
{
    public string tilename;

    public string Layer; 

    public int x; 

    public int y;
}


[Serializable]
public class StructurePrefab
{
    public string name;

    public Vector3 location;
}
