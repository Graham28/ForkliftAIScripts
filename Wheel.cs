using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Wheel : MonoBehaviour
{
    public bool powered = false;
    public float maxAngle = 90f;
    public float offset = 0f;

    private float turnAngle;
    private WheelCollider wcol;

    private void Start()
    {
        wcol = GetComponent<WheelCollider>();
    }

    public void Steer(float steerInput)
    {
        turnAngle = steerInput * maxAngle + offset;
        wcol.steerAngle = turnAngle;
    }

    public void Accelerate(float powerInput)
    {
        if(powered) wcol.motorTorque = powerInput;
        else wcol.brakeTorque = 0;
    }




    public void UpdatePosition()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        wcol.GetWorldPose(out pos, out rot);
        //wmesh.transform.position = pos;
        //wmesh.transform.rotation = rot;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Accelerate(400);
            
        }

        else if (Input.GetKey(KeyCode.S))
        {
            Accelerate(-400); 
        }

        if (Input.GetKey(KeyCode.D))
        {
            Steer(0.5f);
        }

        else if (Input.GetKey(KeyCode.A))
        {
            Steer(-0.5f);
        }
    }
}
