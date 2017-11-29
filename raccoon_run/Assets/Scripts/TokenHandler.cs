﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TokenHandler : MonoBehaviour {
	public NavMeshAgent navMeshAgent;
	public List<Transform> pathSteps;
	public int playerNumber;

	private int destPoint = 0;

	// Use this for initialization
	void Start () {
		this.navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		GameObject pathObject = GameObject.Find("Pathway_Player" + this.playerNumber);
		if (pathObject)
		{
			PathwayHandler pathwayHandler = pathObject.GetComponent<PathwayHandler>();
			this.pathSteps = pathwayHandler.stops;
			CheckCurrentTarget();
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Choose the next destination point when the agent gets
		// close to the current one.
		/*if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
			CheckCurrentTarget();*/
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
			{
				ActivateClickableObject(hit.point);
			}
		}
	}

	void ActivateClickableObject(Vector3 point)
	{
		GameObject target = GetClosestGameObject(this.pathSteps.ToArray(), point);
		//target.GetComponent<IClickable>().Activate();
		Debug.Log(target);
		navMeshAgent.SetDestination(target.transform.position);
	}

	GameObject GetClosestGameObject(Transform[] otherTransforms, Vector3 point)
	{
		GameObject closestTarget = null;
		// Set the initial closest distance really high. We don't want to return null.
		float closestDistance = 1000;

		for (int i = 0; i < otherTransforms.Length; i++)
		{
			float distanceFromTarget = Vector3.Distance(point, otherTransforms[i].position);
			if (distanceFromTarget < closestDistance)
			{
				// We have a new closest target.
				closestTarget = otherTransforms[i].gameObject;
				closestDistance = distanceFromTarget;
			}
		}

		return closestTarget;
	}


	public void CheckCurrentTarget()
	{
		// Returns if no points have been set up
		if (pathSteps.Count == 0)
			return;

		// Set the agent to go to the currently selected destination.
		navMeshAgent.destination = pathSteps[destPoint].position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % pathSteps.Count;
	}
}