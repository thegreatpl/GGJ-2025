using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Attributes))]  
public class Movement : MonoBehaviour
{
    public Attributes Attributes;

    public Vector2 MovementDirction;

    public int attackCooldown; 

    public Animator Animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MovementDirction = Vector2.zero;
        Attributes = GetComponent<Attributes>();
        Animator = GetComponent<Animator>();
        attackCooldown = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown--;
            return; 
        }

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


    public void Attack (string type)//float damage, , int cooldown)
    {
        if (attackCooldown > 0)
        {
            return;
        }
        Vector2 direction;
        switch (Animator.GetInteger("Direction"))
        {
            case 0:
                direction = Vector2.up;
                break;
            case 1:
                direction = Vector2.left;
                break;
            case 2:
                direction = Vector2.down;
                break;
            case 3:
                direction = Vector2.right;
                break;
            default:
                direction = Vector2.down;
                break;
        }

        var hits = Physics2D.BoxCastAll(transform.position, new Vector2(1, 1), 0, direction, Attributes.AttackDistance);
        foreach (var hit in hits)
        {
            var attribute = hit.rigidbody?.gameObject?.GetComponent<Attributes>();
            if (attribute != null && attribute.Faction != Attributes.Faction)
            {
                attribute.DealDamage(Attributes.AttackPower, type, Attributes);
            }
        }
        if (hits.Length > 0)
        {
            attackCooldown = Attributes.AttackSpeed;
            Animator.SetTrigger("ThrustAttack"); 
        }
    }
}
