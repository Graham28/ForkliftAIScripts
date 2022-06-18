using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class ForkliftDrive : Agent
{
    private Rigidbody rb;
    public Wheel[] wheels;
    public Transform lift;
    public Transform Target;
    public Rigidbody targetRigidBody;
    public float CentreOfMass = -0.9f;

    void Start()

    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, CentreOfMass, 0);
    }

    public override void OnEpisodeBegin()

    {
        lift.localPosition = new Vector3(0, -0.4f, 0);
        // Move the target to a new spot
        float ranNumber1 = UnityEngine.Random.Range(-5, 5);
        float ranNumber2 = UnityEngine.Random.Range(3.5f, 7);
        targetRigidBody.velocity = Vector3.zero;
        targetRigidBody.angularVelocity = Vector3.zero;
        Target.transform.rotation = Quaternion.identity;
        Target.transform.localPosition = new Vector3(ranNumber1, 0.2f, ranNumber2);
        

        // Reset position of forklift
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.transform.localPosition = new Vector3(0, 0.2f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        //sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.transform.localRotation);
        sensor.AddObservation(Target.transform.localPosition);
        sensor.AddObservation(lift.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        
        foreach (Wheel w in wheels)
        {
            var actionFloatDrive = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
            var actionFloatSteer = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
            w.Steer(actionFloatSteer);
            w.Accelerate(1000* actionFloatDrive);
            w.UpdatePosition();
        }

        var liftMove = Mathf.Clamp(actionBuffers.ContinuousActions[2], -1f, 1f);
        if (lift.localPosition.y < 1.5f && liftMove > 0.5f)
        {
            lift.localPosition += new Vector3(0f, liftMove / 30, 0f);
        }
        else if (lift.localPosition.y > -0.4f && liftMove < -0.5f)
        {
            lift.localPosition += new Vector3(0f, liftMove / 30, 0f);
        }

        //Calculate reward
        this.CalculateReward();


    }

    private void CalculateReward()
    {
        // Pallet fell off platform or fell over (bug)
        /*
        if (this.Target.transform.localPosition.y < -1.0)
        {
            this.Target.transform.localPosition += new Vector3(0f,2f,0f);
        }
        */
        //Calculate distance to target
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Fell off platform or fell over
        if (this.transform.localPosition.y < -1.0 || this.transform.localRotation.z > 45f)
        {
            SetReward(-0.2f);
            EndEpisode();
        }

        
        else if (this.StepCount > 1000)
        {
            if (this.StepCount > 100 && Target.position.y > -0.15f && distanceToTarget < 4)
            {
                float liftedHeight = Target.position.y + 0.2f;
                SetReward(liftedHeight*liftedHeight);
                EndEpisode();
            }
            else
            {
                EndEpisode();
            }
            
        }
        /*
        // Reached target

        if (distanceToTarget <= 2.5f && this.StepCount > 1000)
        {
            
            SetReward(0.6f + Target.position.y);
            EndEpisode();
        }

        else if (distanceToTarget > 2.5f && this.StepCount > 1000)
        {
            //Episode ends with
            SetReward(0.5f/distanceToTarget);
            EndEpisode();
        }

        else if (this.StepCount > 1000)
        {
            EndEpisode();
        }
        */

    }

    private void Act(float action_float1, float action_float2)
    {
        //Vector3 controlSignal = Vector3.zero;
        //controlSignal.x = action_float1;
        //controlSignal.z = action_float2;
        //rb.AddForce(controlSignal * forceMultiplier);
        //var actionZ = 100f * Mathf.Clamp(action_float1, -1f, 1f);
        //var actionX = 100f * Mathf.Clamp(action_float2, -1f, 1f);
        //rb.AddRelativeForce(Vector3.forward * (actionX));
        //rb.AddRelativeForce(Vector3.forward * (actionZ));

        //rb.AddTorque(Vector3.up * (actionZ));
    }

    //This is used to test actions and environment
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.U) && lift.localPosition.y < 1.5f)
        {
            //lift.localPosition += new Vector3(0f, 0.1f, 0f);
            continuousActionsOut[2] = 0.6f;
        }
        else if (Input.GetKey(KeyCode.D) && lift.localPosition.y >= -0.1f)
        {
            //lift.localPosition += new Vector3(0f, 0.1f, 0f);
            continuousActionsOut[2] = -0.6f;
        }
        else
        {
            continuousActionsOut[2] = 0f;
        }




    }



}
