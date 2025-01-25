using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;


    public TileManager TileManager; 

    public MapController MapController;

    public WorldGenerator WorldGenerator;


    public GameObject Player; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return; 
        }
        instance = this;

        TileManager = GetComponent<TileManager>();
        WorldGenerator = GetComponent<WorldGenerator>();

        StartCoroutine(StartNewGame()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator StartNewGame()
    {
        yield return StartCoroutine(TileManager.LoadTiles()); 
        WorldGenerator.tileManager = TileManager;
        WorldGenerator.map = MapController;
        yield return StartCoroutine(WorldGenerator.GenerateWorld());
    }
}
