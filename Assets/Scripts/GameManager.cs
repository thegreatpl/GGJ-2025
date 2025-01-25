using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public Camera Camera; 

    public TileManager TileManager; 

    public MapController MapController;

    public WorldGenerator WorldGenerator;

    public PrefabManager PrefabManager;

    public StructureManager StructureManager;

    public UIController UIController;


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
        UIController = GetComponentInChildren<UIController>();

        DontDestroyOnLoad(gameObject);
        if (Camera == null)
            Camera = Camera.main;

        if (Camera != null)
            DontDestroyOnLoad(Camera);

        StartCoroutine(LoadGame()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator LoadGame()
    {
        UIController.HideInfoScreen();
        UIController.SetGameUI(false);
        yield return StartCoroutine(TileManager.LoadTiles()); 
        yield return StartCoroutine(StructureManager.LoadStructures());


        if (SceneManager.GetActiveScene().name != "MainMenu")
            yield return StartCoroutine(StartNewGame("Tiffany")); 
    }

    public void StartGame(string character)
    {
        instance.StartCoroutine(instance.StartNewGame(character));
    }

    IEnumerator StartNewGame(string Character)
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        if (Camera == null)
        {
            Camera = Instantiate(PrefabManager.GetPrefab("Camera")).GetComponent<Camera>();
            DontDestroyOnLoad(Camera);
        }
        yield return null;       
        UIController.ShowMessage("Loading..."); 
        MapController = FindAnyObjectByType<MapController>();//there should only be one so... 
        WorldGenerator.tileManager = TileManager;
        WorldGenerator.map = MapController;
        yield return StartCoroutine(WorldGenerator.GenerateWorld());
        var characterPrefab = PrefabManager.GetPrefab(Character);
        if (characterPrefab == null)
        {
            UIController.SendMessage($"ERROR: Character {Character} not found!");
            StartCoroutine(EndGameWaitForInput());
        }
        Player = Instantiate(characterPrefab);
        Camera.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, Camera.transform.position.z);
        Camera.transform.SetParent(Player.transform);
        var attributes = Player.GetComponent<Attributes>();
        attributes.OnDeath += () =>
        {
            var movement = Player.GetComponent<Movement>();
            movement.Animator.SetTrigger("Death");
            PlayerDeath(); 
        };

        yield return null; 
        UIController.HideInfoScreen();
        UIController.SetGameUI(true);
    }

    public void Escape()
    {
        if (Player == null)
            //wtf this should not happen. 
            return;

        var attributes = Player.GetComponent<Attributes>();
        int gold = attributes.Gold;
        Player.transform.DetachChildren(); 
        Destroy(Player);
        UIController.ShowMessage($"You have escaped from the bubble worlds, carrying a fortune in gold. Earned: {gold}gp");
        StartCoroutine(EndGameWaitForInput());

    }

    public void PlayerDeath()
    {
        Player.transform.DetachChildren(); 
        Camera.transform.parent = null; 
        Destroy(Player);
        UIController.ShowMessage("Death... You failed to escape the bubble worlds"); 
        StartCoroutine(EndGameWaitForInput());
    }

    IEnumerator EndGameWaitForInput()
    {
        yield return new WaitForSeconds(5); 
         UIController.HideInfoScreen();
        UIController.SetGameUI(false)  ;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); 
       
    }
}
