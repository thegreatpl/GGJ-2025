using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public Vector3 PortalTo; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.transform.position = PortalTo;
    }
}
