using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class RaccoonToken : MonoBehaviour {
    public int playerNumber;
    private Animator anim;
    private Transform target;
    private NavMeshAgent nav;
    private GameController gameController;
    private Transform[] pathway;
    private bool isJumping;

    // Use this for initialization
    void Awake () {
        SetReferences();
    }
	
	// Update is called once per frame
	void Update () {
        if (this.target != null) {
            CheckDistanceFromDestination();
            CheckIfWeNeedToJump(GetCurrentGameSquare(), this.gameController);
        }
        if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")) Debug.Log("Jumping");

        if (this.isJumping) this.anim.SetBool("Jump", this.isJumping);
    }

    void SetReferences() {
        GameObject gameControllerObject;
        this.anim = gameObject.GetComponent<Animator>();
        this.nav = gameObject.GetComponent<NavMeshAgent>();
        gameControllerObject = GameObject.FindGameObjectsWithTag("GameController")[0];
        this.gameController = gameControllerObject.GetComponent<GameController>();
        this.isJumping = false;
        if (this.playerNumber == 1) {
            pathway = gameController.player1PathStops;
        } else
        {
            pathway = gameController.player2PathStops;
        }
    }

    public void SetPlayerNumber(int number ) {
        this.playerNumber = number;
    }

    public void MoveTo(Transform destination) {
        this.target = destination;
        Vector3 position = new Vector3(destination.position.x, destination.position.y + 1, destination.position.z);
        this.nav.SetDestination(position);
        this.anim.SetBool("Walking", true);
    }

    GameObject GetCurrentGameSquare() {
        GameObject closestSquare = GetClosestGameObject(pathway);
        return closestSquare;
    }

    void CheckIfWeNeedToJump(GameObject closestSquare, GameController gameController) {
        Transform[] jumpSpots = gameController.jumpSpots;
        for (int i = 0 ; i < jumpSpots.Length; i++) {
            if(closestSquare.name == jumpSpots[i].name) {
                this.isJumping = true;
                //Debug.Log("Jumping! " + closestSquare.name + " == " + jumpSpots[i].name);
            }
        }
        this.isJumping = false;
    }

    GameObject GetClosestGameObject(Transform[] otherTransforms) {
        GameObject closestTarget = null;
        // Set the initial closest distance really high. We don't want to return null.
        float closestDistance = 1000;
 
        for(int i = 0; i < otherTransforms.Length; i++) {
            float distanceFromTarget = Vector3.Distance(transform.position, otherTransforms[i].position);
            if(distanceFromTarget < closestDistance) {
                // We have a new closest target.
                closestTarget = otherTransforms[i].gameObject;
                closestDistance = distanceFromTarget;
            }
        }

        return closestTarget;
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
