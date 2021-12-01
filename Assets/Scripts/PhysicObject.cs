using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicObject : MonoBehaviour
{
    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;

    public PhysicSystem physicSystem;
    public float gravityScale = 1.0f;

    public float elasticityCollision = 1.0f;
    //if toggled true, then physic system will not move whis object
    public bool lockPosition = false;

    //PhysicColliderShape colliderShape = PhysicColliderShape.Sphere;

    // Start is called before the first frame update
    void Start()
    {
        physicSystem = FindObjectOfType<PhysicSystem>();
        if(physicSystem)
        {
            physicSystem.physicObjects.Add(this);
        }    
    }

 /*   // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + velocity * Time.deltaTime;
    }*/
}
