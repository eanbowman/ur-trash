using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public Transform followTarget;
	private Vector3 offset;

	void Start () {
		offset = transform.position;
	}
	
	void LateUpdate () 
	{
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		transform.position = followTarget.transform.position + offset;
	}

	public void SetFollowTarget(GameObject newTarget) {
		this.followTarget = newTarget.transform;
	}
}
