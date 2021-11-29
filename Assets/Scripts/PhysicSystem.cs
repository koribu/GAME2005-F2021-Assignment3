using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicSystem : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<PhysicObject> physicObjects = new List<PhysicObject>();
    public List<PhysicCollisionShapeBase> physicColliderShapes = new List<PhysicCollisionShapeBase>();

    [SerializeField]
    float collisionEnergyLoss = .6f;

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

        float dot = Vector3.Dot(fromPlaneToSphereCenter, b.GetNormal());

        float distanceFromPlaneToSphereCenter = Mathf.Abs(dot);

       //bool isOverlapping = distanceFromPlaneToSphereCenter <= a.radius || (distanceFromPlaneToSphereCenter - Mathf.Abs(Vector3.Dot(a.GetComponent<PhysicObject>().velocity,Vector3.up))) <= a.radius;
       
       bool isOverlapping = distanceFromPlaneToSphereCenter <= a.radius;

        if(isOverlapping)
        {
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = a.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = colorB;
            b.GetComponent<Renderer>().material.color = colorA;

            Vector3 aVec = a.GetComponent<PhysicObject>().velocity;

          /*  a.GetComponent<PhysicObject>().velocity = new Vector3(aVec.x + 2 * dot * b.GetNormal().x , aVec.y + 2 * dot * b.GetNormal().y , aVec.z + 2 * dot * b.GetNormal().z );
            a.GetComponent<PhysicObject>().veilocity = new Vector3(a.GetComponent<PhysicObject>().velocity.x * collisionEnergyLoss, a.GetComponent<PhysicObject>().velocity.y * collisionEnergyLoss, a.GetComponent<PhysicObject>().velocity.z * collisionEnergyLoss);
           // a.GetComponent<PhysicObject>().velocity = a.GetComponent<PhysicObject>().velocity - 2*dot* b.GetNormal();
            //a.GetComponent<PhysicObject>().velocity = - aVec  2 * Vector3.Dot(aVec, Vector3.Normalize(fromPlaneToSphereCenter)) *.3f* fromPlaneToSphereCenter;
      */  }
    }
}
