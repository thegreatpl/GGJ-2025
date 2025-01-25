using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;


    public TileManager TileManager; 

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
