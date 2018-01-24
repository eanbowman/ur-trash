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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerHandler>().hasControl = false;
        }
        playerNumber++;
        if (playerNumber >= players.Length)
            playerNumber = 0;
        Debug.Log("All players set as not having control. Setting player #" + playerNumber + " to have control");
        players[playerNumber].GetComponent<PlayerHandler>().hasControl = true;
    }
}
