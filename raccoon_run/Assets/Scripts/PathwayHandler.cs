using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathwayHandler : MonoBehaviour {
	public int playerNumber;
	public List<Transform> stops;
	public List<int> safeSpaces;
	public List<int> sharedSpaces;

	public GameObject[] occupancy;

	void Start()
	{
		occupancy = new GameObject[stops.Count];
		for(int i = 0; i < stops.Count; i++)
		{
			occupancy[i] = null; // it's vacant
		}
	}

	public GameObject GetOccupancy(int spot)
	{
		if (spot < occupancy.Length)
		{
			return occupancy[spot];
		}
		throw new System.Exception("Invalid occupancy slot selected!");
	}

	public bool SetOccupancy(int spot, GameObject token)
	{
		bool sharedSpace = false;
		foreach(int space in sharedSpaces) {
			if(spot == space) {
				sharedSpace = true;
			}
		}

		// Remove occupancy from other spots
		for(int i = 0; i < occupancy.Length; i++)
		{
			if(occupancy[i] == token)
			{
				occupancy[i] = null;
			}
		}

		// If this is our own space, set our own occupancy value
		// Don't count the end spot or the winner's circle
		if (spot < occupancy.Length - 2 && occupancy[spot] == null && sharedSpace)
		{
			occupancy[spot] = token;
			GameObject[] pathways = GameObject.FindGameObjectsWithTag("Pathway");
			foreach(GameObject path in pathways) {
				if(path.GetComponent<PathwayHandler>().GetOccupancy(spot) == null) {
					path.GetComponent<PathwayHandler>().SetOccupancy(spot, token);
				}
			}
			return true;
		} else if (spot < occupancy.Length && occupancy[spot] == null) {
			occupancy[spot] = token;
			return true;
		}
		return false;
	}

	public void LeaveSpot(GameObject token)
	{
		for(int i = 0; i < stops.Count; i++) {
			if (stops[i].GetComponent<GameObject>() == token) stops[i] = null;
		}
		// If this time we are looking at token that is the player's own, run this additional check
		// otherwise don't, or we'll blow the stack up with infinite checks :D
		if (token.GetComponent<TokenHandler>().playerNumber == playerNumber)
		{
			// Update other player's path list too in case it contains a reference to this token
			// No more ghost tokens! boo!
			int otherPlayerNumber = 1;
			if (token.GetComponent<TokenHandler>().playerNumber == 1) { otherPlayerNumber = 2; }
			GameObject player = GameObject.Find("Player " + otherPlayerNumber + " Tokens");
			// this convoluted mess finds the other player's first token's pathway handler and calls its copy of LeaveSpot
			player.GetComponent<PlayerHandler>().tokens[0].GetComponent<TokenHandler>().pathwayHandler.LeaveSpot(token);
		}
	}
}
