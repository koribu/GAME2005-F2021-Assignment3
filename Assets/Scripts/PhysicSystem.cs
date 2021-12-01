using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicSystem : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<PhysicObject> physicObjects = new List<PhysicObject>();
    public List<PhysicCollisionShapeBase> physicColliderShapes = new List<PhysicCollisionShapeBase>();


    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (PhysicObject obj in physicObjects)
        {
            if (obj.lockPosition) continue;
            obj.transform.position = obj.transform.position + obj.velocity * Time.fixedDeltaTime;
        }

        foreach (PhysicObject obj in physicObjects)
        {
            if (obj.lockPosition) continue;
            obj.velocity += gravity * obj.gravityScale * Time.fixedDeltaTime;
        }

        CollisionUpdate();

    }

    void CollisionUpdate()
    {
        for (int i = 0; i < physicColliderShapes.Count; i++)
        {
            for (int j = 0; j < physicColliderShapes.Count; j++)
            {
                if (i == j) continue;

                PhysicCollisionShapeBase obj1 = physicColliderShapes[i];
                PhysicCollisionShapeBase obj2 = physicColliderShapes[j];
                PhysicColliderShape shapeA = obj1.GetColliderShape();
                PhysicColliderShape shapeB = obj2.GetColliderShape();

                if (shapeA == PhysicColliderShape.Sphere && shapeB == PhysicColliderShape.Sphere)
                {
                    SphereSphereCollision((PhysicCollisionShapeSphere)obj1, (PhysicCollisionShapeSphere)obj2);
                }

                if (shapeA == PhysicColliderShape.Sphere && shapeB == PhysicColliderShape.Plane)
                {
                    SpherePlaneCollision((PhysicCollisionShapeSphere)obj1, (PhysicCollisionShapePlane)obj2);
                }

                if (shapeA == PhysicColliderShape.Plane && shapeB == PhysicColliderShape.Sphere)
                {
                    SpherePlaneCollision((PhysicCollisionShapeSphere)obj2, (PhysicCollisionShapePlane)obj1);
                }
            }
        }
    }
    void SphereSphereCollision(PhysicCollisionShapeSphere a, PhysicCollisionShapeSphere b)
    {

        Vector3 displacementBetweenSpheresAtoB = b.transform.position - a.transform.position;
        float distanceBetween = displacementBetweenSpheresAtoB.magnitude;
        float sumRad = a.radius + b.radius;
        float penetration = sumRad - distanceBetween;
        bool isOverlapping = 0.0f < penetration;

        if (!isOverlapping)
        {
            return;
         
        }
        Color colorA = a.GetComponent<Renderer>().material.color;
        Color colorB = a.GetComponent<Renderer>().material.color;
        a.GetComponent<Renderer>().material.color = colorB;
        b.GetComponent<Renderer>().material.color = colorA;

        // Normalized vector of length 1, representing the direction from A to B, and for two spheres, it is also the normal of our collision
        Vector3 collisioNormalAtoB = displacementBetweenSpheresAtoB / distanceBetween;

        float mtvScalarA;
        float mtvScalarB;

        ComputeMovementScalars(a.physicObject, b.physicObject, out mtvScalarA, out mtvScalarB);

        //the displacement we must move them by to no longer be overlapping
        Vector3 minimumTranslateVectorAtoB = collisioNormalAtoB * penetration ;
        Vector3 translationA = -minimumTranslateVectorAtoB * mtvScalarA;
        Vector3 translationB =  minimumTranslateVectorAtoB * mtvScalarB;

        b.transform.position += minimumTranslateVectorAtoB;

        a.transform.position += translationA;
        b.transform.position += translationB;

        CollisionInfo collisionInfo;
        collisionInfo.colliderA = a;
        collisionInfo.colliderB = b;
        collisionInfo.collisionNormalAtoB = collisioNormalAtoB;
        collisionInfo.contactPoint = a.transform.position + collisioNormalAtoB * a.radius;

        ApplyVelocityResponse(collisionInfo);

    }

    //In C++: computeMovementScalar(float &mtvScalarA)
    // In C#m the out keyword says that variables are beign passed like reference parameters in C++ to be filled with data
    void ComputeMovementScalars(PhysicObject a, PhysicObject b, out float mtvScalarA, out float mtvScalarB)
    {
        if(a.lockPosition && !b.lockPosition)
        {
            mtvScalarA = 0.0f;
            mtvScalarB = 1.0f;
            return;
        }

        if (!a.lockPosition && b.lockPosition)
        {
            mtvScalarA = 1.0f;
            mtvScalarB = 0.0f;
            return;
        }
        if (!a.lockPosition && !b.lockPosition)
        {
            mtvScalarA = .5f;
            mtvScalarB = 0.5f;
            return;
        }

        mtvScalarA = 0.0f;
        mtvScalarB = 0.0f;
    }
    void SpherePlaneCollision(PhysicCollisionShapeSphere a, PhysicCollisionShapePlane b)
    {
 
        Vector3 fromPlaneToSphereCenter = a.transform.position - b.transform.position;

        //use dot product to find the length of the projection of the center of the sphere sphere onto the plane normal
        //This gives the shortest distance from the plane to the center of the sphere
        //The sign is negative, they point in opposite directions
        //if the sign is positive they are at least somewhat in the same d irection

        float dot = Vector3.Dot(fromPlaneToSphereCenter, b.GetNormal());

        float distanceFromPlaneToSphereCenter = dot;

       //bool isOverlapping = distanceFromPlaneToSphereCenter <= a.radius || (distanceFromPlaneToSphereCenter - Mathf.Abs(Vector3.Dot(a.GetComponent<PhysicObject>().velocity,Vector3.up))) <= a.radius;
       
       bool isOverlapping = distanceFromPlaneToSphereCenter <= a.radius;

        if(isOverlapping)
        {
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = a.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = colorB;
            b.GetComponent<Renderer>().material.color = colorA;

            Vector3 aVec = a.GetComponent<PhysicObject>().velocity;

            a.GetComponent<PhysicObject>().velocity += -aVec + 2 * dot * b.GetNormal() /** a.physicObject.elasticityCollision*/;
;
        }
    }

    void ApplyVelocityResponse(CollisionInfo collisionInfo)
    {
        //TODO: bounce!
        //Do something with restitution, mass and velocity
        //will use normal and contact point
        Vector3 vecA = collisionInfo.colliderA.physicObject.velocity;
        Vector3 vecB = collisionInfo.colliderB.physicObject.velocity;

        float massA = collisionInfo.colliderA.physicObject.mass;
        float massB = collisionInfo.colliderB.physicObject.mass;

        // calculating the response = firstVec - 2(dot(firstVec,normal))*normal
        Vector3 response = vecA - 2 * Vector3.Dot(vecA, collisionInfo.collisionNormalAtoB) * collisionInfo.collisionNormalAtoB ;

        //applying the response according to mass
        collisionInfo.colliderA.physicObject.velocity += response * (massB / (2 * massA)) * collisionInfo.colliderA.physicObject.elasticityCollision;
        collisionInfo.colliderB.physicObject.velocity -= response * (massA / (2 * massB)) * collisionInfo.colliderB.physicObject.elasticityCollision; 
 

    }
}



