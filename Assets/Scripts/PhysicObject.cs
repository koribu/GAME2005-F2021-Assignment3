using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicObject : MonoBehaviour
{
    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;

    public PhysicSystem physicSystem;
    public float gravityScale = 1.0f;

    PhysicColliderShape colliderShape = PhysicColliderShape.Shere;

    // Start is called before the first frame update
    void Start()
    {
        physicSystem = FindObjectOfType<PhysicSystem>();
        if(physicSystem)
        {
            physicSystem.physicObjects.Add(this);
        }    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + velocity * Time.deltaTime;
    }
}
