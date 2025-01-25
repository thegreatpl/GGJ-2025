using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public List<StructureDef> StructureDefs; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public IEnumerator LoadStructures()
    {
        StructureDefs = new List<StructureDef>();
        var text = Resources.LoadAll("Structures", typeof(TextAsset));

        foreach (var t in text)
        {
            var te = t as TextAsset;
            if (te == null)
                continue;

            var structure = JsonUtility.FromJson<StructureDef>(te.text);
            StructureDefs.Add(structure);
        }

        yield return null;
    }
}

