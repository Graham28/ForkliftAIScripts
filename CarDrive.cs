using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDrive : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;

    public float speed;
    public float turnSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(Vector3.forward*speed*10);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(-Vector3.forward*speed*10);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddTorque(Vector3.up*turnSpeed*10);
        }

        else if (Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(-Vector3.up*turnSpeed*10);
        }
    }
}
