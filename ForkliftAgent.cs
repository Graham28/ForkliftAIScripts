using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class ForkliftAgent : Agent
{   
    private Rigidbody rb;
    private float reward;
    public float forceMultiplier = 10;
    public Transform Target;

    void Start()

    {
        rb = GetComponent<Rigidbody>(); 
    }

    public override void OnEpisodeBegin()

    {

        // Move the target to a new spot
        float ranNumber1 = UnityEngine.Random.Range(-10,10);
        float ranNumber2 = UnityEngine.Random.Range(-10,10);
        Target.localPosition = new Vector3(ranNumber1, 0.1f, ranNumber2);

        // Reset position of forklift
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //this.transform.rotation = Quaternion.identity;
        //this.transform.localPosition = new Vector3(-ranNumber1, 0.2f, -ranNumber2);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers) 
    {
        //Take actions
        this.Act(actionBuffers.ContinuousActions[0], actionBuffers.ContinuousActions[1]);


        //Calculate reward
        this.CalculateReward();
        
        
    }

    private void CalculateReward()
    {
        //Calculate distance to target
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 2f)
        {

            float ranNumber1 = UnityEngine.Random.Range(-20, 20);
            float ranNumber2 = UnityEngine.Random.Range(-20, 20);
            Target.localPosition = new Vector3(ranNumber1, 0.1f, ranNumber2);
            reward += 0.5f;

            if (reward > 0.6f)
            {
                SetReward(1.0f);
                EndEpisode();
            }
        }

        if (this.StepCount > 5000)
        {
            if (reward > 0.6f)
            {
                SetReward(1.0f);
            }
            else
            {
                SetReward(reward);
            }
            EndEpisode();
        }

        // Fell off platform or jumped in the air
        if (this.transform.localPosition.y < -1.0 ^ this.transform.localPosition.y > 0.7)
        {
            Console.WriteLine("Fell off Platform");
            SetReward(-0.5f);
            EndEpisode();
        }
    }

    private void Act(float action_float1, float action_float2)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = action_float1;
        controlSignal.z = action_float2;
        rb.AddForce(controlSignal * forceMultiplier);
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
        }

    

}
