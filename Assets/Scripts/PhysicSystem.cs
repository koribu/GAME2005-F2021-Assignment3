using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicSystem : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<PhysicObject> physicObjects = new List<PhysicObject>();
    public List<PhysicCollisionShapeBase> physicColliderShapes = new List<PhysicCollisionShapeBase>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (PhysicObject obj in physicObjects)
        {
            obj.velocity += gravity * obj.gravityScale * Time.deltaTime;
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

        Vector3 displacementBetweenSphere = a.transform.position - b.transform.position;
        float distanceBetween = displacementBetweenSphere.magnitude;
        float sumRad = a.radius + b.radius;
        bool isOverlapping = distanceBetween < sumRad;

        if (isOverlapping)
        {
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = a.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = colorB;
            b.GetComponent<Renderer>().material.color = colorA;
        }
    }

    void SpherePlaneCollision(PhysicCollisionShapeSphere a, PhysicCollisionShapePlane b)
    {
        Vector3 fromPlaneToSphereCenter = a.transform.position - b.transform.position;

        float dot = Vector3.Dot(fromPlaneToSphereCenter, b.GetNormal());

        float distanceFromPlaneToSphereCenter = Mathf.Abs(dot);

        bool isOverlapping = distanceFromPlaneToSphereCenter <= a.radius;

        if(isOverlapping)
        {
            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = a.GetComponent<Renderer>().material.color;
            a.GetComponent<Renderer>().material.color = colorB;
            b.GetComponent<Renderer>().material.color = colorA;

            a.GetComponent<PhysicObject>().velocity = new Vector3(0, 0, 0);
        }
    }
}
