using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PhysicInfo : MonoBehaviour
{
    private Vector3 positionTMinus1;
    private Quaternion rotationTMinus1;

    private Vector3 velocityTMinus3Half;
    private Vector3 velocityTMinusHalf;
    private Vector3 angularVelocityTMinus3half;
    private Vector3 angularVelocityTMinusHalf;

    private Vector3 accelerationTMinus1;
    private Vector3 angularAccelerationTMinus1;

    private float dTMinus1;


    public float samplingDuration = 0.1f;

    private List<Vector3> positions;
    private List<Vector3> velocities;

    private bool isVelocityCalculated;
    private Vector3 calculatedVelocity;
    public Vector3 Velocity
    {
        get
        {
            if (!isVelocityCalculated)
            {
                if (velocities.Count < 2)
                {
                    calculatedVelocity = Vector3.zero;
                }
                else
                {
                    calculatedVelocity = Vector3.zero;

                    foreach (Vector3 velocity in velocities)
                    {
                        calculatedVelocity += velocity;
                    }

                    calculatedVelocity /= velocities.Count;

                    isVelocityCalculated = true;
                }
            }

            return calculatedVelocity;
        }
    }

    private bool isOld = false;
    
    #region Save properties

    private Vector3 savedPositionTMinus1;
    private Quaternion savedRotationTMinus1;

    private Vector3 savedVelocityTMinus3Half;
    private Vector3 savedVelocityTMinusHalf;
    private Vector3 savedAngularVelocityTMinus3half;
    private Vector3 savedAngularVelocityTMinusHalf;

    private Vector3 savedAccelerationTMinus1;
    private Vector3 savedAngularAccelerationTMinus1;

    private float savedDTMinus1;

    #endregion

    private void Start()
    {
        positionTMinus1 = transform.position;

        velocityTMinus3Half = Vector3.zero;
        velocityTMinusHalf = Vector3.zero;
        angularVelocityTMinus3half = Vector3.zero;
        angularVelocityTMinusHalf = Vector3.zero;

        accelerationTMinus1 = Vector3.zero;
        angularAccelerationTMinus1 = Vector3.zero;

        rotationTMinus1 = transform.rotation;

        dTMinus1 = 1;

        positions = new List<Vector3>();
        velocities = new List<Vector3>();

        isVelocityCalculated = true;
    }

    void FixedUpdate()
    {
        if (!BallManager.instance.IsBallPaused)
        {
            velocityTMinusHalf = CalculateVelocity(transform.position, positionTMinus1, Time.fixedDeltaTime);
            angularVelocityTMinusHalf = CalculateAngularVelocity(transform.rotation, rotationTMinus1, Time.fixedDeltaTime);

            accelerationTMinus1 = CalculateAcceleration(velocityTMinusHalf, velocityTMinus3Half, Time.fixedDeltaTime, dTMinus1);
            angularAccelerationTMinus1 = CalculateAcceleration(angularVelocityTMinusHalf, angularVelocityTMinus3half, Time.fixedDeltaTime, dTMinus1);

            positionTMinus1 = transform.position;
            rotationTMinus1 = transform.rotation;

            velocityTMinus3Half = velocityTMinusHalf;
            angularVelocityTMinus3half = angularVelocityTMinusHalf;

            dTMinus1 = Time.fixedDeltaTime;


            isVelocityCalculated = false;

            AddPosition(transform.position);
        }
        else
            ResetValues();
    }

    private void AddPosition(Vector3 newPosition)
    {
        bool isBufferFull = false;

        positions.Add(newPosition);
        if (positions.Count * Time.fixedDeltaTime > samplingDuration)
        {
            positions.RemoveAt(0);
            isBufferFull = true;
        }

        UpdateVelocities(isBufferFull);
    }

    private void UpdateVelocities(bool shouldRemoveFirstElement)
    {
        if(positions.Count > 1)
        {
            velocities.Add( CalculateVelocity(positions[positions.Count - 1], positions[positions.Count - 2], Time.fixedDeltaTime) );
            if (shouldRemoveFirstElement)
                velocities.RemoveAt(0);
        }
    }

    private void ResetValues()
    {
        positions.Clear();
        velocities.Clear();
    }

    #region Getter

    private Vector3 GetPosition()
    {
        return transform.position;
    }

    public Quaternion GetRotation()             // Prendre en compte la rotation de base de la raquette dans la main du joueur
    {
        return transform.rotation;
    }

    public Vector3 GetVelocity()
    {
        if (isOld)
            return velocityTMinusHalf;
        else
            return Velocity;
    }

    public Vector3 GetAngularVelocity()         // Prendre en compte la rotation de base de la raquette dans la main du joueur
    {
        return angularVelocityTMinusHalf;
    }

    public Vector3 GetAcceleration()
    {
        return accelerationTMinus1;
    }

    public Vector3 GetAngularAcceleration()
    {
        return angularAccelerationTMinus1;
    }

    #endregion

    #region Calculation Methods

    private Vector3 CalculateVelocity(Vector3 currentPosition, Vector3 previousPosition, float deltaTime)
    {
        return (currentPosition - previousPosition) / deltaTime;
    }

    private Vector3 CalculateAngularVelocity(Quaternion currentRotation, Quaternion lastRotation, float deltaTime)      // Trouver la bonne formule...
    {
        return Vector3.zero;
    }

    private Vector3 CalculateAcceleration(Vector3 velocity2, Vector3 velocity1, float deltaTime2, float deltaTime1)
    {
        return (velocity2 - velocity1) / ((deltaTime2 + deltaTime1) / 2);
    }

    #endregion

    public void SaveCurrentState()
    {
        savedPositionTMinus1 = positionTMinus1;
        savedRotationTMinus1 = rotationTMinus1;

        savedVelocityTMinus3Half = velocityTMinus3Half;
        savedVelocityTMinusHalf = velocityTMinusHalf;
        savedAngularVelocityTMinus3half = angularVelocityTMinus3half;
        savedAngularVelocityTMinusHalf = angularVelocityTMinusHalf;

        savedAccelerationTMinus1 = accelerationTMinus1;
        savedAngularAccelerationTMinus1 = angularAccelerationTMinus1;

        savedDTMinus1 = dTMinus1;
    }

    public void RestoreSavedState()
    {
        positionTMinus1 = savedPositionTMinus1;
        rotationTMinus1 = savedRotationTMinus1;

        velocityTMinus3Half = savedVelocityTMinus3Half;
        velocityTMinusHalf = savedVelocityTMinusHalf;
        angularVelocityTMinus3half = savedAngularVelocityTMinus3half;
        angularVelocityTMinusHalf = savedAngularVelocityTMinusHalf;

        accelerationTMinus1 = savedAccelerationTMinus1;
        angularAccelerationTMinus1 = savedAngularAccelerationTMinus1;

        dTMinus1 = savedDTMinus1;
    }
}
