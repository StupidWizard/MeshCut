using UnityEngine;
using System.Collections;

public class SwordTracker : MonoBehaviour {

	[SerializeField]
	Transform origin;

	[SerializeField]
	Transform vertex;		// Distance of (Vertex, origin) Must be 1.0f

	public Vector3 startOrigin;
	public Vector3 startVertex;

	public Vector3 endOrigin;
	public Vector3 endVertex;

	// Use this for initialization
	void Start () {
		if (origin == null) {
			origin = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartTracker() {
		startOrigin = origin.position;
		startVertex = vertex.position;
	}

	public void StopTracker() {
		endOrigin = origin.position;
		endVertex = vertex.position;
	}

	public Vector3[] GetTrackerData() {
		return new Vector3[] {
			0.5f * (startOrigin + endOrigin),
			startVertex,
			endVertex
		};
	}
}
