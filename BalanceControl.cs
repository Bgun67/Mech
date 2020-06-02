using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceControl : MonoBehaviour
{
	public enum GroundState
	{
		None,
		RightFoot,
		LeftFoot,
		Both,
	}
	public float balanceScale = 1f;
	public float forceFactor = 1000f;
	public float desiredHeight = 10f;
	Skeleton skeleton;
	public Rigidbody rb;
	public MultiAxisDriver leftFoot;
	public MultiAxisDriver rightFoot;
	public GroundState groundState;
	bool configured;

	// Start is called before the first frame update
	public void Configure()
    {
		skeleton = GetComponent<Skeleton>();

		
		rightFoot = (MultiAxisDriver)skeleton.GetBone(HumanBodyBones.RightFoot);
		leftFoot = (MultiAxisDriver)skeleton.GetBone(HumanBodyBones.LeftFoot);
		rb = skeleton.GetBone(HumanBodyBones.LeftUpperLeg).GetComponent<Rigidbody>();
		configured = true;
	}

    // Update is called once per frame
    void Update()
    {
		if (!configured)
		{
			return;
		}
		CheckGrounded();
		Vector3 delta;

		switch (groundState)
		{
			case GroundState.RightFoot:
				delta = Vector3.ProjectOnPlane(rightFoot.GetJointPos() - rb.position, rb.transform.up);
				//delta += Vector3.up * ((rightFoot.GetJointPos().y - rb.position.y) + desiredHeight);
				rb.AddForce(delta * forceFactor * balanceScale);
				rightFoot.rb.AddForce(-rightFoot.rb.velocity * forceFactor * balanceScale*10f);
				break;

			case GroundState.LeftFoot:
				delta = Vector3.ProjectOnPlane(leftFoot.GetJointPos() - rb.position, rb.transform.up);
				//delta += Vector3.up * ((leftFoot.GetJointPos().y - rb.position.y) + desiredHeight);
				rb.AddForce(delta * forceFactor * balanceScale);
				leftFoot.rb.AddForce(-leftFoot.rb.velocity * forceFactor * balanceScale*10f);

				break;

			case GroundState.Both:
				//delta = Vector3.up *2f* ((rightFoot.GetJointPos().y - rb.position.y) + desiredHeight);
				//rb.AddForce(delta * forceFactor * balanceScale);
				leftFoot.rb.AddForce(-leftFoot.rb.velocity * forceFactor * balanceScale*10f);
				rightFoot.rb.AddForce(-rightFoot.rb.velocity * forceFactor * balanceScale*10f);

				break;

			case GroundState.None:

				break;

		}

	}
	void CheckGrounded()
	{
		RaycastHit RHit;
		RaycastHit LHit;
		Physics.Raycast(rightFoot.GetJointPos(), Vector3.down, out RHit, 10f);
		Physics.Raycast(leftFoot.GetJointPos(), Vector3.down, out LHit, 10f);

		if (RHit.collider != null)
		{
			if (LHit.collider != null)
			{
				groundState = GroundState.Both;
			}
			else
			{
				groundState = GroundState.RightFoot;
			}
		}
		else if (LHit.collider != null)
		{
			groundState = GroundState.LeftFoot;
		}
		else
		{
			groundState = GroundState.None;
		}

	}
}
