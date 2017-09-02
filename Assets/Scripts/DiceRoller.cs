using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour {
    public int roll = 0;
    public float minRoll = 0.0f;
    public float maxRoll = 4.0f;
	public GameController gameController;
    Text diceRoll;

    private void Start()
    {
        this.diceRoll = transform.GetComponent<Text>();
    }

    public int Roll() {
        this.roll = (int)Random.Range(minRoll, maxRoll + 1);
        if(diceRoll != null) UpdateText();
		gameController.playerHasRolled = true;
        return this.roll;
    }

    /* Only used when this script is attached to a Text GUI object */
    void UpdateText() {
        this.diceRoll.text = "Dice roll: " + this.roll;
    }
}
