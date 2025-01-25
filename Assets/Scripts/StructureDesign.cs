using System.Collections.Generic;
using UnityEngine;

public enum StructureType
{
    Tree, 
    Building
}

public class StructureDesign : MonoBehaviour
{

    public string StructureName;

    public List<string> Biomes;

    public StructureType Type; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
