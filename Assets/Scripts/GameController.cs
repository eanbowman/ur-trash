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
	public int m_StartWait = 3;
    public Transform[] jumpSpots; // The positions on the map where a jump is required

    private bool gameOver = false;
    private int player1Points = 0;
    private int player2Points = 0;
    private int currentRoll = 0;
    private int[] player1Tokens; // current position on the board for each player token
	private Pathway player1Path;
    private int[] player2Tokens; // current position on the board for each player token
	private Pathway player2Path;
    private List<GameObject> p1TokenObjects;
    private List<GameObject> p2TokenObjects;

	private GameObject camera;

    void Start () {
        InitializeGame();
		RollDice ();
		PlayerTurn (whichPlayersTurn);
	}

	private void PlayerTurn( int player ) {
		if( whichPlayersTurn == 1 ) {
			this.player1Tokens[0] = this.currentRoll;
			this.currentRoll = 15;
			InstantiateToken(player1PathStops[0], p1TokenObjects);
            this.p1TokenObjects[0].GetComponent<RaccoonToken>().SetPlayerNumber(1);
			camera.GetComponent<CameraFollow> ().SetFollowTarget (p1TokenObjects [0]);
			if (this.currentRoll > 0) {
				p1TokenObjects[0].GetComponent<RaccoonToken>().MoveTo(player1PathStops[this.currentRoll]);
			}
			Debug.Log("Player 1 rolled " + this.currentRoll);            
		} else {
			this.player2Tokens[0] = this.currentRoll;
			InstantiateToken(player2PathStops[0], p2TokenObjects);
            this.p2TokenObjects[0].GetComponent<RaccoonToken>().SetPlayerNumber(2);
            if (this.currentRoll > 0) {
				p2TokenObjects[0].GetComponent<RaccoonToken>().MoveTo(player2PathStops[this.currentRoll]);
			}
			Debug.Log("Player 2 rolled " + this.currentRoll);
		}
	}

	private void RollDice () {
		this.currentRoll = dice.Roll();
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

		this.camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
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
