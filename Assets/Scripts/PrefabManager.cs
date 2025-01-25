using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{

    public List<PrefabDef> Prefabs; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject GetPrefab(string name)
    {
        return Prefabs.FirstOrDefault(x => x.Name == name).prefab;
    }
}


[Serializable]
public class PrefabDef
{
    public string Name;

    public string Type; 

    public GameObject prefab;
}
