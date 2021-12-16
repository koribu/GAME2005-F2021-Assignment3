using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicSystem : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<PhysicObject> physicObjects = new List<PhysicObject>();
    public List<PhysicCollisionShapeBase> physicColliderShapes = new List<PhysicCollisionShapeBase>();

    float mtvScalarA;
    float mtvScalarB;

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

                if (shapeA == PhysicColliderShape.AABB && shapeB == PhysicColliderShape.AABB)
                {
                    AABBAABBACollision((PhysicCollisionShapeAABB)obj1, (PhysicCollisionShapeAABB)obj2);
                }
                if (shapeA == PhysicColliderShape.AABB && shapeB == PhysicColliderShape.Sphere)
                {
                    AABBASphereCollision((PhysicCollisionShapeAABB)obj1, (PhysicCollisionShapeSphere)obj2);
                }
                if (shapeA == PhysicColliderShape.Sphere && shapeB == PhysicColliderShape.AABB)
                {
                    AABBASphereCollision((PhysicCollisionShapeAABB)obj2, (PhysicCollisionShapeSphere)obj1);
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


        ComputeMovementScalars(a.physicObject, b.physicObject, out mtvScalarA, out mtvScalarB);

        //the displacement we must move them by to no longer be overlapping
        Vector3 minimumTranslateVectorAtoB = collisioNormalAtoB * penetration;
        Vector3 contactPoint = a.transform.position + collisioNormalAtoB * a.radius;
        ApplyMinimumTranslationVector(a.physicObject, b.physicObject, minimumTranslateVectorAtoB, collisioNormalAtoB, contactPoint);
    }

    //In C++: computeMovementScalar(float &mtvScalarA)
    // In C#m the out keyword says that variables are beign passed like reference parameters in C++ to be filled with data
    void ComputeMovementScalars(PhysicObject a, PhysicObject b, out float mtvScalarA, out float mtvScalarB)
    {
        if (a.lockPosition && !b.lockPosition)
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

        if (isOverlapping)
        {
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = a.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = colorB;
            b.GetComponent<Renderer>().material.color = colorA;

            Vector3 aVec = a.GetComponent<PhysicObject>().velocity;

            a.GetComponent<PhysicObject>().velocity += -aVec + 2 * dot * b.GetNormal() /** a.physicObject.elasticityCollision*/;
            
        }
    }

    private void AABBASphereCollision(PhysicCollisionShapeAABB obj1, PhysicCollisionShapeSphere obj2)
    {
        Vector3 fromAABBCenterToSphereCenter = obj2.transform.position - obj1.transform.position;
        Vector3 normal = new Vector3(0, 1, 0);
        float dot = Vector3.Dot(fromAABBCenterToSphereCenter, normal);

        float distanceFromSurfaceToSurface = Vector3.Magnitude(fromAABBCenterToSphereCenter) - Vector3.Magnitude(obj1.getHalfSize());
        if (distanceFromSurfaceToSurface <0)
        {
            Vector3 bVec = obj2.GetComponent<PhysicObject>().velocity;

            obj2.physicObject.velocity += -bVec + 2 * dot * normal;
        }

    }
    private void AABBAABBACollision(PhysicCollisionShapeAABB obj1, PhysicCollisionShapeAABB obj2)
    {
        //Get displacement between the boxes
        Vector3 displacementAB = obj2.transform.position - obj1.transform.position;
        //find the length of their projections along each axis

        //compare the distance between them along each axis with half of their size in that axis
        float penatrationX = obj1.getHalfSize().x + obj2.getHalfSize().x - Mathf.Abs(displacementAB.x);
        float penatrationY = obj1.getHalfSize().y + obj2.getHalfSize().y - Mathf.Abs(displacementAB.y);
        float penatrationZ = obj1.getHalfSize().z + obj2.getHalfSize().z - Mathf.Abs(displacementAB.z);
        //if the distance (along that axis) is less than the sum of their half sizes (along that axis) then they must be overlapping

        if(penatrationX<0 || penatrationY < 0 || penatrationZ < 0 )
        {
            return; // no collision
        }

     
        Vector3 normal = new Vector3(Mathf.Sign(displacementAB.x), 0, 0);
        Vector3 minimumTranslateVectorAtoB = normal * penatrationX;

        //find the shortest penetration to move along
/*        if (penatrationX < penatrationY && penatrationX < penatrationZ) // X is shortest
        {
            normal = new Vector3(Mathf.Sign(displacementAB.x), 0, 0);
            minimumTranslateVectorAtoB = normal * penatrationX;
        }*/
        if (penatrationY < penatrationX && penatrationY < penatrationZ) // Y is shortest
        {
            normal = new Vector3(0, Mathf.Sign(displacementAB.y), 0);
            minimumTranslateVectorAtoB = normal * penatrationY;
        }
        else if (penatrationZ < penatrationY && penatrationZ < penatrationX) // Z is shortest
        {
            normal = new Vector3(0, 0, Mathf.Sign(displacementAB.z));
            minimumTranslateVectorAtoB = normal * penatrationZ;
        }
        Vector3 contactPoint = obj1.transform.position + minimumTranslateVectorAtoB;
        //find the minimum translation vector to move them

        //apply displacement to separate them along the shortest path we can
        ApplyMinimumTranslationVector(obj1.physicObject, obj2.physicObject, minimumTranslateVectorAtoB, normal, contactPoint);
    }

    private void ApplyMinimumTranslationVector(PhysicObject a, PhysicObject b, Vector3 minimumTranslateVectorAtoB, Vector3 normal, Vector3 contactPoint)
    {

        ComputeMovementScalars(a, b, out mtvScalarA, out mtvScalarB);

        Vector3 translationA = -minimumTranslateVectorAtoB * mtvScalarA;
        Vector3 translationB = minimumTranslateVectorAtoB * mtvScalarB;


        a.transform.position += translationA;
        b.transform.position += translationB;

        CollisionInfo collisionInfo;
        collisionInfo.colliderA = a.shape;
        collisionInfo.colliderB = b.shape;
        collisionInfo.collisionNormalAtoB = normal;
        collisionInfo.contactPoint = contactPoint;

        ApplyVelocityResponse(collisionInfo);
    }

    void ApplyVelocityResponse(CollisionInfo collisionInfo)
    {
        PhysicObject objA = collisionInfo.colliderA.physicObject;
        PhysicObject objB = collisionInfo.colliderB.physicObject;
        Vector3 normal = collisionInfo.collisionNormalAtoB;

        //velocity B realtive to A
        Vector3 relativeVelocityAB = objB.velocity - objA.velocity;
        //find relative velocity along the collision normal axis
        float relativeNormalVelocityAB = Vector3.Dot(relativeVelocityAB, normal);

        // early exit if they are not going towards each other (no bounce)
        if (relativeNormalVelocityAB >= 0.0f)
        {
            return;
        }

        // Choose a coefficient of restitution
        float restitution = (objA.elasticity + objB.elasticity) * .5f;

        //Determine change in velocity necessary
        float deltaV = -(relativeNormalVelocityAB * (1.0f + restitution));
        float impulse;

        //determine the impulse needed to apply this change
        //apply the impulse to each object

        //respond differently based on locked states
        if (!objA.lockPosition && objB.lockPosition)
        {
            //impulse required to create our desired change velocity
            //impulse = Force * time = kg + m/s^2 * s = kg m/s
            //only A change velocity
            impulse = deltaV * objA.mass;
            objA.velocity -= normal * impulse / objA.mass;
        }
        else if (objA.lockPosition && !objB.lockPosition)
        {
            //only B change velocity
            impulse = deltaV * objB.mass;
            objB.velocity += normal * impulse / objB.mass;
        }
        else if (!objA.lockPosition && !objB.lockPosition)
        {
            //Both change velocity
            impulse = deltaV / ((1.0f / objA.mass) + (1.0f / objB.mass));
            objA.velocity -= normal * impulse / objA.mass;
            objB.velocity += normal * impulse / objB.mass;
        }
        else
            return;


        /*        //TODO: bounce!
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
                collisionInfo.colliderB.physicObject.velocity -= response * (massA / (2 * massB)) * collisionInfo.colliderB.physicObject.elasticityCollision; */


    }
}



