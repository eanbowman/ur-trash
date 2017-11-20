using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public int numTokens;
    public DiceRoller dice;
	public bool gameStarted = false;
	public bool gameOver = false;
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
	public GameObject scoreTextObject;
	public GameObject gameUIPanel;
	public GameObject gameOverPanel;

	private ScoreTextHandler scoreTextHandler;

	private Text gameStatusText; // A reference to the game's UI
    private int currentRoll = 0;
    private int[] player1Tokens; // current position on the board for each player token
	private int player1SelectedToken = 0;
	private int[] player2Tokens; // current position on the board for each player token
	private int player2SelectedToken = 0;
	private List<GameObject> p1TokenObjects;
    private List<GameObject> p2TokenObjects;
	private int player1Points = 0;
	private int player2Points = 0;
	private GameObject tokenInPlay;

	private GameObject camera;

    void Start () {
        InitializeGame();
	}

	private void Update()
	{
		if( this.whichPlayersTurn.Equals(null) ) { InitializeGame(); }
		if (!this.gameOver)
		{
			PlayerTurn(this.whichPlayersTurn);
		} else
		{
			PlayAgainMenu();
		}
	}

	private void PlayerTurn(int player) {
		List<GameObject> currentTokenObjectList;
		int currentTokenIndex;
		GameObject currentRaccoonToken;
		Transform target;
		Transform[] currentPathStops;
		int[] currentTokenPositions;

		this.currentRoll = 2;

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

		// If the player has exhausted all of their tokens, they won!
		if (currentTokenIndex >= currentTokenPositions.Length)
		{
			Debug.Log("This player has exhausted all of their tokens! They won!");
			gameOver = true;
			return;
		};

		// If the player rolls a number higher than the number of steps in the path, they lose a turn.
		if (currentTokenPositions[currentTokenIndex] >= currentPathStops.Length)
		{
			throw new UnityException("Somehow we have a current token position past the end of the path!");
		}

		// Set the Current Token
		// If it doesn't exist, create one!
		target = currentPathStops[currentTokenPositions[currentTokenIndex]];
		if (playerHasRolled &&
			currentPathStops.Length > 0 &&
			currentTokenObjectList.Count <= currentTokenIndex) {
			target = currentPathStops[0];
			if (!InstantiateToken(target, currentTokenObjectList, player, currentTokenIndex))
			{
				Debug.LogError("Error creating token!");
				return;
			}
		}

		if (currentTokenIndex > currentTokenObjectList.Count - 1)
		{
			return;
		}
		else
		{
			currentRaccoonToken = currentTokenObjectList[currentTokenIndex];
		}

		// Set current token in play
		this.tokenInPlay = currentRaccoonToken;

		
		// If the player has rolled,
		// move that player!
		RaccoonToken currentRaccoon = currentRaccoonToken.GetComponent<RaccoonToken>();
		bool otherPlayersTurn = false;
		if (playerHasRolled) {
			// Follow the currently selected token with the camera
			camera.GetComponent<CameraFollow>().SetFollowTarget(this.tokenInPlay);

			ClearStatusMessages();
			playerHasRolled = false;
			int newPositionIndex = currentTokenPositions[currentTokenIndex];
			// ensure new position index is in array
			Debug.Log("newPositionIndex: " + newPositionIndex + " currentTokenPositions.Length: " + currentPathStops.Length);
			if (newPositionIndex >= currentPathStops.Length) {
				this.gameStatusText.text += "Player " + this.whichPlayersTurn + " can not move that piece!";
			} else if (newPositionIndex == currentPathStops.Length - 1) {
				this.gameStatusText.text += "Player " + this.whichPlayersTurn + " earned a point!";
				if (whichPlayersTurn == 1)
				{
					player1SelectedToken++;
					this.player1Points++;
				} else
				{
					player2SelectedToken++;
					this.player2Points++;
				}
			} else
			{
				newPositionIndex += this.currentRoll;
			}
			if (newPositionIndex >= currentPathStops.Length) newPositionIndex = currentPathStops.Length - 1;
			currentTokenPositions[currentTokenIndex] = newPositionIndex;
			
			Transform newPosition = currentPathStops[newPositionIndex];
			if (this.currentRoll != 0) {
				currentRaccoon.MoveTo(newPosition);
			} else {
				currentRaccoon.IsNotInPlay();
				otherPlayersTurn = true;
			}
			UpdateScore();
		}

		// Are we at a dangerous position?
		bool dangerous = false;
		foreach( int spot in dangerousPositions )
		{
			if (currentTokenPositions[currentTokenIndex] == spot)
			{
				dangerous = true;
			}
		}

		// If the player is at their destination,
		// their turn is over
		if (otherPlayersTurn || 
			(currentRaccoon.IsInPlay() && currentRaccoon.IsAtDestination())
			) {
			if(player == 1)
			{
				// If we are on another player's space, knock them back to start
				if (dangerous) for (int i = 0; i < player2Tokens.Length; i++)
				{
					if (player1Tokens[i] != 0)
					{
						if (player2Tokens[i] == currentTokenPositions[currentTokenIndex])
						{
							Debug.Log(player2Tokens[i].ToString() + " == " + currentTokenIndex.ToString());
							gameStatusText.text = "Player 1 knocked player 2's token back to the beginning!" + Environment.NewLine;
							if (p2TokenObjects.Count > i)
							{
								p2TokenObjects[i].GetComponent<RaccoonToken>().RollBackTo(player2PathStops[0]);
							}
						}
					}
				}
				this.whichPlayersTurn = 2;
				currentRaccoon.IsNotInPlay();
				LogPlayerTurn();
			}
			else if(player == 2)
			{
				// If we are on another player's space, knock them back to start
				if (dangerous) for (int i = 0; i < player1Tokens.Length; i++)
				{
					if(player1Tokens[i] != 0)
					{
						if (player1Tokens[i] == currentTokenPositions[currentTokenIndex])
						{
							Debug.Log(player1Tokens[i].ToString() + " == " + currentTokenPositions[currentTokenIndex].ToString());
							gameStatusText.text = "Player 2 knocked player 1's token back to the beginning!" + Environment.NewLine;
							if (p1TokenObjects.Count > i)
							{
								p1TokenObjects[i].GetComponent<RaccoonToken>().RollBackTo(player1PathStops[0]);
							}
						}
					}
				}
				this.whichPlayersTurn = 1;
				currentRaccoon.IsNotInPlay();
				LogPlayerTurn();
			}
			else
			{
				throw new UnityException("An invalid player number was set.");
			}
		}
	}

	private void PlayAgainMenu()
	{
		this.gameStatusText.text = "GAME OVER!";
		this.gameUIPanel.SetActive(false);
		this.gameOverPanel.SetActive(true);
	}

	private void ResetMenus()
	{
		this.gameStatusText.text = "";
		this.gameUIPanel.SetActive(true);
		this.gameOverPanel.SetActive(false);
	}

	private void UpdateScore()
	{
		if (scoreTextObject)
		{
			scoreTextHandler = scoreTextObject.GetComponent<ScoreTextHandler>();
			scoreTextHandler.scoresText = "P1 Pos: " + player1Tokens[player1SelectedToken];
			scoreTextHandler.scoresText += ", Score: " + (this.player1Points.ToString());
			scoreTextHandler.scoresText += Environment.NewLine;
			scoreTextHandler.scoresText += "P2 Pos: " + player2Tokens[player2SelectedToken];
			scoreTextHandler.scoresText += ", Score: " + (this.player2Points.ToString());
			scoreTextHandler.scoresText += Environment.NewLine;
		}
	}

	public void RollDice () {
		this.currentRoll = dice.Roll();
	}

	void ClearStatusMessages()
	{
		this.gameStatusText.text = "";
	}

	void LogPlayerTurn()
	{
		if (this.oldPlayersTurn != this.whichPlayersTurn) Debug.Log("Player " + whichPlayersTurn + "'s turn");
		this.oldPlayersTurn = this.whichPlayersTurn;

		this.gameStatusText.text += "Player " + this.whichPlayersTurn + "'s turn!";
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

	public void ResetGame()
	{
		Debug.Log("Reset game!");

		// Remove all Player 1 token objects
		foreach (GameObject token in p1TokenObjects)
		{
			GameObject.Destroy(token);
		}
		p1TokenObjects = new List<GameObject>();
		player1SelectedToken = 0;

		// Remove all Player 2 token objects
		foreach (GameObject token in p2TokenObjects)
		{
			GameObject.Destroy(token);
		}
		p2TokenObjects = new List<GameObject>();
		player2SelectedToken = 0;

		this.whichPlayersTurn = 1;
		this.gameStarted = false;
		this.gameOver = false;

		// Create Player Token Arrays
		this.player1Tokens = new int[this.numTokens + 1];
		this.player2Tokens = new int[this.numTokens + 1];

		ResetMenus();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); // Rarely, if ever, force Garbage Collection
	}

    private void InitializeGame()
    {
		Debug.Log("Initializing game!");
		this.whichPlayersTurn = 1;
		this.gameStarted = false;
		this.gameOver = false;

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
