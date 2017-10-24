using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextHandler : MonoBehaviour {
	public string scoresText;

	// Use this for initialization
	void Start () {
		this.scoresText  = "Player 1: ";
		this.scoresText += Environment.NewLine;
		this.scoresText += "Player 2: ";
		this.scoresText += Environment.NewLine;
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.gameObject.GetComponent<Text>().text = scoresText;
	}
}
