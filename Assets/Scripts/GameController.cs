using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public int numTokens;
    public DiceRoller dice;
	public bool gameStarted = false;
    public int whichPlayersTurn = 1; // counting starts at 1 to be less confusing here
	public bool playerHasRolled = false;
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
	private int player1SelectedToken = 0;
	private Pathway player1Path;
    private int[] player2Tokens; // current position on the board for each player token
	private int player2SelectedToken = 0;
	private Pathway player2Path;
    private List<GameObject> p1TokenObjects;
    private List<GameObject> p2TokenObjects;

	private GameObject camera;

    void Start () {
        InitializeGame();
	}

	private void Update()
	{
		PlayerTurn();
	}

	private bool PlayerTurn() {
		if (this.whichPlayersTurn == 1) {
			if (playerHasRolled)
			{
				this.gameStarted = true;
				this.player1Tokens[player1SelectedToken] = this.currentRoll;
				if (!InstantiateToken(player1PathStops[player1SelectedToken], p1TokenObjects, 1)) return false;
				camera.GetComponent<CameraFollow>().SetFollowTarget(p1TokenObjects[player1SelectedToken]);
				if (this.currentRoll > 0)
				{
					p1TokenObjects[player1SelectedToken].GetComponent<RaccoonToken>().MoveTo(player1PathStops[this.currentRoll]);
				}
				Debug.Log("Player 1 rolled " + this.currentRoll);

				playerHasRolled = false;
				if(this.player1SelectedToken < numTokens) this.player1SelectedToken++;
			}
			if (gameStarted && p1TokenObjects.Count > 0 && p1TokenObjects[player1SelectedToken].GetComponent<RaccoonToken>().IsAtDestination())
			{
				this.whichPlayersTurn = 2;
			}
		} else {
			if (playerHasRolled)
			{
				this.gameStarted = true;
				this.player2Tokens[player2SelectedToken] = this.currentRoll;
				if (!InstantiateToken(player2PathStops[player2SelectedToken], p2TokenObjects, 2)) return false;
				camera.GetComponent<CameraFollow>().SetFollowTarget(p2TokenObjects[player2SelectedToken]);
				if (this.currentRoll > 0)
				{
					p2TokenObjects[player2SelectedToken].GetComponent<RaccoonToken>().MoveTo(player2PathStops[this.currentRoll]);
				}
				Debug.Log("Player 2 rolled " + this.currentRoll);

				playerHasRolled = false;
				if (this.player2SelectedToken < numTokens) this.player2SelectedToken++;
			}
		}
		return true;
	}

	public void RollDice () {
		this.currentRoll = dice.Roll();
		Debug.Log(this.currentRoll);
		if( this.whichPlayersTurn == 2 )
		{
			this.whichPlayersTurn = 1;
		} else
		{
			this.whichPlayersTurn = 2;
		}
	}

	bool InstantiateToken(Transform target, List<GameObject> objects, int playerNumber)
	{
		GameObject clone = Instantiate(tokenPrefab, target.position, target.rotation) as GameObject;
		clone.GetComponent<RaccoonToken>().SetPlayerNumber(playerNumber);
		Debug.Log(clone);
		objects.Add(clone);
		if (objects.Count > 0)
		{
			return true;
		} else
		{
			return false;
		}
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
}
