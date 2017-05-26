using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class RaccoonToken : MonoBehaviour {
    private Animator animator;
    private Transform target;
    private NavMeshAgent nav;

	// Use this for initialization
	void Awake () {
        SetReferences();
    }
	
	// Update is called once per frame
	void Update () {
        CheckDistanceFromDestination();
    }

    void SetReferences() {
        this.animator = GetComponent<Animator>();
        this.nav = GetComponent<NavMeshAgent>();
        this.animator.Play("Raccoon");
    }

    public void MoveTo(Transform destination) {
        SetReferences();
        this.target = destination;
        this.nav.SetDestination(this.target.position);
        Debug.Log(destination);
        this.animator.SetBool("Walking", true);
        Debug.Log("Walking state: " + this.animator.GetBool("Walking"));
    }

    void CheckDistanceFromDestination()
    {
        float dist = Vector3.Distance(this.target.position, transform.position);
        if (this.animator.GetBool("Walking") == true)
        {
            if (dist < 1.85f)
            {
                Debug.Log("Walking state: " + this.animator.GetBool("Walking"));
                this.animator.SetBool("Walking", false);
                Debug.Log("Walking state: " + this.animator.GetBool("Walking"));
            }
        }
    }
}
