using UnityEngine;

public class Attributes : MonoBehaviour
{
    /// <summary>
    /// what team this entity is on. 
    /// </summary>
    public string Faction;

    public int Level; 

    public int Strength;

    public int Agility;

    public int Constitution;

    public int Wisdom;

    public int Intelligence; 


    public int CurrentExp; 

    public float MaxHP;

    public float CurrentHP;

    public float MovementSpeed;


    public float AttackDistance; 

    public float AttackPower; 

    public int AttackSpeed;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaxHP = Constitution;
        CurrentExp = 0;

        CalculateValues();

        CurrentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DealDamage(float damage, string type, Attributes attackerAttributes)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0)
        {
            Death();
        }
    }

    public void Death()
    {

    }


    public void GainExp(int value)
    {
        CurrentExp += value;
        //level up code here. 
    }


    public void CalculateValues()
    {
        MovementSpeed = (float)Agility / 100; 

    }
}
