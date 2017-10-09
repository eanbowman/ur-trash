﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour {
	public List<Transform> stops;

	public void Set(Transform[] newPath) {
		stops.Clear();
		foreach ( Transform step in newPath ) {
			stops.Add(step);
		}
		Debug.Log("Pathway::Set()");
	}
	public Transform[] Get() {
		Transform[] currentPath = new Transform[stops.Count];

		for(int i=0; i<stops.Count; i++ ) {
			currentPath[i] = stops[i];
		}
		return currentPath;
	}
	public int Count()
	{
		return stops.Count;
	}
}
