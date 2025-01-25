using UnityEngine;

public class ExitPortalScript : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == GameManager.instance.Player)
            GameManager.instance.Escape(); 
    }
}
