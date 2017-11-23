using System.Collections.Generic;
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
		if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
			CheckCurrentTarget();
	}

	void CheckCurrentTarget()
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
