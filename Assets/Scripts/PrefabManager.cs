using System;
using System.Collections.Generic;
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
}


[Serializable]
public class PrefabDef
{
    public string name;

    public GameObject prefab;
}
