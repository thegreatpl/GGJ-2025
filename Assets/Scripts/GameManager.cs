using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;


    public TileManager TileManager; 

    public MapController MapController;

    public WorldGenerator WorldGenerator;

    public PrefabManager PrefabManager;

    public StructureManager StructureManager;


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
        PrefabManager = GetComponent<PrefabManager>();
        StructureManager = GetComponent<StructureManager>();    

        DontDestroyOnLoad(gameObject);

        StartCoroutine(LoadGame()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator LoadGame()
    {
        yield return StartCoroutine(TileManager.LoadTiles()); 
        yield return StartCoroutine(StructureManager.LoadStructures());

        yield return StartCoroutine(StartNewGame()); 
    }

    IEnumerator StartNewGame()
    {
        MapController = FindAnyObjectByType<MapController>();//there should only be one so... 
        WorldGenerator.tileManager = TileManager;
        WorldGenerator.map = MapController;
        yield return StartCoroutine(WorldGenerator.GenerateWorld());
    }
}
