﻿using System.Collections;
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
}
