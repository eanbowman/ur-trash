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
	public PathwayHandler pathwayHandler;

	public int targetBoardSpace = 0; // the current destination of the token
	private int nextStep = 0; // the next step toward targetBoardSpace
	private bool hasStarted = false;
	private GameController gameController;
	private bool isMoving = false;
	private Material m_Material;

	// Use this for initialization
	void Start() {
		isHighlighted = false;
		m_Material = GetComponent<Renderer>().material;
		navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		GameObject pathObject = GameObject.Find("Pathway_Player" + playerNumber);
		if (pathObject) {
			pathwayHandler = pathObject.GetComponent<PathwayHandler>();
			pathSteps = pathwayHandler.stops;
			CheckCurrentTarget();
		}
	}

	// Update is called once per frame
	void Update() {
		// If this token is highlighted, show the highlight
		// requires Toon/Basic Outline or Toon/Lighted Outline shader!
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
		if (!hasStarted) {
			if (isSelected) {
				navMeshAgent.isStopped = false;
				//targetBoardSpace = 1; // player is selected, place them at the start
				hasStarted = true;
			} else {
				targetBoardSpace = 0; // player is not selected, place them in the waiting area
			}
		}

		// If this token is selected, we can move!
		if (isSelected && gameController.hasRolled) {
			if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < stoppingDistance)
				CheckCurrentTarget();

			activationIndicator.SetActive(true);
		} else {
			activationIndicator.SetActive(false);
		}

		// If this token has reached the end, move it to the winner's area
		if (winner) {
			// If we've reached the winner's area, stop navigation and celebrate!
			if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < stoppingDistance) {
				navMeshAgent.isStopped = true;
				if (hasStarted || isSelected || isMoving) {
					gameController.ChangeControl();
					hasStarted = false;
					isSelected = false;
					isMoving = false;
				}
			} else {
				navMeshAgent.destination = pathSteps[pathSteps.Count - 1].position;
			}
		}
	}

	void ClaimSpaceOnBoard(int space) {
		GameObject[] pathwayHandlers = GameObject.FindGameObjectsWithTag("Pathway");
		foreach (GameObject path in pathwayHandlers)
		{
			path.GetComponent<PathwayHandler>().SetOccupancy(space, this.GetComponent<GameObject>());
		}
	}

	public void ActivateClickableObject(Vector3 point) {
		// New strategy: player can only click on their own pieces!
		// If a piece can move to the space represented by the
		// number of spaces ahead on the die, then the new destination
		// is set and the player's token starts moving.

		int diceRoll = gameController.diceValue;
		int nextBoardSpace = targetBoardSpace + diceRoll;

		// Exclude the winner's circle, and check that the nextBoardSpace
		// is inside of the movable spaces on the board. Player can not
		// move a token a partial roll ahead.
		// Extra spaces (3):
		//	start space (0)
		//	end space (15)
		//	winner's circle (16)
		// Last valid space is 15.
		if (nextBoardSpace > pathSteps.Count - 2) {
			gameController.AddStatus("Token does not have that many spaces left to move. Can not move a partial roll ahead.");
			return;
		}

		// Find all token objects
		GameObject[] tokenObjects = GameObject.FindGameObjectsWithTag("Token");
		// Check if another piece occupies that space
		GameObject clickedObject = GetClosestGameObject(tokenObjects, point, 0.6f);
		// Check if the space diceRoll spaces ahead contains a Token
		GameObject occupantObject = pathwayHandler.GetOccupancy(nextBoardSpace);

		if(HasAValidMove(diceRoll))
		{
			if (IsAValidMove(diceRoll))
			{
				// We can move there, so move there!
				navMeshAgent.isStopped = false;
				pathwayHandler.LeaveSpot(this.gameObject);
				targetBoardSpace = nextBoardSpace;
				isMoving = true;
				pathwayHandler.SetOccupancy(targetBoardSpace, gameObject);
				gameController.AddStatus("Player " + playerNumber + " moved ahead " + diceRoll + " spaces.");
				if (occupantObject && occupantObject.GetComponent<TokenHandler>().playerNumber != playerNumber)
				{
					occupantObject.GetComponent<TokenHandler>().KnockBack();
					gameController.AddStatus("Player " + occupantObject.GetComponent<TokenHandler>().playerNumber + " was knocked back.");
				}
				return;
			} else
			{
				gameController.AddStatus("That is not a valid move, but you have other valid moves!");
			}
		} else
		{
			gameController.ChangeControl();
			gameController.AddStatus("Player " + playerNumber + " has no valid moves for " + diceRoll + " spaces.");
		}
	}

	public bool HasAValidMove(int roll)
	{
		GameObject player = GameObject.Find("Player " + playerNumber + " Tokens");
		bool hasAValidMove = false;
		foreach(Transform token in player.GetComponent<PlayerHandler>().tokens)
		{
			if (token.GetComponent<TokenHandler>().IsAValidMove(roll)) hasAValidMove = true;
		}
		return hasAValidMove;
	}

	public bool IsAValidMove(int roll)
	{
		int nextBoardSpace = targetBoardSpace + roll;

		// Exclude the winner's circle, and check that the nextBoardSpace
		// is inside of the movable spaces on the board. Player can not
		// move a token a partial roll ahead.
		// Extra spaces (3):
		//	start space (0)
		//	end space (15)
		//	winner's circle (16)
		// Last valid space is 15.
		if (nextBoardSpace > pathSteps.Count - 2)
		{
			return false;
		}

		// Check if the space diceRoll spaces ahead contains a Token
		GameObject occupantObject = pathwayHandler.GetOccupancy(nextBoardSpace);

		// If the spot is empty, that's a valid move
		if (occupantObject == null)
		{
			return true;
		} else
		{
			if(occupantObject.GetComponent<TokenHandler>().playerNumber != playerNumber)
			{
				// If the target space is occupied by the other player...
				if(occupantObject.GetComponent<TokenHandler>().IsOnSafeSpace() == false)
				{
					// and they're not on a safe space, it's a valid move.
					return true;
				}
			}
		}

		return false;
	}

	bool IsOnSafeSpace() {
		bool status = false;

		foreach (int safeSpace in pathwayHandler.safeSpaces) {
			if (targetBoardSpace == safeSpace) status = true;
		}

		return status;
	}

	// Reset this token to the start
	public void KnockBack() {
		targetBoardSpace = 1;
		pathwayHandler.LeaveSpot(this.gameObject);
		nextStep = 1;
		transform.position = pathSteps[nextStep].position;
		navMeshAgent.isStopped = true;
		isSelected = false;
		gameController.AddStatus("Token " + this.name + " was knocked back!");
	}

	GameObject GetClosestGameObject(GameObject[] otherTransforms, Vector3 point, float maxDistance) {
		GameObject closestTarget = null;
		// Set the initial closest distance really high. We don't want to return null.
		float closestDistance = 1000;

		for (int i = 0; i < otherTransforms.Length; i++) {
			float distanceFromTarget = Vector3.Distance(point, otherTransforms[i].transform.position);
			if (distanceFromTarget < closestDistance) {
				// We have a new closest target.
				closestTarget = otherTransforms[i];
				closestDistance = distanceFromTarget;
			}
		}
		if (closestDistance < maxDistance) {
			return closestTarget;
		} else {
			return null;
		}
	}

	int GetClosestObjectID(Transform[] otherTransforms, Vector3 point) {
		int closestTarget = -1;
		// Set the initial closest distance really high. We don't want to return null.
		float closestDistance = 1000;

		for (int i = 0; i < otherTransforms.Length; i++) {
			float distanceFromTarget = Vector3.Distance(point, otherTransforms[i].position);
			if (distanceFromTarget < closestDistance) {
				// We have a new closest target.
				closestTarget = i; // otherTransforms[i].gameObject;
				closestDistance = distanceFromTarget;
			}
		}

		return closestTarget;
	}


	public void CheckCurrentTarget() {
		// Returns if no points have been set up
		if (pathSteps.Count == 0)
			return;

		// Set the agent to go to the currently selected destination.
		navMeshAgent.destination = pathSteps[nextStep].position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		// targetBoardSpace = (targetBoardSpace + 1) % pathSteps.Count;

		// If the target is before the current step, 
		// or it's the end, set it to the winning circle
		if (!winner && nextStep >= pathSteps.Count - 2) {
			targetBoardSpace = pathSteps.Count - 1;
			isSelected = false;
			winner = true;
			GetComponentInParent<PlayerHandler>().IncrementPoints();
		}

		// If the target is ahead, keep progressing to the next step.
		if (targetBoardSpace > nextStep) {
			nextStep = (nextStep + 1) % pathSteps.Count;
		} else if (targetBoardSpace == nextStep) {
			// We've reached our destination
			if (IsOnSafeSpace() && isMoving) {
				// and it's still our turn
				isMoving = false; // Move is over
				gameController.hasRolled = false; // Prevent the user from re-using the same roll
				gameController.AddStatus("This token is at its destination and its turn is continuing. (Safe space)");
			} else if (isMoving == true) {
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
			depth * Mathf.Sin(2 * Mathf.PI * Time.time / period) + bias,
			depth * Mathf.Sin(2 * Mathf.PI * Time.time / period) + bias);
	}
}
