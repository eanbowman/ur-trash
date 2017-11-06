using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAgainHandler : MonoBehaviour {
	public GameController gameController;

	// Use this for initialization
	void Start () {
		Debug.Log("PlayAgainHandler");
	}

	public void PlayAgainClicked()
	{
		Debug.Log("Play Again!");
		gameController.ResetGame();
	}
}
