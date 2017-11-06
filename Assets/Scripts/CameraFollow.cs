using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public Transform followTarget;
	private Vector3 offset;
	private Quaternion rotation;


	public void MoveTo(Transform target)
	{
		if (target)
		{
			transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * 100);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 100);
		}
	}

	void Start () {
		offset = transform.position;
		rotation = transform.rotation;
	}
	
	void LateUpdate () 
	{
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		MoveTo(followTarget);
	}

	public void SetFollowTarget(GameObject newTarget) {
		this.followTarget = newTarget.transform;
	}
}
