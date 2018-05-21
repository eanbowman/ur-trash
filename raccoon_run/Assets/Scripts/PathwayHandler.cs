using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathwayHandler : MonoBehaviour {
	public int playerNumber;
	public List<Transform> stops;
	public List<int> safeSpaces;
	public List<int> sharedSpaces;
	public GameObject[] occupancy;

	private GameObject gameBoardObject;
	private BoardStatus boardStatus;

	void Start()
	{
		gameBoardObject = GameObject.FindGameObjectsWithTag("GameBoard")[0];
		boardStatus = gameBoardObject.GetComponent<BoardStatus>();
	}

	public GameObject GetOccupancy(int spot)
	{
		GameObject occupant = boardStatus.GetToken(spot, playerNumber);
		return occupant;
	}

	public bool SetOccupancy(int spot, GameObject token)
	{
		// Remove occupancy from other spots
		boardStatus.RemoveTokenByNameFromAllSpaces(token.name);
		return boardStatus.AddToken(token, spot, playerNumber);
	}

	public void LeaveSpot(GameObject token)
	{
		boardStatus.RemoveTokenByNameFromAllSpaces(token.name);
	}
}
