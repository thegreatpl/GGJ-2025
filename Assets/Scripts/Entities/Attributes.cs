using UnityEngine;

public enum WeaponType
{
    Spear
}


public delegate void OnDeath(); 

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

    public int Gold; 


    public OnDeath OnDeath;

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
        if (CurrentHP <= 0)
        {
            Death();
        }
    }


    public void DealDamage(float damage, string type, Attributes attackerAttributes)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            attackerAttributes.Gold += Gold; //kill them, get gold. Probably should drop instead but... 
            Death();
        }
    }

    public void Death()
    {
        OnDeath?.Invoke();
    }


    public void GainExp(int value)
    {
        CurrentExp += value;
        //level up code here. 
    }


    public void CalculateValues()
    {
        MovementSpeed = (float)Agility / 500;

        MaxHP = Constitution * Level * 1.5f;

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
