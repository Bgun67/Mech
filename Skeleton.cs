using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Skeleton : MonoBehaviour
{
	public JointDriver[] bones;
	public float[] boneConfiguration = new float[16];

	public void Configure()
	{
		bones = GetComponentsInChildren<JointDriver>();

	}
	public JointDriver[] GetBones(HumanBodyBones _type)
	{
		JointDriver primaryBone = GetBone(HumanBodyBones.RightShoulder);
		JointDriver secondaryBone = GetBone(HumanBodyBones.RightShoulder);
		JointDriver tertiaryBone = GetBone(HumanBodyBones.RightShoulder);

		switch (_type)
		{
			case HumanBodyBones.RightHand:
				primaryBone = GetBone(HumanBodyBones.RightUpperArm);
				secondaryBone = GetBone(HumanBodyBones.RightLowerArm);
				tertiaryBone = GetBone(HumanBodyBones.RightHand);
				break;
			case HumanBodyBones.LeftHand:
				primaryBone = GetBone(HumanBodyBones.LeftUpperArm);
				secondaryBone = GetBone(HumanBodyBones.LeftLowerArm);
				tertiaryBone = GetBone(HumanBodyBones.LeftHand);
				break;
			case HumanBodyBones.RightFoot:
				primaryBone = GetBone(HumanBodyBones.RightUpperLeg);
				secondaryBone = GetBone(HumanBodyBones.RightLowerLeg);
				tertiaryBone = GetBone(HumanBodyBones.RightFoot);
				break;
			case HumanBodyBones.LeftFoot:
				primaryBone = GetBone(HumanBodyBones.LeftUpperLeg);
				secondaryBone = GetBone(HumanBodyBones.LeftLowerLeg);
				tertiaryBone = GetBone(HumanBodyBones.LeftFoot);
				break;
		}

		return new JointDriver[] { primaryBone, secondaryBone, tertiaryBone };
	}
	public float[] CalculateAngles(JointDriver[] bones, Vector3 relativePosition, Vector3 normal)
	{
		relativePosition = Vector3.Scale(relativePosition, new Vector3(0, 1, 1));
		Debug.DrawLine(bones[0].GetJointPos(), bones[1].GetJointPos(), Color.green);
		float a = (bones[0].GetJointPos() - bones[1].GetJointPos()).magnitude;

		Debug.DrawLine(bones[1].GetJointPos(), bones[2].GetJointPos(), Color.green);
		float b =   (bones[1].GetJointPos() - bones[2].GetJointPos()).magnitude;

		float c = Mathf.Clamp((relativePosition).magnitude, Mathf.Abs(a - b), a + b - 0.1f);

		//rotation around x delta to up
		float xRot1 = -Vector3.SignedAngle(-bones[0].transform.up, bones[0].transform.TransformVector(relativePosition), bones[0].transform.TransformDirection(normal));
		//rotation between shin and delta
		float A = Mathf.Acos((b * b + c * c - a * a) / (2f * b * c)) * Mathf.Rad2Deg;
		//rotation between Femur and delta
		float B = Mathf.Acos((a * a + c * c - b * b) / (2f * c * a)) * Mathf.Rad2Deg;
		//rotation at knee
		float C = 180f - A - B;
		//strafe angle
		float D = Vector3.SignedAngle(-bones[0].transform.up, relativePosition, transform.TransformDirection(normal));
		return new float[] { xRot1, A, B, C, D };
		
	}

	// Start is called before the first frame update

	public JointDriver GetBone(HumanBodyBones _type)
	{
		foreach (JointDriver driver in bones)
		{
			if (driver.boneType == _type)
			{
				return driver;		
			}
		}
		return null;
	}
	public void SetConfiguration(HumanBodyBones boneType, float[] angles)
	{
		int offset = GetOffset(boneType);
		boneConfiguration[0 + offset * 4] = angles[0];
		boneConfiguration[1 + offset * 4] = angles[1];
		boneConfiguration[2 + offset * 4] = angles[2];
		boneConfiguration[3 + offset * 4] = angles[3];
	}
	int GetOffset(HumanBodyBones boneType)
	{
		//4 for each limb
		//RLeg, LLeg,RArm, LArm
		int offset = 0;
		switch (boneType)
		{
			case (HumanBodyBones.RightFoot):
				offset = 0;
				break;
			case (HumanBodyBones.LeftFoot):
				offset = 1;
				break;
			case (HumanBodyBones.RightHand):
				offset = 2;
				break;
			case (HumanBodyBones.LeftHand):
				offset = 3;
				break;
		}
		return offset;
	}
	public float[] GetConfiguration(HumanBodyBones boneType)
	{
		//4 for each limb
		//RLeg, LLeg,RArm, LArm
		int offset = GetOffset(boneType);
		float[] angles = new float[4];

		angles[0] = boneConfiguration[0 + offset * 4];
		angles[1] = boneConfiguration[1 + offset * 4];
		angles[2] = boneConfiguration[2 + offset * 4];
		angles[3] = boneConfiguration[3 + offset * 4];
		return angles;
	}
}
