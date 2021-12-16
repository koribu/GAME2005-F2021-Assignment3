using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysicCollisionShapeAABB : PhysicCollisionShapeBase
{
    public Vector3 dimensions = new Vector3(1, 1, 1);

    public Vector3 getMin()
    {
        return transform.position - getHalfSize();
    }
    public Vector3 getMax()
    {
        return transform.position + getHalfSize();
    }
    public Vector3 getSize()
    {
        return Vector3.Scale(dimensions, transform.lossyScale);
    }
    public Vector3 getHalfSize()
    {
        return Vector3.Scale(dimensions, transform.lossyScale) * .5f;
    }

    public override PhysicColliderShape GetColliderShape()
    {
        return PhysicColliderShape.AABB;
    }

}
