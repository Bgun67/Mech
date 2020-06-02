using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleAxisDriver : JointDriver
{
	public HingeJoint joint;
	public float motorSpeed;
	public float targetAngle;

	void Reset()
	{
		joint = GetComponent<HingeJoint>();
	}
	public override Vector3 GetJointPos()
	{
		return transform.TransformPoint(joint.anchor);
	}
	public override void DriveJoint(float input, JointDriveState driveState)
	{

		JointMotor motor = joint.motor;
		if (driveState == JointDriveState.Position)
		{

			motorSpeed = k * ((input - joint.angle) - joint.velocity);
		}
		else if (driveState == JointDriveState.Velocity)
		{
			motorSpeed = input;
		}
		targetAngle = input;
		motor.targetVelocity = motorSpeed;
		joint.motor = motor;
	}
}
