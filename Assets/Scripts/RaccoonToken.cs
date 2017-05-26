using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class RaccoonToken : MonoBehaviour {
    private Animator anim;
    private Transform target;
    private NavMeshAgent nav;

	// Use this for initialization
	void Awake () {
        SetReferences();
    }
	
	// Update is called once per frame
	void Update () {
        if( this.target != null ) CheckDistanceFromDestination();

        bool jump = Input.GetButtonDown("Fire1");
        this.anim.SetBool("Jump", jump);
    }

    void SetReferences() {
        this.anim = gameObject.GetComponent<Animator>();
        Debug.Log(this.anim.runtimeAnimatorController);
        this.nav = gameObject.GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Transform destination) {
        Debug.Log(this.anim);
        this.target = destination;
        Vector3 position = new Vector3(destination.position.x, destination.position.y + 1, destination.position.z);
        this.nav.SetDestination(position);
        Debug.Log(this.nav);
        this.anim.SetBool("Walking", true);
        Debug.Log("Walking state: " + this.anim.GetBool("Walking"));
    }

    void CheckDistanceFromDestination()
    {
        //Debug.Log("destination: " + this.nav.destination + ", pathStatus: " + this.nav.pathStatus);
        float dist = Vector3.Distance(this.target.position, transform.position);
        if (this.anim.GetBool("Walking") == true)
        {
            if (dist < 2.0f)
            {
                this.nav.enabled = false;
                Debug.Log("Walking state: " + this.anim.GetBool("Walking"));
                this.anim.SetBool("Walking", false);
                Debug.Log("Walking state: " + this.anim.GetBool("Walking"));
            }
        }
    }
}
