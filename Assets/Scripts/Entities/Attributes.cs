using UnityEngine;

public enum WeaponType
{
    Spear
}

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

    public WeaponType Weapon;


    public int CurrentExp; 

    public float MaxHP;

    public float CurrentHP;

    public float MovementSpeed;


    public float AttackDistance; 

    public float AttackPower; 

    public int AttackSpeed;

    public string DamageType; 

    public float SightDistance; 



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        MovementSpeed = (float)Agility / 1000;

        MaxHP = Constitution * Level;

        switch (Weapon)
        {
            case WeaponType.Spear:
                AttackPower = 1 * ((float)Strength / 10);
                AttackSpeed = 100;
                AttackDistance = 1.5f;
                DamageType = "piercing"; 
                break;

            default:
                AttackSpeed = 0;
                AttackDistance = 0;
                AttackPower = 0;
                break; 
        }

        SightDistance = Wisdom * 10; 
    }
}
