using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class JointDriver : MonoBehaviour
{
	public float length = 10f;
	public HumanBodyBones boneType;
	public Rigidbody rb;
	public float k = 10f;
	protected void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.ResetCenterOfMass();
	}
	void FixedUpdate()
	{
	}
	public virtual Vector3 GetJointPos()
	{
		return Vector3.zero;
	}
	public virtual Vector3 GetPrimaryAxis()
	{
		Debug.LogError("Check to make sure you didn't fuck up");
		return Vector3.zero;
	}
	public virtual Vector3 GetSecondaryAxis()
	{
		Debug.LogError("Check to make sure you didn't fuck up");
		return Vector3.zero;
	}
	public virtual void DriveJoint(float angle,JointDriveState driveState)
	{
		Debug.LogError("Warning Shoudl not be called");
	}
	public virtual void DriveJoint(Vector3 angle,JointDriveState driveState)
	{
		Debug.LogError("Warning Shoudl not be called");

	}
}
