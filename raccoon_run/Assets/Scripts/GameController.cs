using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public string gameState = "Rolling"; // the first move is to roll the die
	public int playerNumber = 0;
	public int diceValue = 0;

	public int maxRoll = 4;
	public int minRoll = 0;

	private Random r;

	// Use this for initialization
	void Start () {
		playerNumber = Random.Range(0, 1);
	}

	public void RollDice()
	{
		diceValue = Random.Range(minRoll, maxRoll+1);
		if (diceValue > maxRoll || diceValue < minRoll) RollDice();
	}

    public void ChangeControl()
    {
        Debug.Log("#" + playerNumber  + " player currently has control");
        /* This list does not necessarily come back in any logical order */
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playerNumber++;
        if (playerNumber >= players.Length)
            playerNumber = 0;

        foreach (GameObject player in players)
        {
            Debug.Log("Player " + player.name);
            if ("Player " + (playerNumber + 1) + " Tokens" == player.name) {
                player.GetComponent<PlayerHandler>().hasControl = true;
                Debug.Log("has control.");
            } else {
                player.GetComponent<PlayerHandler>().hasControl = false;
                Debug.Log("does not have control.");
            }
        }
    }
}
