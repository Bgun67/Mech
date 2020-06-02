using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAxisDriver : JointDriver
{
    public ConfigurableJoint joint;
	public Vector3 motorSpeed;
	public Vector3 targetAngle;

	void Reset()
	{
		joint = GetComponent<ConfigurableJoint>();

	}
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.ResetCenterOfMass();
	}
	public override Vector3 GetJointPos()
	{
		return transform.TransformPoint(joint.anchor);
	}
	public override Vector3 GetPrimaryAxis()
	{
		return (joint.axis);
	}
	public override Vector3 GetSecondaryAxis()
	{
		return (joint.secondaryAxis);
	}
	public override void DriveJoint(Vector3 input, JointDriveState driveState)
	{
		if (!this.enabled)
		{
			input = Vector3.zero;
		}
		targetAngle = input;

		joint.targetRotation = (Quaternion.Euler(input));
	}
}
