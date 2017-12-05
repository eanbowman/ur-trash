using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {
	public bool hasControl = false;
	public List<Transform> tokens;
	public float hitBoxSize = 1.0f;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform)
		{
			if (child.tag == "Token")
			{
				tokens.Add(child);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(this.hasControl)
		{
			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;

				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
				{
					GameObject target = GetClosestGameObject(this.tokens.ToArray(), hit.point);
					if (target)
					{
						Debug.Log("User clicked close to " + target.name);

						// Toggle token selected state
						target.GetComponent<TokenHandler>().isSelected = !target.GetComponent<TokenHandler>().isSelected;
					}
				}
			}
		}
	}

	GameObject GetClosestGameObject(Transform[] otherTransforms, Vector3 point)
	{
		GameObject closestTarget = null;
		// Set the initial closest distance really high. We don't want to return null.
		float closestDistance = 1000;

		for (int i = 0; i < otherTransforms.Length; i++)
		{
			float distanceFromTarget = Vector3.Distance(point, otherTransforms[i].position);
			if (distanceFromTarget < closestDistance &&
				distanceFromTarget < this.hitBoxSize)
			{
				// We have a new closest target.
				closestTarget = otherTransforms[i].gameObject;
				closestDistance = distanceFromTarget;
			}
		}

		return closestTarget;
	}
}
