using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerController : MonoBehaviour
{
    public Movement movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        if (x > 0)
            x = 1;
        else if (x < 0) 
            x = -1;
        if (y > 0) 
            y = 1;
        else if (y < 0) 
            y = -1;

        movement.MovementDirction = new Vector2 (x, y);


        if (Input.GetButtonDown("Jump"))
            movement.Attack("piercing"); 
       

    }
}
