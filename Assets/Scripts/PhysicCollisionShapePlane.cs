using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicCollisionShapePlane : PhysicCollisionShapeBase
{
    public override PhysicColliderShape GetColliderShape()
    {
        return PhysicColliderShape.Plane;
    }

    public Vector3 GetNormal()
    {
        return transform.up;
    }


}
