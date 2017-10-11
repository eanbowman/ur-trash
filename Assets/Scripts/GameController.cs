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
    private int currentRoll = 0;
    private int[] player1Tokens; // current position on the board for each player token
	private int player1SelectedToken = 0;
	private int[] player2Tokens; // current position on the board for each player token
	private int player2SelectedToken = 0;
	private List<GameObject> p1TokenObjects;
    private List<GameObject> p2TokenObjects;
	private GameObject tokenInPlay;

	private GameObject camera;

    void Start () {
        InitializeGame();
	}

	private void Update()
	{
		if (playerHasRolled) PlayerTurn(this.whichPlayersTurn);
	}

	private void PlayerTurn(int player) {
		LogPlayerTurn();
		List<GameObject> currentTokenObjectList;
		int currentTokenIndex;
		GameObject currentRaccoonToken;
		Transform target;
		Transform[] currentPathStops;
		int[] currentTokenPositions;

		// Set current variables for player objects based on whose turn it is
		if (player == 1)
		{
			currentTokenObjectList = p1TokenObjects;
			currentTokenIndex = player1SelectedToken;
			currentPathStops = player1PathStops;
			currentTokenPositions = player1Tokens;
		} else if (player == 2)
		{
			currentTokenObjectList = p2TokenObjects;
			currentTokenIndex = player2SelectedToken;
			currentPathStops = player2PathStops;
			currentTokenPositions = player2Tokens;
		} else {
			throw new UnityException("An invalid player number was set.");
		}

		// Set the Current Token
		// If it doesn't exist, create one!
		target = currentPathStops[currentTokenPositions[currentTokenIndex]];
		if (currentTokenObjectList.Count <= currentTokenIndex) {
			if(!InstantiateToken(target, currentTokenObjectList, player, currentTokenIndex)) Debug.Log("Error creating token!");
		}
		currentRaccoonToken = currentTokenObjectList[currentTokenIndex];

		// Set current token in play
		this.tokenInPlay = currentRaccoonToken;

		// Follow the currently selected token with the camera
		camera.GetComponent<CameraFollow>().SetFollowTarget(this.tokenInPlay);

		// If the player has rolled,
		// move that player!
		RaccoonToken currentRaccoon = currentRaccoonToken.GetComponent<RaccoonToken>();
		if (playerHasRolled) {
			playerHasRolled = false;
			int newPositionIndex = currentTokenPositions[currentTokenIndex] + this.currentRoll;
			Transform newPosition = currentPathStops[newPositionIndex];
			currentRaccoon.MoveTo(newPosition);
		}

		// If the player is at their destination,
		// their turn is over
		if (currentRaccoon.IsAtDestination()) {
			if(player == 1)
			{
				this.whichPlayersTurn = 2;
			} else if(player == 2)
			{
				this.whichPlayersTurn = 1;
			}
			else
			{
				throw new UnityException("An invalid player number was set.");
			}
		}
	}

	public void RollDice () {
		this.currentRoll = dice.Roll();
	}

	void LogPlayerTurn()
	{
		if (this.oldPlayersTurn != this.whichPlayersTurn) Debug.Log("Player " + whichPlayersTurn + "'s turn");
		this.oldPlayersTurn = this.whichPlayersTurn;

		this.gameStatusText.text = "Player " + this.whichPlayersTurn + "'s turn!";
	}

	bool InstantiateToken(Transform target, List<GameObject> objects, int playerNumber, int tokenNumber)
	{
		GameObject clone = Instantiate(tokenPrefab, target.position, target.rotation) as GameObject;
		clone.GetComponent<RaccoonToken>().SetPlayerNumber(playerNumber);
		clone.GetComponent<RaccoonToken>().SetTokenNumber(tokenNumber);
		//Debug.Log(clone);
		int count = objects.Count;
		objects.Add(clone);
		if (objects.Count > count)
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
		player1SelectedToken = 0;
        p2TokenObjects = new List<GameObject>();
		player2SelectedToken = 0;

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
