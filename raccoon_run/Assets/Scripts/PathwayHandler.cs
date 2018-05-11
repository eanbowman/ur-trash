using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathwayHandler : MonoBehaviour {
	public List<Transform> stops;
    public bool[] vacancy;

    void Start()
    {
        vacancy = new bool[stops.Count];
        for(int i = 0; i < stops.Count; i++)
        {
            vacancy[i] = true; // it's vacant
        }
    }

    public bool SetVacancy(int spot)
    {
        if (spot < vacancy.Length && vacancy[spot] == true)
        {
            vacancy[spot] = false;
            return true;
        }
        return false;
    }

    public void LeaveSpot(int spot)
    {
        if(spot < vacancy.Length) vacancy[spot] = true;
    }
}
