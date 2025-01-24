using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Attributes))]
[RequireComponent(typeof(Movement))]
public class BaseAI : MonoBehaviour
{
    public Movement movement;
    public Attributes attributes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponent<Movement>();
        attributes = GetComponent<Attributes>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void MoveToPositionDumb(Vector3 targetPos)
    {
        var distance = targetPos - transform.position;

        if (Vector3.Distance(distance, transform.position) < 0.1f)
        {
            movement.MovementDirction = Vector2.zero;
        }
        else
        {
            movement.MovementDirction = distance;
        }
   
    }

    protected bool CanSee(GameObject other)
    {
        var hit = Physics2D.Raycast(transform.position, other.transform.position - transform.position, attributes.SightDistance);
        if (hit.transform?.gameObject == other)
        { return true; }
        return false;
    }

    protected IEnumerable<GameObject> GetVisionObjects()
    {
        var results = Physics2D.OverlapCircleAll(transform.position, attributes.SightDistance);

        foreach (var obj in results)
        {
            yield return obj.gameObject;
        }
    }
}
