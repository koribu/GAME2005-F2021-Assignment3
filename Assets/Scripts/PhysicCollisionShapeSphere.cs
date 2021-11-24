using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicCollisionShapeSphere : PhysicCollisionShapeBase
{
    public float radius = 1.0f;

    public override PhysicColliderShape GetColliderShape()
    {
        return PhysicColliderShape.Sphere;
    }
}
