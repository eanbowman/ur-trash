using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceButtonHandler : MonoBehaviour {
	public void RollTheDie()
	{
		GameObject gameController = GameObject.FindGameObjectsWithTag("GameBoard")[0];
		gameController.GetComponent<GameController>().RollDice();
	}
}
