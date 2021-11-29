using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject[] balls;
    [SerializeField]
    private int ballNumber;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject ball in balls)
        {
            for(int i = 0; i < ballNumber; i++)
            {
                Instantiate(ball, new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(1.0f, 6.0f), Random.Range(-2.0f, 2.0f)), Quaternion.identity);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
