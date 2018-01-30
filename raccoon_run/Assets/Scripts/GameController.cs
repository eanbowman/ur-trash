using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public string gameState = "Rolling"; // the first move is to roll the die
	public int playerNumber = 0;
	public int diceValue = 0;
    public bool hasRolled = false;

	public int maxRoll = 4;
	public int minRoll = 0;

	private Random r;

    private Text status;

	// Use this for initialization
	void Start () {
        this.status = GameObject.FindGameObjectWithTag("Status").GetComponent<Text>();
        this.status.text = "Welcome to the Royal Game of UR!\n\n";
        playerNumber = Random.Range(0, 1);
        this.status.text += "Player #" + (playerNumber + 1) + "'s turn\n\n";
	}

	public void RollDice()
	{
        hasRolled = true;
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

        hasRolled = false;

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

    public void AddStatus(string text)
    {
        this.status.text += text + "\n\n";
    }
}
