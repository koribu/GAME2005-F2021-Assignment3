using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicSystem : MonoBehaviour
{
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public List<PhysicObject> physicObjects;

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
        
    }
}
