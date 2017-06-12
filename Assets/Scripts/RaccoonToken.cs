using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class RaccoonToken : MonoBehaviour {
    private Animator anim;
    private Transform target;
    private NavMeshAgent nav;
    private GameController gameController;

	// Use this for initialization
	void Awake () {
        SetReferences();
    }
	
	// Update is called once per frame
	void Update () {
        if (this.target != null) {
            CheckDistanceFromDestination();
            GetCurrentGameSquare();
            CheckIfWeNeedToJump();
        }
    }

    void SetReferences() {
        GameObject gameObjects;
        this.anim = gameObject.GetComponent<Animator>();
        this.nav = gameObject.GetComponent<NavMeshAgent>();
        gameObjects = GameObject.FindGameObjectsWithTag("GameController")[0];
        this.gameController = gameObjects.GetComponent<GameController>();
    }

    public void PlayJumpAnimation() {
        this.anim.SetBool("Jump", true);
    }

    public void StopJumpAnimation()
    {
        this.anim.SetBool("Jump", false);
    }

    public void MoveTo(Transform destination) {
        this.target = destination;
        Vector3 position = new Vector3(destination.position.x, destination.position.y + 1, destination.position.z);
        this.nav.SetDestination(position);
        this.anim.SetBool("Walking", true);
    }

    void GetCurrentGameSquare() {

    }

    void CheckIfWeNeedToJump() {
        //Transform nextJump = 
        float dist = Vector3.Distance(this.target.position, transform.position);
    }

    void CheckDistanceFromDestination()
    {
        //Debug.Log("destination: " + this.nav.destination + ", pathStatus: " + this.nav.pathStatus);
        float dist = Vector3.Distance(this.target.position, transform.position);
        if (this.anim.GetBool("Walking") == true)
        {
			if (dist < 1.1f)
            {
                this.nav.enabled = false;
                this.anim.SetBool("Walking", false);
            }
        }
    }
}
