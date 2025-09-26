using System;
using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
	[SerializeField] private float SpinForce;
	[SerializeField] private Transform WheelParent;

	private Rigidbody _rigidbody;

	public void Spin()
	{
		_rigidbody = WheelParent.GetComponent<Rigidbody>();
		_rigidbody.AddTorque(Vector3.back * SpinForce, ForceMode.Impulse);
	}
}
