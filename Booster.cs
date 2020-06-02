using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
	public Transform boostVector;
	[HideInInspector]
	public float boosterPower = 1000f;
	public Rigidbody rb;
	void Start()
	{
		boosterPower = FindObjectOfType<Body>().boostPower;
		rb = GetComponentInParent<Rigidbody>();
	}
	public void Boost()
	{
		rb.AddForceAtPosition(boostVector.forward * boosterPower, boostVector.position);
	}
}
