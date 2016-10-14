using UnityEngine;
using System.Collections;

public class SwordTracker : MonoBehaviour {

	[SerializeField]
	Transform origin;

	[SerializeField]
	Transform vertex;		// Distance of (Vertex, origin) Must be 1.0f

	[SerializeField]


	// Use this for initialization
	void Start () {
		if (origin == null) {
			origin = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
