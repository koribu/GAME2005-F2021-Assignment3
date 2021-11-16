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
        foreach(PhysicObject obj in physicObjects)
        {
            obj.velocity += gravity * obj.gravityScale * Time.deltaTime;
        }
        for (int i = 0; i<physicColliderShapes.Count; i++)
        {
            for (int j = 0; j < physicColliderShapes.Count; j++)
            {
                if (i == j) continue;

                PhysicCollisionShapeBase obj1 = physicColliderShapes[i];
                PhysicCollisionShapeBase obj2 = physicColliderShapes[j];
                PhysicColliderShape shapeA = obj1.GetColliderShape();
                PhysicColliderShape shapeB = obj2.GetColliderShape();

                if(shapeA == PhysicColliderShape.Shere && shapeB == PhysicColliderShape.Shere)
                {
                    SphereSphereCollision((PhysicCollisionShapeSphere)obj1, (PhysicCollisionShapeSphere)obj2);
                }
            }
        }

    }

    void SphereSphereCollision(PhysicCollisionShapeSphere a, PhysicCollisionShapeSphere b)
    {
        if (Vector3.Distance(a.transform.position, b.transform.position) < (a.radius + b.radius))
            Debug.LogError("Two object collided!");
    }
}
