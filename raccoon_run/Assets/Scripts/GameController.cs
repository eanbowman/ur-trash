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
        this.AddStatus("Welcome to the Royal Game of UR!");
        playerNumber = Random.Range(0, 1);
        this.AddStatus("Player #" + (playerNumber + 1) + "'s turn");
	}

	public void RollDice()
	{
        hasRolled = true;
		diceValue = Random.Range(minRoll, maxRoll+1);
		if (diceValue > maxRoll || diceValue < minRoll) RollDice();
        this.AddStatus("Player " + (playerNumber + 1) + " has rolled a " + diceValue);
        if(diceValue == 0) ChangeControl();
	}

    public void ChangeControl()
    {
        this.AddStatus("#" + playerNumber  + " player currently has control");
        /* This list does not necessarily come back in any logical order */
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        playerNumber++;
        if (playerNumber >= players.Length)
            playerNumber = 0;

        hasRolled = false;

        foreach (GameObject player in players)
        {
            this.AddStatus("Player " + player.name);
            if ("Player " + (playerNumber + 1) + " Tokens" == player.name) {
                player.GetComponent<PlayerHandler>().hasControl = true;
                this.AddStatus("has control.");
            } else {
                player.GetComponent<PlayerHandler>().hasControl = false;
                this.AddStatus("does not have control.");
            }
        }
        // On turn changeover, no tokens are selected.
        foreach(GameObject token in tokens)
        {
            token.GetComponent<TokenHandler>().isSelected = false;
        }
    }

    public void AddStatus(string text)
    {
        this.status.text += text + "\n";
        if(this.status.text.Length > 2000) this.status.text = this.status.text.Substring(1000);
    }
}
