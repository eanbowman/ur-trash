using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public string gameState = "Rolling"; // the first move is to roll the die
	public int playerNumber = 1;
	public int diceValue = 0;

	public int maxRoll = 4;
	public int minRoll = 0;

	private Random r;

	// Use this for initialization
	void Start () {
		playerNumber = Random.Range(1, 2);
	}

	public void RollDice()
	{
		diceValue = Random.Range(minRoll, maxRoll+1);
		if (diceValue > maxRoll || diceValue < minRoll) RollDice();
	}

    public void ChangeControl()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerHandler>().hasControl = false;
        }
        if (++playerNumber >= players.Length)
            playerNumber = 0;
        if (players != null && players.Length > 0)
        {
            Debug.Log("'players' exists and has elements");
            Debug.Log("playerNumber: " + playerNumber);
            if (players[playerNumber] != null)
            {
                Debug.Log("'players[playerNumber]' exists");
                PlayerHandler handler = players[playerNumber].GetComponent<PlayerHandler>();
                if (handler != null) {
                    Debug.Log("PlayerHandler component exists in players[playerNumber]");
                    handler.hasControl = true;
                }
            }
        }
    }
}
