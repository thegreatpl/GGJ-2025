using UnityEngine;

[RequireComponent(typeof(Attributes))]  
public class Movement : MonoBehaviour
{
    public Attributes Attributes;

    public Vector2 MovementDirction; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MovementDirction = Vector2.zero;
        Attributes = GetComponent<Attributes>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)Vector2.ClampMagnitude(MovementDirction, Attributes.MovementSpeed);
    }
}
