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
        animator.Play("Walking");
    }
	
	// Update is called once per frame
	void Update () {
        float dist = Vector3.Distance(target.position, transform.position);
        //Debug.Log("Distance to other: " + dist);
        if( dist < 1 )
        {
            this.animator.SetBool("Walking", false);
        }
    }

    public void MoveTo(Transform destination) {
        this.target = destination;
        Debug.Log(destination);
        this.ai.SetTarget(destination);
        this.animator.SetBool("Walking", true);
        Debug.Log("Walking state: " + this.animator.GetBool("Walking"));
    }
}
