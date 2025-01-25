using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// AI that stands around until an enemy appears. 
/// </summary>
public class GuardAI : BaseAI
{

    public Attributes TargetGameObject; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attributes = GetComponent<Attributes>();
        movement = GetComponent<Movement>();

        attributes.OnDeath += () => {
            movement.Animator.SetTrigger("Death");
            StartCoroutine(CountdownToDestruction(1));
        }; 
    }

    // Update is called once per frame
    void Update()
    {
        if (attributes.CurrentHP < 0)
        {
            return;
        }

        if (TargetGameObject != null && TargetGameObject.CurrentHP > 0)
        {
            if (CanSee(TargetGameObject.gameObject))
            {
                if (Vector3.Distance(transform.position, TargetGameObject.transform.position) < attributes.AttackDistance)
                {
                    movement.Attack(attributes.DamageType);
                }
                else
                {
                    MoveToPositionDumb(TargetGameObject.transform.position);
                }
            }
            else
            {
                TargetGameObject = null; 
            }

        }
        else
        {
            IdleBehaviour();
        }
    }


    protected void IdleBehaviour()
    {
        var vision = GetVisionObjects().ToList(); 
        List<Attributes> Targets = new List<Attributes>();
        foreach (var v in vision)
        {
            var attribute = v.GetComponent<Attributes>();
            if (attribute != null && attribute.Faction != attributes.Faction)
                Targets.Add(attribute);
        }
        if (Targets.Count > 0)
        {
            TargetGameObject = Targets.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
            return; 
        }
    }

}
