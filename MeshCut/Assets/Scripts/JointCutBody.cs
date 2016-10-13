using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//[Serializable]
//public class CutGroup {
//	public GameObject origin;
//	public GameObject left;			// true vs plane
//	public GameObject right;		// false vs plane
//}

public class JointCutBody : MonoBehaviour {

	public JointElement[] listJointElement;

	public List<JointElement> listCutGroup = new List<JointElement>();

	public Plane blade;

	// Use this for initialization
	void Start () {
		Init();
	}

	void Init() {
		listJointElement = gameObject.GetComponentsInChildren<JointElement>();
		foreach (JointElement joint in listJointElement) {
			joint.GetComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ActivePhysics() {
		foreach (JointElement joint in listJointElement) {
			joint.GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	public void SetBladePlane(Plane blade) {
		this.blade = blade;
	}

	public void AddCutGroup(GameObject origin, GameObject left, GameObject right) {
		foreach (JointElement element in listJointElement) {
			if (element.mCollider.gameObject == origin) {
				element.CutToParts(left, right);
				listCutGroup.Add(element);
				return;
			}
		}
	}

	public void ProcessAfterCut() {
		Debug.LogError("Start Process: update main body and joint for all cutGroup");
		foreach (JointElement jointElement in listJointElement) {
			jointElement.CheckSide(blade);
//			if (!jointElement.beCut) {
//				jointElement.CheckSide(blade);
//			}
		}

		Debug.LogError("During Process: update target and co-target for cutGroup");
		foreach (JointElement jointElement in listCutGroup) {
			ProcessJointNode(jointElement);
		}

		Debug.LogError("End Process: remove unuse body");
		foreach (JointElement jointElement in listCutGroup) {
			jointElement.ClearOldObject();
		}
	}

	void ProcessJointNode(JointElement jointElement) {
		// vs parent
		if (jointElement.mJoint != null) {
			Rigidbody targetBody = jointElement.mJoint.connectedBody;

			if (targetBody != null) {
				JointElement targetElement = targetBody.gameObject.GetComponent<JointElement>();
				if (targetElement != null && targetElement.beCut) {
					// this process must be called when check targetElement. So do not process here
				} else {
					// joint as normal body -> check side and set
					bool onLeftSide = blade.GetSide(targetBody.transform.position);
					jointElement.JointWithParent(targetBody, onLeftSide);
				}
			}
		}


		// vs child
		foreach (JointElement otherElement in listJointElement) {
			if (otherElement != jointElement) {
				if (otherElement.mJoint != null && otherElement.mJoint.connectedBody != null) {
					if (otherElement.mJoint.connectedBody.transform == jointElement.transform) {
						otherElement.JointWithParent(jointElement.mLeftBody, jointElement.mRightBody);
						// set new parent for other
//						otherElement.mJoint.transform.SetParent(targetOfOther.transform);
					}
				}
			}
		}
	}
}
