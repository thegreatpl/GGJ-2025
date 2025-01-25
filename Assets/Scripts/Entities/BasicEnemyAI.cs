using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BasicEnemyAI : BaseAI
{
    public Vector3? TargetLocation;

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
        else if (TargetLocation.HasValue)
        {
            if (Vector3.Distance(transform.position, TargetLocation.Value) < 0.1f
                || Vector3.Distance(transform.position, TargetLocation.Value) > attributes.SightDistance)
            {
                TargetLocation = null; 
            }
            else
            {
                MoveToPositionDumb(TargetLocation.Value);
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

        //random position to wander. 
        TargetLocation = new Vector3(Random.Range(transform.position.x - (attributes.SightDistance / 2),
            transform.position.x + (attributes.SightDistance / 2)), Random.Range(transform.position.y - (attributes.SightDistance / 2),
            transform.position.y + (attributes.SightDistance / 2)));

        if (!GameManager.instance.MapController.IsPassable(TargetLocation.Value))
            TargetLocation = null; 
    }

}
