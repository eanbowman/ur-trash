using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Interactable : MonoBehaviour {
    [HideInInspector]
    public NavMeshAgent playerAgent;
    public float stoppingDistance = 2.0f;
    private bool hasInteracted = false;

    void Update()
    {
        if(playerAgent != null && !playerAgent.pathPending && !hasInteracted)
        {
            if(playerAgent.remainingDistance < playerAgent.stoppingDistance)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public virtual void MoveToInteraction(NavMeshAgent playerAgent)
    {
        hasInteracted = false;
        this.playerAgent = playerAgent;
        playerAgent.stoppingDistance = stoppingDistance;
        playerAgent.destination = this.transform.position;
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting with the base class.");
    }
}
