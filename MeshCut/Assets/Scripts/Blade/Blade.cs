using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Blade : MonoBehaviour {

	public Transform[] eyes;

	public Material capMaterial;

	public List<GameObject> listTarget = new List<GameObject>();

	public JointCutBody targetRoot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Default Cut
	public void Cut() {
		FindTarget();

		DoCut ();
	}


	public Transform fakeOrigin;
	public Transform fakeStart;
	public Transform fakeEnd;

	[ContextMenu("Fake RotateCut")]
	void FakeRotateCut() {
		Cut (fakeOrigin.position, fakeStart.position, fakeEnd.position);
	}

	public void Cut(Vector3 originPos, Vector3 vertexStart, Vector3 vertexEnd) {
		transform.parent.position = originPos;
		vertexEnd = originPos + (vertexEnd - originPos).normalized * (vertexStart - originPos).magnitude;
		transform.parent.LookAt (0.5f * (vertexStart + vertexEnd));

		Vector3 localStart = transform.parent.InverseTransformPoint (vertexStart).normalized;
		Vector3 localEnd = transform.parent.InverseTransformPoint (vertexEnd).normalized;


		Vector3 horizontalDirect = localEnd - localStart;
		Debug.LogError ("horizontal Direct = " + horizontalDirect + " localStart " + localStart + " localEnd " + localEnd);
		if (horizontalDirect.magnitude > 0) {
			horizontalDirect.Normalize ();
			float angle = Mathf.Rad2Deg * Mathf.Asin (horizontalDirect.x);		// default: transform plane is rotate 90degree -> sin = x; cos = y
			transform.localRotation = Quaternion.Euler(0, 0, angle);
		}

		Cut ();
	}

	void DoCut() {
		if (listTarget.Count == 0) {
			Debug.LogError("Fail!");
			return;
		}

		targetRoot.ActivePhysics();
		//		Debug.LogError("Start cut at " + Time.realtimeSinceStartup);

		foreach (GameObject victim in listTarget) {
			GameObject[] pieces = MeshCut.Cut(victim, transform.position, transform.right, capMaterial);

			foreach (GameObject piece in pieces) {
				if (!piece.GetComponent<Rigidbody>()) {
					piece.AddComponent<Rigidbody>();
				}
			}
			targetRoot.AddCutGroup(victim, pieces[0], pieces[1]);
		}


		//		Debug.LogError("Finish cut at " + Time.realtimeSinceStartup);
		Plane testBlade = new Plane(transform.TransformPoint(Vector3.zero), 
			transform.TransformPoint(Vector3.forward),
			transform.TransformPoint(Vector3.up));
		targetRoot.SetBladePlane(testBlade);
		targetRoot.ProcessAfterCut();
	}

	void FindTarget () {
//		RaycastHit hit;

		for (float y = -10; y <= 10; y += 0.1f) {
			Vector3 direct = transform.TransformDirection(new Vector3(0, y, 1));
//			if (Physics.Raycast(transform.position, direct, out hit)) {
//				GameObject victim = hit.collider.gameObject;
//				AddTarget(victim);
//			}
			foreach (Transform eye in eyes) {
				RaycastHit[] listHit = Physics.RaycastAll(eye.position, direct);
				if (listHit != null && listHit.Length > 0) {
					foreach (RaycastHit hitInfo in listHit) {
						GameObject victim = hitInfo.collider.gameObject;
						AddTarget(victim);
					}
				}
			}
		}
	}

	void AddTarget(GameObject pTarget) {
		JointPlane jointPlane = pTarget.GetComponent<JointPlane>();
		if (jointPlane != null) {
			jointPlane.beCut = true;

			return;
		}

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


}
