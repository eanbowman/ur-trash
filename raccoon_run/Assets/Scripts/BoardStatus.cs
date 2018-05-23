using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Maps a space on the game board to a token if it resides there
public class BoardStatus : MonoBehaviour {
	public GameObject[] spaces;
	public GameObject[] tokens;

	// Use this for initialization
	void Start() {
		// Initialize a new parallel array to hold tokens
		// The same size as the game board
		// Assuming only one token can occupy a space
		tokens = new GameObject[spaces.Length];
		// When we start, ther are no game pieces on the spaces
		for (int i = 0; i < tokens.Length; i++) {
			tokens[i] = null;
		}
	}

	public bool AddToken(GameObject token, int index, int playerNumber) {
		int space = GetGameBoardSpotIndex(index, playerNumber);
		if (space < tokens.Length) {
			// Ensure the user of this class clears out
			// the space before trying to add a token
			// to it.
			if (tokens[space] == null) {
				tokens[space] = token;
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}


	/* Converts a spot integer into its space integer */
	public int GetGameBoardSpotIndex(int spot, int playerNumber) {
		spot--;
		SpotName spotName = new SpotName();
		if (playerNumber == 2) {
			if (spot < 5) {
				return spot + 14;
			}
		}
		return spot;
	}

	/* Converts a spot integer into its GameObject name */
	public SpotName GetGameBoardSpotName(int spot, int playerNumber) {
		spot--;
		SpotName spotName = new SpotName();
		spotName.text = "Token ";
		// map the difference between the pathway index and the spot on the board
		if (playerNumber == 2) {
			if (spot < 5) {
				spotName.text += (spot + 15) + " P2";
			} else {
				spotName.text += spot + " P2";
			}
		} else {
			spotName.text += spot;
		}

		return spotName;
	}

	public GameObject GetToken(int index, int playerNumber) {
		int spotIndex = GetGameBoardSpotIndex(index, playerNumber);
		if (tokens[spotIndex] != null) return tokens[spotIndex];
		return null;
	}

	public bool RemoveToken(int index, int playerNumber) {
		int space = 0;
		space = GetGameBoardSpotIndex(index, playerNumber);
		if (space < spaces.Length) {
			spaces[space] = null;
			return true;
		} else {
			return false;
		}
	}

	public bool RemoveTokenByNameFromAllSpaces(string tokenName) {
		for(int i = 0; i < tokens.Length; i++) {
			if (tokens[i] && tokens[i].name == tokenName) {
				tokens[i] = null;
				return true;
			}
		}
		return false;
	}
}

public class SpotName {
	public string text;
}
