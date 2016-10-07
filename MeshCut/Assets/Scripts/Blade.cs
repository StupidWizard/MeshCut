using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Blade : MonoBehaviour {

	public Material capMaterial;

	public List<GameObject> listTarget = new List<GameObject>();

	public JointCutBody targetRoot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Cut() {
		FindTarget();

		if (listTarget.Count == 0) {
			Debug.LogError("Fail!");
			return;
		}

		targetRoot.ActivePhysics();
		Debug.LogError("Start cut at " + Time.realtimeSinceStartup);

		foreach (GameObject victim in listTarget) {
			GameObject[] pieces = MeshCut.Cut(victim, transform.position, transform.right, capMaterial);

			foreach (GameObject piece in pieces) {
				if (!piece.GetComponent<Rigidbody>()) {
					piece.AddComponent<Rigidbody>();
				}
			}
			targetRoot.AddCutGroup(victim, pieces[0], pieces[1]);
		}


		Debug.LogError("Finish cut at " + Time.realtimeSinceStartup);
		Plane testBlade = new Plane(transform.TransformPoint(Vector3.zero), 
			transform.TransformPoint(Vector3.forward),
			transform.TransformPoint(Vector3.up));
		targetRoot.SetBladePlane(testBlade);
		targetRoot.ProcessAfterCut();
	}

	void FindTarget () {
		RaycastHit hit;

		for (float y = -1; y <= 1; y += 0.2f) {
			Vector3 direct = transform.TransformDirection(new Vector3(0, y, 1));
			if (Physics.Raycast(transform.position, direct, out hit)) {
				GameObject victim = hit.collider.gameObject;
				AddTarget(victim);
			}
		}
	}

	void AddTarget(GameObject pTarget) {
		bool isPart = CheckPart(pTarget.transform);
		if (!isPart) {
			return;
		}

		foreach (GameObject obj in listTarget) {
			if (obj == pTarget) {
				return;
			}
		}
		listTarget.Add(pTarget);
	}

	bool CheckPart(Transform pTarget) {
		if (targetRoot != null) {
			Transform rootTransform = targetRoot.transform;

			Transform tempParent = pTarget;
			while (tempParent != null) {
				if (tempParent == rootTransform) {
					return true;
				}
				tempParent = tempParent.parent;
			}
		}
		return false;
	}

	void OnDrawGizmosSelected() {

		Gizmos.color = Color.green;

		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + transform.up * 0.5f, transform.position + transform.up * 0.5f + transform.forward * 5.0f);
		Gizmos.DrawLine(transform.position + -transform.up * 0.5f, transform.position + -transform.up * 0.5f + transform.forward * 5.0f);

		Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);
		Gizmos.DrawLine(transform.position,  transform.position + -transform.up * 0.5f);

	}

	void OnGUI() {
		if (GUI.Button(new Rect(10, 10, Screen.width/6, Screen.height/8), "Cut")) {
			Cut();
		}
	}
}
