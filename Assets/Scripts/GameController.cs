using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public int numTokens;
    public DiceRoller dice;
	public bool gameStarted = false;
    public int whichPlayersTurn = 1; // counting starts at 1 to be less confusing here
	private int oldPlayersTurn = 0;
	public bool playerHasRolled = false;
    public int goalPosition = 15; // number of spaces to go before a token counts as a point
    public int[] dangerousPositions; // positions which represent a fight
    public Transform[] player1PathStops; // the positions in game space of the game board stops
    public Transform[] player2PathStops; // the positions in game space of the game board stops
    public GameObject tokenPrefab;
	public int m_StartWait = 3;
    public Transform[] jumpSpots; // The positions on the map where a jump is required

	private Text gameStatusText; // A reference to the game's UI
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
	private string state = "waiting to roll";
	private GameObject tokenInPlay;

	private GameObject camera;

    void Start () {
        InitializeGame();
	}

	private void Update()
	{
		PlayerTurn();
	}

	private bool PlayerTurn() {
		if(this.oldPlayersTurn != this.whichPlayersTurn) Debug.Log("Player " + whichPlayersTurn + "'s turn");
		this.oldPlayersTurn = this.whichPlayersTurn;

		this.gameStatusText.text = "Player " + this.whichPlayersTurn + "'s turn!";

		if (!this.tokenInPlay || this.tokenInPlay.GetComponent<RaccoonToken>().IsAtDestination())
		{
			if (this.whichPlayersTurn == 1)
			{
				if (playerHasRolled)
				{
					playerHasRolled = false;
					this.gameStarted = true;

					// If it doesn't exist, create a token for this piece
					if (!InstantiateToken(player1PathStops[player1SelectedToken], p1TokenObjects, 1, player1SelectedToken)) return false;

					// Set the target position for this token
					int targetPosition = this.player1Tokens[player1SelectedToken] += this.currentRoll;

					// The token currently in play is this one
					this.tokenInPlay = p1TokenObjects[this.player1SelectedToken];

					// Follow the token with the camera
					camera.GetComponent<CameraFollow>().SetFollowTarget(this.tokenInPlay);

					// Move the token toward its next target
					if (this.currentRoll > 0)
					{
						p1TokenObjects[player1SelectedToken].GetComponent<RaccoonToken>().MoveTo(player1PathStops[targetPosition]);
					}
					this.gameStatusText.text += " and they rolled " + this.currentRoll;

					// TODO: Fix this - if we haven't exhausted the tokens, create a new one. Otherwise recycle!
					if (this.player1SelectedToken < numTokens)
					{
						this.player1SelectedToken++;
					}
					else
					{
						this.player1SelectedToken = 0;
					}
				}
				if (this.tokenInPlay && this.tokenInPlay.GetComponent<RaccoonToken>().IsAtDestination())
				{
					this.whichPlayersTurn = 2;
					this.tokenInPlay = null;
				}
			} else if (this.whichPlayersTurn == 2) {
				if (playerHasRolled)
				{
					playerHasRolled = false;
					this.gameStarted = true;

					// If it doesn't exist, create a token for this piece
					if (!InstantiateToken(player2PathStops[player2SelectedToken], p1TokenObjects, 1, player2SelectedToken)) return false;

					// Set the target position for this token
					int targetPosition = this.player2Tokens[player2SelectedToken] += this.currentRoll;

					// The token currently in play is this one
					this.tokenInPlay = p1TokenObjects[this.player2SelectedToken];

					// Follow the token with the camera
					camera.GetComponent<CameraFollow>().SetFollowTarget(this.tokenInPlay);

					// Move the token toward its next target
					if (this.currentRoll > 0)
					{
						p1TokenObjects[player2SelectedToken].GetComponent<RaccoonToken>().MoveTo(player2PathStops[targetPosition]);
					}
					this.gameStatusText.text += " and they rolled " + this.currentRoll;

					// TODO: Fix this - if we haven't exhausted the tokens, create a new one. Otherwise recycle!
					if (this.player2SelectedToken < numTokens)
					{
						this.player2SelectedToken++;
					}
					else
					{
						this.player2SelectedToken = 0;
					}
				}
				if (this.tokenInPlay && this.tokenInPlay.GetComponent<RaccoonToken>().IsAtDestination())
				{
					this.whichPlayersTurn = 1;
					this.tokenInPlay = null;
				}
			} else {
				Debug.Log("You broke the game, Lilithe.");
				this.whichPlayersTurn = 1;
				this.tokenInPlay = null;
			}
		} else {
			// Do nothing, player token in transit
		}
		return true;
	}

	public void RollDice () {
		this.state = "rolled dice";
		this.currentRoll = dice.Roll();
	}

	bool InstantiateToken(Transform target, List<GameObject> objects, int playerNumber, int tokenNumber)
	{
		GameObject clone = Instantiate(tokenPrefab, target.position, target.rotation) as GameObject;
		clone.GetComponent<RaccoonToken>().SetPlayerNumber(playerNumber);
		clone.GetComponent<RaccoonToken>().SetTokenNumber(tokenNumber);
		//Debug.Log(clone);
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
		this.whichPlayersTurn = 1;
		this.gameStarted = false;

        // Create Player Token Arrays
        this.player1Tokens = new int[this.numTokens+1];
        this.player2Tokens = new int[this.numTokens+1];

        // Zero out the positions for all of each player's tokens
        for (int i = 0; i < this.numTokens; i++)
        {
            this.player1Tokens[i] = 0;
            this.player2Tokens[i] = 0;
        }

        p1TokenObjects = new List<GameObject>();
        p2TokenObjects = new List<GameObject>();

		this.camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

		this.gameStatusText = GameObject.Find("GameStatus").GetComponent<Text>();
		Debug.Log(this.gameStatusText.text);
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
