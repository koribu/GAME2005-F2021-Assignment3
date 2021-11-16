using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhysicColliderShape
{
    Shere = 0,
    Plane,
    AABB
}

public abstract class PhysicCollisionShapeBase : MonoBehaviour
{
    private PhysicColliderShape shape;

    void Start()
    {
        FindObjectOfType<PhysicSystem>().physicColliderShapes.Add(this);
    }

    public abstract  PhysicColliderShape GetColliderShape();



}
