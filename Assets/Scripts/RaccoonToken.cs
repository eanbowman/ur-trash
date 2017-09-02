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
	private Transform step;
	private int stepIndex;
	private float stepDist;
	public Pathway pathway;
	public float stoppingDistance;

	private NavMeshAgent nav;
    private GameController gameController;
    private bool isJumping;

	public bool IsAtDestination()
	{
		Debug.Log(this.step + " : " + this.target);
		if(this.step == this.target )
		{
			return true;
		}
		return false;
	}

    // Use this for initialization
    void Awake () {
        SetReferences();
    }
	
	// Update is called once per frame
	void Update ()
	{
		if (this.target != null) {
			UpdateDistanceFromStep();
			SetNextDestination();
            CheckDistanceFromDestination();
            CheckIfWeNeedToJump(GetCurrentGameSquare(), this.gameController);
        }

        this.anim.SetBool ("Jump", this.isJumping);
    }

    void SetReferences() {
        GameObject gameControllerObject;
        this.anim = gameObject.GetComponent<Animator>();
        this.nav = gameObject.GetComponent<NavMeshAgent>();
        gameControllerObject = GameObject.FindGameObjectsWithTag("GameController")[0];
        this.gameController = gameControllerObject.GetComponent<GameController>();
        this.isJumping = false;
		this.playerNumber = 1;
		this.pathway = this.gameObject.GetComponent<Pathway>();
		this.pathway.Set(gameController.GetComponent<GameController>().player1PathStops);
    }

    public void SetPlayerNumber(int number ) {
        this.playerNumber = number;
    }

    public void MoveTo(Transform destination) {
        this.target = destination;
		SetNextDestination();
		this.anim.SetBool("Walking", true);
    }

    GameObject GetCurrentGameSquare() {
		GameObject closestSquare = GetClosestGameObject(pathway.Get());
        return closestSquare;
    }

    void CheckIfWeNeedToJump(GameObject closestSquare, GameController gameController) {
        Transform[] jumpSpots = gameController.jumpSpots;
		isJumping = false;
        for (int i = 0 ; i < jumpSpots.Length; i++) {
            if(closestSquare.name == jumpSpots[i].name) {
                isJumping = true;
				Debug.Log("Jumping! " + closestSquare.name + " == " + jumpSpots[i].name);
            }
        }
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

	void UpdateDistanceFromStep()
	{
		if (!this.step) SetNextDestination();
		Vector3 currentPos = new Vector3(this.step.position.x, transform.position.y, this.step.position.z);
		this.stepDist = Vector3.Distance(currentPos, transform.position);
	}

	void SetNextDestination()
	{
		if (this.pathway)
		{
			if (!this.step)
			{
				this.stepIndex = 0;
				Transform[] steps = this.pathway.Get();
				this.step = steps[this.stepIndex];
				Debug.Log(this.pathway.Get());
			}
			if (this.stepDist <= this.stoppingDistance)
			{
				Debug.Log(stepDist);
				if (this.step == this.target) StopWalking();
				this.stepIndex++;
				if (this.stepIndex < this.pathway.Count())
				{
					Transform next = this.pathway.Get()[this.stepIndex];
					this.step = next;
					Vector3 position = new Vector3(this.step.position.x, this.step.position.y + 1, this.step.position.z);
					this.nav.SetDestination(position);
				}
				//Debug.Log("Step: " + this.step + " Target: " + this.target);
			}
		}
		else
		{
			SetReferences();
			SetNextDestination();
		}
		//Debug.Log("Target is: " + this.target + " Next Step: " + this.step);
	}

    void CheckDistanceFromDestination()
    {
        //Debug.Log("destination: " + this.nav.destination + ", pathStatus: " + this.nav.pathStatus);
        float dist = Vector3.Distance(this.target.position, transform.position);
		bool walking = this.anim.GetBool("Walking");

		if (walking)
        {
			//Debug.Log(dist);
			if (dist < this.stoppingDistance)
            {
				StopWalking();
			}
        }
    }

	void StopWalking()
	{
		Debug.Log("Stop walking");
		this.nav.enabled = false;
		this.anim.SetBool("Walking", false);
	}
}
