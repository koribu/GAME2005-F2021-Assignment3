using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicObject : MonoBehaviour
{
    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;

    public PhysicSystem physicSystem;
    public float gravityScale = 1.0f;

   // [Range(0,1)]
    public float elasticity = 0.6f;
    //if toggled true, then physic system will not move whis object
    public bool lockPosition = false;

    public PhysicCollisionShapeBase shape;
    //PhysicColliderShape colliderShape = PhysicColliderShape.Sphere;

    // Start is called before the first frame update
    void Start()
    {
        physicSystem = FindObjectOfType<PhysicSystem>();
        if(physicSystem)
        {
            physicSystem.physicObjects.Add(this);
        }

        shape = GetComponent<PhysicCollisionShapeBase>();
    }

 /*   // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + velocity * Time.deltaTime;
    }*/
}
