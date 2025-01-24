using UnityEngine;

public class Attributes : MonoBehaviour
{
    /// <summary>
    /// what team this entity is on. 
    /// </summary>
    public string Faction; 


    public float MaxHP;

    public float CurrentHP;

    public float MovementSpeed;


    public float AttackDistance; 

    public float AttackPower; 

    public int AttackSpeed;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DealDamage(float damage, string type)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) 
            Death();
    }

    public void Death()
    {

    }
}
