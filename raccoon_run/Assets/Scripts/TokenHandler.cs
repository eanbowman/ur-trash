using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TokenHandler : MonoBehaviour {
	public NavMeshAgent navMeshAgent;
	public List<Transform> pathSteps;
	public int playerNumber;
	public bool isSelected;
	public GameObject activationIndicator;

	private int destPoint = 0; // the current destination of the token
	private int nextStep = 0; // the next step toward destPoint
	private string gameState;
	private GameObject gameController;

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
		gameController = GameObject.FindGameObjectsWithTag("GameController")[0];
	}
	
	// Update is called once per frame
	void Update () {
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
			CheckCurrentTarget();
		if (this.isSelected)
		{
			activationIndicator.SetActive(true);
			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;

				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
				{
					ActivateClickableObject(hit.point);
				}
			}
		} else
		{
			activationIndicator.SetActive(false);
		}
	}

	void ActivateClickableObject(Vector3 point)
	{
		int target = GetClosestGameObject(this.pathSteps.ToArray(), point);
		Debug.Log("User clicked close to " + target);
		//navMeshAgent.SetDestination(target.transform.position);
		int difference = target - destPoint;
		if (difference <= gameController.GetComponent<GameController>().diceValue)
		{
			destPoint = target;
		} else
		{
			Debug.Log("You can't move there!");
		}
	}

	int GetClosestGameObject(Transform[] otherTransforms, Vector3 point)
	{
		int closestTarget = -1;
		// Set the initial closest distance really high. We don't want to return null.
		float closestDistance = 1000;

		for (int i = 0; i < otherTransforms.Length; i++)
		{
			float distanceFromTarget = Vector3.Distance(point, otherTransforms[i].position);
			if (distanceFromTarget < closestDistance)
			{
				// We have a new closest target.
				closestTarget = i; // otherTransforms[i].gameObject;
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
		navMeshAgent.destination = pathSteps[nextStep].position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		// destPoint = (destPoint + 1) % pathSteps.Count;
		if (destPoint > nextStep) nextStep = (nextStep + 1) % pathSteps.Count;
	}
}
