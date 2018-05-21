using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStatus : MonoBehaviour {
	public GameObject[] spaces;

	// Use this for initialization
	void Start() {
		// When we start, ther are no game pieces on the spaces
		for (int i = 0; i < spaces.Length; i++) {
			spaces[i] = null;
		}
	}

	public bool AddToken(GameObject token, int space) {
		if (space < spaces.Length) {
			spaces[space] = token;
			return true;
		} else {
			return false;
		}
	}

	public bool RemoveToken(int space) {
		if (space < spaces.Length) {
			spaces[space] = null;
			return true;
		} else {
			return false;
		}
	}
}
