using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public int numTokens;
    public DiceRoller dice;
    public int whichPlayersTurn = 1; // counting starts at 1 to be less confusing here
    public int goalPosition = 15; // number of spaces to go before a token counts as a point
    public int[] dangerousPositions; // positions which represent a fight
    public Transform[] player1PathStops; // the positions in game space of the game board stops
    public Transform[] player2PathStops; // the positions in game space of the game board stops
    public GameObject tokenPrefab;

    private bool gameOver = false;
    private int player1Points = 0;
    private int player2Points = 0;
    private int currentRoll = 0;
    private int[] player1Tokens; // current position on the board for each player token
    private int[] player2Tokens; // current position on the board for each player token
    private List<GameObject> p1TokenObjects;
    private List<GameObject> p2TokenObjects;

    void Start () {
        InitializeGame();
        this.currentRoll = dice.Roll();

        // Move the starting player automatically since they have no other moves
        if( whichPlayersTurn == 1 )
        {
            this.player1Tokens[0] = this.currentRoll;
			this.currentRoll = 15;
            InstantiateToken(player1PathStops[0], p1TokenObjects);
            if (this.currentRoll > 0) {
                p1TokenObjects[0].GetComponent<RaccoonToken>().MoveTo(player1PathStops[this.currentRoll]);
            }
            Debug.Log("Player 1 rolled " + this.currentRoll);
        }
         else
        {
            this.player2Tokens[0] = this.currentRoll;
            InstantiateToken(player2PathStops[0], p2TokenObjects);
            if (this.currentRoll > 0) {
                p2TokenObjects[0].GetComponent<RaccoonToken>().MoveTo(player2PathStops[this.currentRoll]);
            }
            Debug.Log("Player 2 rolled " + this.currentRoll);
        }
	}

    void InstantiateToken(Transform target, List<GameObject> objects)
    {
        GameObject clone = Instantiate(tokenPrefab, target.position, target.rotation) as GameObject;
        Debug.Log(clone);
        objects.Add(clone);
    }

    void InitializeGame()
    {
        // Create Player Token Arrays
        this.player1Tokens = new int[this.numTokens];
        this.player2Tokens = new int[this.numTokens];

        // Zero out the positions for all of each player's tokens
        for (int i = 0; i < this.numTokens; i++)
        {
            this.player1Tokens[i] = 0;
            this.player2Tokens[i] = 0;
        }

        p1TokenObjects = new List<GameObject>();
        p2TokenObjects = new List<GameObject>();
    }

    int TokensInPlay(int[] playerTokens)
    {
        int count = 0;
        for (int i = 0; i < this.numTokens; i++)
        {
            if (playerTokens[i] != 0) count++;
        }
        return count;
    }

    void DumpGameStateToConsole()
    {
        Debug.Log("P1: " + player1Points + " P2: " + player2Points + " ROLL: " + currentRoll + " GO: " + gameOver + " #Tokens: " + numTokens);
    }
}
