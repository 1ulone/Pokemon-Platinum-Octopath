using UnityEngine;

public class OverworldCameraController : MonoBehaviour
{
	[SerializeField] private float cameraSpeed;
	[SerializeField] private Transform DefaultTarget;
	[SerializeField] private Vector3 offset;
	private Transform target;

	private void Start()
	{
		target = DefaultTarget;
	}

	private void LateUpdate()
	{
		if (target == null)
			target = DefaultTarget;

		Vector3 smoothCam = Vector3.Lerp(this.gameObject.transform.position, target.position + offset, cameraSpeed* Time.deltaTime);
		this.transform.position = smoothCam;
	}
}										 
