using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
	[SerializeField] private float SpinForce;
	[SerializeField] private Transform WheelParent;

	private Rigidbody _rigidbody;

	private void Start()
	{
		_rigidbody = WheelParent.GetComponent<Rigidbody>();
		_rigidbody.AddTorque(Vector3.back * SpinForce, ForceMode.Impulse);
	}
}