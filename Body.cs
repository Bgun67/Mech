using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum JointDriveState
{
	Position,
	Velocity,
}

public class Body : MonoBehaviourPunCallbacks, IPunObservable
{
	public Vector3 rightLegPos = -Vector3.one;
	public Vector3 leftLegPos = -Vector3.one;
	public Vector3 rightArmPos = -Vector3.one;
	public Vector3 leftArmPos = -Vector3.one;

	public float boostPower = 1000f;

	public Input_Module operator1;
	public Input_Module operator2;
	Skeleton skeleton;
	Booster[] boosters;
	public bool umbilicalAttached = true;
	LineRenderer[] lines = new LineRenderer[2];

	void Start(){
		skeleton = gameObject.AddComponent<Skeleton>();
		skeleton.Configure();
		gameObject.AddComponent<BalanceControl>().Configure();
		boosters = FindObjectsOfType<Booster>();
		lines = GetComponentsInChildren<LineRenderer>();
		
		if (!photonView.IsMine)
		{
			GetComponentInChildren<Camera>().gameObject.SetActive(false);
		}
	}
	void Update()
	{
		
		if (photonView.IsMine)
		{
			GetInput();
		}
		
	}
	void GetInput()
	{
		if (Input.GetKeyDown("space"))
		{
			if (!umbilicalAttached)
			{
				Application.LoadLevel(Application.loadedLevel);

			}
			else
			{
				umbilicalAttached = false;
			}
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			foreach (Booster booster in boosters)
			{
				booster.Boost();
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			Input_Module temp1 = operator1;
			operator1 = operator2;
			operator2 = temp1;
		}
		if (operator1 != null)
		{
			Vector3 Rinput = operator1.GetRightMove();
			rightLegPos += new Vector3(0f,Rinput.y, Rinput.z);
			rightLegPos = Vector3.ClampMagnitude(rightLegPos, 50f);
			rightLegPos = new Vector3(Rinput.x*20f, Mathf.Clamp(rightLegPos.y, -100f, 0f),rightLegPos.z);
			//rightLegPos = Vector3.ClampMagnitude(rightLegPos, 50f);
			Vector3 Linput = operator1.GetLeftMove();
			leftLegPos += new Vector3(0f,Linput.y, Linput.z);
			leftLegPos = Vector3.ClampMagnitude(leftLegPos, 50f);
			leftLegPos = new Vector3(Linput.x*20f, Mathf.Clamp(leftLegPos.y, -100f, 0f),leftLegPos.z);
			//leftLegPos = Vector3.ClampMagnitude(leftLegPos, 50f);
		}
		if (operator2 != null)
		{
			Vector3 Rinput = operator2.GetRightMove();
			rightArmPos += new Vector3(0f,Rinput.y, Rinput.z);
			rightArmPos = Vector3.ClampMagnitude(rightArmPos, 50f);
			rightArmPos = new Vector3(Rinput.x*20f, rightArmPos.y,rightArmPos.z);

			Vector3 Linput = operator2.GetLeftMove();
			leftArmPos += new Vector3(0f,Linput.y, Linput.z);
			leftArmPos = Vector3.ClampMagnitude(leftArmPos, 50f);
			leftArmPos = new Vector3(Linput.x*20f, leftArmPos.y,leftArmPos.z);
		}
	}
	void FixedUpdate()
	{
		if (!umbilicalAttached)
		{
			JointDriver hip = skeleton.GetBone(HumanBodyBones.LeftUpperLeg);
			hip.rb.isKinematic = false;
		}

		MoveFoot(HumanBodyBones.LeftFoot, leftLegPos);
		MoveFoot(HumanBodyBones.RightFoot, rightLegPos);
		MoveHand(HumanBodyBones.LeftHand, leftArmPos);
		MoveHand(HumanBodyBones.RightHand, rightArmPos);

	}
	void Boost()
	{

	}
	void MoveFoot(HumanBodyBones _type, Vector3 relativePosition)
	{
		JointDriver[] bones = skeleton.GetBones(_type);
		float[] xAngles;
		if (photonView.IsMine)
		{
			xAngles = skeleton.CalculateAngles(bones, relativePosition, bones[0].GetPrimaryAxis());
			skeleton.SetConfiguration(_type, xAngles);
		}
		else
		{
			xAngles = skeleton.GetConfiguration(_type);
		}

		bones[0].rb.MoveRotation(bones[0].rb.rotation * Quaternion.Euler(0, relativePosition.x * 0.01f, 0));

		bones[0].DriveJoint(new Vector3(-xAngles[0] - xAngles[2], relativePosition.x, 0f), JointDriveState.Position);
		bones[1].DriveJoint(new Vector3(-xAngles[3] + 180f, 0f), JointDriveState.Position);
		bones[2].DriveJoint(new Vector3(-xAngles[1] + xAngles[0], 0f), JointDriveState.Position);


		Debug.DrawLine(bones[0].GetJointPos(), bones[0].GetJointPos() + bones[0].transform.TransformVector(relativePosition), Color.red);
		if (_type == HumanBodyBones.LeftFoot)
		{
			lines[0].SetPosition(0, bones[0].GetJointPos());
			lines[0].SetPosition(1, bones[0].GetJointPos() + bones[0].transform.TransformVector(relativePosition));
		}
		else
		{
			lines[1].SetPosition(0, bones[0].GetJointPos());
			lines[1].SetPosition(1, bones[0].GetJointPos() + bones[0].transform.TransformVector(relativePosition));
		}
	}
	void MoveHand(HumanBodyBones _type, Vector3 relativePosition)
	{
		JointDriver[] bones = skeleton.GetBones(_type);
		float[] xAngles;
		if (photonView.IsMine)
		{
			xAngles = skeleton.CalculateAngles(bones, relativePosition, bones[0].GetPrimaryAxis());
			skeleton.SetConfiguration(_type, xAngles);
		}
		else
		{
			xAngles = skeleton.GetConfiguration(_type);
		}

		bones[0].rb.MoveRotation(bones[0].rb.rotation*Quaternion.Euler(0, relativePosition.x*0.01f, 0));
		
		bones[0].DriveJoint(new Vector3(xAngles[0] - xAngles[2],relativePosition.x,0f), JointDriveState.Position);
		bones[1].DriveJoint(new Vector3(+xAngles[3]-180f,0f,0f), JointDriveState.Position);
		bones[2].DriveJoint(new Vector3(xAngles[1] +xAngles[0]-90f,0f,0f), JointDriveState.Position);

		Debug.DrawLine(bones[0].GetJointPos(), bones[0].GetJointPos() + bones[0].transform.TransformVector(relativePosition), Color.red);
	}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.skeleton.boneConfiguration);
                stream.SendNext(this.umbilicalAttached);
            }
            else
            {
                // Network player, receive data
				this.skeleton.boneConfiguration = (float[])stream.ReceiveNext();
                this.umbilicalAttached = (bool)stream.ReceiveNext();
            }
        }
	
	
	
	

}
