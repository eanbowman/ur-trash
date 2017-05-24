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
        CheckDistanceFromDestination();
    }

    public void MoveTo(Transform destination) {
        this.target = destination;
        Debug.Log(destination);
        this.ai.SetTarget(destination);
        this.animator.SetBool("Walking", true);
        Debug.Log("Walking state: " + this.animator.GetBool("Walking"));
    }

    void CheckDistanceFromDestination()
    {
        float dist = Vector3.Distance(target.position, transform.position);
        if (this.animator.GetBool("Walking"))
        {
            if (dist < 1.85f)
            {
                this.animator.SetBool("Walking", false);
                Debug.Log("Walking state: " + this.animator.GetBool("Walking"));
            }
        }
    }
}
