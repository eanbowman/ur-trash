using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUTokenHandler : TokenHandler
{
	void Update()
	{
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
			CheckCurrentTarget();
	}
}
