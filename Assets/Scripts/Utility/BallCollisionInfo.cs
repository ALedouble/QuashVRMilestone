using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BallCollisionInfo
{
    public string OtherTag { get; private set; }
    public Vector3 ContactPoint { get; private set; }
    public Vector3 ContactNormal { get; private set; }
    public Vector3 BallVelocity { get; private set; }

    /// <summary>
    /// Creates a BallCollisioInfo class that contains a large amount of data about the collision of the ball with another object
    /// </summary>
    /// <param name="p_otherTag"> the tag of the other object</param>
    /// <param name="p_contactPoint">the contactPoint</param>
    /// <param name="p_contactNormal">the normal vector of the contact point</param>
    /// <param name="p_ballVelocity">the velocity of the ball when the ball collides with another thing</param>
    public BallCollisionInfo(string p_otherTag, Vector3 p_contactPoint, Vector3 p_contactNormal, Vector3 p_ballVelocity)
    {
        OtherTag = p_otherTag;
        ContactPoint = p_contactPoint;
        ContactNormal = p_contactNormal;
        BallVelocity = p_ballVelocity;
    }
}
