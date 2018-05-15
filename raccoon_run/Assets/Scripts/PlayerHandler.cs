using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour {
	public bool hasControl = false;
	public List<Transform> tokens;
	public float hitBoxSize = 1.0f;
	public int points = 0;
	public int maxPoints = 7;
	public int playerNumber;

	private GameController gameController;

	// Use this for initialization
	void Start () {
		this.gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		foreach (Transform child in transform)
		{
			if (child.tag == "Token")
			{
				tokens.Add(child);
			}
		}
	}

	// Update is called once per frame
	void Update() {
		if (this.hasControl)
		{
			// If player has control, highlight all pieces
			HighlightAllTokens();

			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				if (gameController.hasRolled)
				{
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
					{
						GameObject target = GetClosestGameObject(this.tokens.ToArray(), hit.point);
						if (target)
						{
							gameController.AddStatus("User clicked close to " + target.name);

							// Toggle token selected state
							if (!target.GetComponent<TokenHandler>().winner)
							{
								SetSelected(target);
								target.GetComponent<TokenHandler>().ActivateClickableObject(target.GetComponent<Transform>().position);
							}
						}
					}
				}
				else
				{
					gameController.AddStatus("Player #" + (gameController.playerNumber + 1) + " please roll the dice first!");
				}
			}
		} else {
			DeselectAllTokens();
		}
	}

	private void SetSelected(GameObject target) {
		foreach(Transform token in tokens) {
			token.GetComponent<TokenHandler>().isSelected = false;
		}
		target.GetComponent<TokenHandler>().isSelected = true;
	}

	public void IncrementPoints()
	{
		this.points++;
	}

	private void HighlightAllTokens()
	{
		// Reset selection of all pieces
		foreach (Transform token in this.tokens)
		{
			token.GetComponent<TokenHandler>().isHighlighted = true;
		}
	}

	private void DeselectAllTokens()
	{
		// Reset selection of all pieces
		foreach (Transform token in this.tokens)
		{
			token.GetComponent<TokenHandler>().isSelected = false;
			token.GetComponent<TokenHandler>().isHighlighted = false;
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
