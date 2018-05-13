using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TokenHandler : MonoBehaviour {
	public NavMeshAgent navMeshAgent;
	public List<Transform> pathSteps;
	public int playerNumber;
	public bool isSelected;
    public bool isHighlighted;
	public GameObject activationIndicator;
	public bool winner = false;
	public float stoppingDistance = 0.5f;
    public int[] safeSpaces;

	public int destPoint = 0; // the current destination of the token
	private int nextStep = 0; // the next step toward destPoint
	private bool hasStarted = false;
	private GameController gameController;
    private bool isMoving = false;
    private Material m_Material;
    private PathwayHandler pathwayHandler;

    // Use this for initialization
    void Start () {
        isHighlighted = false;
        m_Material = GetComponent<Renderer>().material;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        GameObject pathObject = GameObject.Find("Pathway_Player" + playerNumber);
		if (pathObject)
		{
            pathwayHandler = pathObject.GetComponent<PathwayHandler>();
            pathSteps = pathwayHandler.stops;
			CheckCurrentTarget();
		}
	}
	
	// Update is called once per frame
	void Update () {
        // If this token is highlighted, show the highlight
        // requires Toon/Basic Outline or Toon/Lighted Outline shader!
        Color color = Color.green; // glow color
        float duration = 2.0f; // duration of each cycle in seconds
        if (isHighlighted == true) {
            Color c = Sinusoid(duration, 0.0f, 0.75f);
            m_Material.SetColor("_SpecColor", c);
        } else {
            Color c = Color.black;
            m_Material.SetColor("_SpecColor", c);
        }

		// Choose the next destination point when the agent gets
		// close to the current one.
		if (!hasStarted)
		{
			if (isSelected)
			{
                navMeshAgent.isStopped = false;
				destPoint = 1; // player is selected, place them at the start
				hasStarted = true;
			}
			else
			{
				destPoint = 0; // player is not selected, place them in the waiting area
			}
		}

		// If this token is selected, we can move!
		if (isSelected && gameController.hasRolled)
		{
			if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < stoppingDistance)
				CheckCurrentTarget();

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

        // If this token has reached the end, move it to the winner's area
        if (winner)
        {
            // If we've reached the winner's area, stop navigation and celebrate!
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < stoppingDistance)
            {
                navMeshAgent.isStopped = true;
                if (hasStarted || isSelected || isMoving)
                {
                    gameController.ChangeControl();
                    hasStarted = false;
                    isSelected = false;
                    isMoving = false;
                }
            }
			else
			{
				navMeshAgent.destination = pathSteps[pathSteps.Count - 1].position;
			}
		}
	}

	void ClaimSpaceOnBoard(int space) {
		pathwayHandler.SetVacancy(space);
	}

	void ActivateClickableObject(Vector3 point)
	{
		int target = GetClosestObjectID(pathSteps.ToArray(), point);
		gameController.AddStatus("User clicked close to " + target);
		//navMeshAgent.SetDestination(target.transform.position);
		int difference = target - destPoint;
		if (difference == gameController.diceValue)
		{
            isMoving = true;
            // Find all token objects
            GameObject[] tokenObjects = GameObject.FindGameObjectsWithTag("Token");
            // Check if another piece occupies that space
            GameObject otherObject = GetClosestGameObject(tokenObjects, point, 0.6f);

            if (otherObject != null || !pathwayHandler.SetVacancy(target))
            {
                gameController.AddStatus("Clicked on a token!");
                // If the other object is the player's own token, don't allow the move
                if (otherObject.GetComponent<TokenHandler>().playerNumber == playerNumber)
                {
                    gameController.AddStatus("The other token is your own. You can't move there!");
                } else
                {
                    gameController.AddStatus("The other token is the opposite player's!");
                    // Check if the other player's piece is on a safe space
                    if (otherObject.GetComponent<TokenHandler>().IsOnSafeSpace())
                    {
                        gameController.AddStatus("Opponent on safe space");
                    } else
                    {
                        gameController.AddStatus("Opponent is not safe! They are knocked back to the start.");
                        otherObject.GetComponent<TokenHandler>().KnockBack();
                        pathwayHandler.LeaveSpot(nextStep);
                        pathwayHandler.SetVacancy(destPoint);
                        destPoint = target; // we are allowed to take the space
                    }
                }
            }
            else
            {
                pathwayHandler.LeaveSpot(nextStep);
                destPoint = target;
            }
		} else
		{
			gameController.AddStatus("You can't move there!");
		}
	}

    bool IsOnSafeSpace()
    {
        bool status = false;
        foreach(int safeSpace in safeSpaces)
        {
            if (destPoint == safeSpace) status = true;
        }

        return status;
    }

    // Reset this token to the start
    public void KnockBack()
    {
        destPoint = 0;
        nextStep = 0;
        transform.position = pathSteps[nextStep].position;
        navMeshAgent.isStopped = true;
        isSelected = false;
    }

    GameObject GetClosestGameObject(GameObject[] otherTransforms, Vector3 point, float maxDistance)
    {
        GameObject closestTarget = null;
        // Set the initial closest distance really high. We don't want to return null.
        float closestDistance = 1000;

        for (int i = 0; i < otherTransforms.Length; i++)
        {
            float distanceFromTarget = Vector3.Distance(point, otherTransforms[i].transform.position);
            if (distanceFromTarget < closestDistance)
            {
                // We have a new closest target.
                closestTarget = otherTransforms[i];
                closestDistance = distanceFromTarget;
            }
        }
        if (closestDistance < maxDistance)
        {
            return closestTarget;
        }
        else
        {
            return null;
        }
    }

    int GetClosestObjectID(Transform[] otherTransforms, Vector3 point)
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

		// If the target is before the current step, 
		// or it's the end, set it to the winning circle
		if (!winner && (destPoint < nextStep || nextStep >= pathSteps.Count - 2))
		{
			//navMeshAgent.isStopped = true;
			//transform.position = pathSteps[pathSteps.Count - 1].position;
			//transform.rotation = pathSteps[pathSteps.Count - 1].rotation;
			destPoint = pathSteps.Count - 1;
			isSelected = false;
			winner = true;
			GetComponentInParent<PlayerHandler>().IncrementPoints();
		}

        // If the target is ahead, keep progressing to the next step.
        if (destPoint > nextStep)
        {
            nextStep = (nextStep + 1) % pathSteps.Count;
        }
        else if (destPoint == nextStep)
        {
            // We've reached our destination
            if (IsOnSafeSpace() && isMoving)
            {
                // and it's still our turn
                isMoving = false; // Move is over
                gameController.hasRolled = false; // Prevent the user from re-using the same roll
                gameController.AddStatus("This token is at its destination and its turn is continuing. (Safe space)");
            }
            else if(isMoving == true)
            {
                // and it's the other player's turn
                gameController.AddStatus("This token is at its destination and its turn is over");
                isMoving = false;
                isSelected = false;
                gameController.ChangeControl();
            }
        }
	}

    private Color Sinusoid(float period, float min, float max) {
        float bias = (min + max) / 2;
        float depth = (max - min) / 2;
        return new Color(depth * Mathf.Sin(2 * Mathf.PI * Time.time / period) + bias,
            depth * Mathf.Sin(2*Mathf.PI* Time.time/period)+bias,
            depth * Mathf.Sin(2 * Mathf.PI * Time.time / period) + bias);
    } 
}
