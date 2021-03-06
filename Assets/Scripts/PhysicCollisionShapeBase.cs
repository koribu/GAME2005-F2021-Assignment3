using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhysicColliderShape
{
    Sphere = 0,
    Plane,
    AABB
}

struct CollisionInfo
{
    public PhysicCollisionShapeBase colliderA;
    public PhysicCollisionShapeBase colliderB;
    public Vector3 collisionNormalAtoB;
    public Vector3 contactPoint;
}
[RequireComponent(typeof(PhysicObject))]//Unity-specific Attribute to make sure this component requires to have another on the same gameobject work
public abstract class PhysicCollisionShapeBase : MonoBehaviour
{
    public PhysicObject physicObject;

    void Start()
    {
        physicObject = GetComponent<PhysicObject>();
        FindObjectOfType<PhysicSystem>().physicColliderShapes.Add(this);
    }

    public abstract  PhysicColliderShape GetColliderShape();



}
