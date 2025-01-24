using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Attributes))]  
public class Movement : MonoBehaviour
{
    public Attributes Attributes;

    public Vector2 MovementDirction; 


    public Animator Animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MovementDirction = Vector2.zero;
        Attributes = GetComponent<Attributes>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)Vector2.ClampMagnitude(MovementDirction, Attributes.MovementSpeed);

        //animation stuff
        if (MovementDirction == Vector2.zero)
        {
            Animator.SetBool("IsWalking", false);
        }
        else if (Mathf.Abs(MovementDirction.x) > Mathf.Abs(MovementDirction.y))
        {
            if (MovementDirction.x < 0)
            {
                Animator.SetInteger("Direction", 1); 
                Animator.SetBool("IsWalking", true);
            }
            else
            {
                Animator.SetInteger("Direction", 3);
                Animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            if (MovementDirction.y < 0)
            {
                Animator.SetInteger("Direction", 2);
                Animator.SetBool("IsWalking", true);
            }
            else
            {
                Animator.SetInteger("Direction", 0);
                Animator.SetBool("IsWalking", true);
            }
        }
    }
}
