using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class RaccoonToken : MonoBehaviour {
    private AICharacterControl ai;
    private Animator animator;
    private Transform target;

	// Use this for initialization
	void Awake () {
        this.ai = GetComponent<AICharacterControl>();
        this.animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        float dist = Vector3.Distance(target.position, transform.position);
        print("Distance to other: " + dist);
    }

    public void MoveTo(Transform destination) {
        this.target = destination;
        Debug.Log(destination);
        this.ai.SetTarget(destination);
        this.animator.SetTrigger("Walking");
        Debug.Log(GetComponent<Animator>());
    }
}
