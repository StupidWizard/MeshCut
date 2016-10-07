using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CutGroup {
	public GameObject origin;
	public GameObject left;			// true vs plane
	public GameObject right;		// false vs plane
}

public class JointCutBody : MonoBehaviour {

	public JointElement[] listJointElement;

	public List<CutGroup> listCutGroup = new List<CutGroup>();

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
		var cutGroup = new CutGroup();
		cutGroup.origin = origin;
		cutGroup.left = left;
		cutGroup.right = right;
		listCutGroup.Add(cutGroup);
	}

	public void ProcessAfterCut() {
		Debug.LogError("Start Process: update main body and joint for all cutGroup");
		foreach (CutGroup cutGroup in listCutGroup) {
			JointElement jointElement = cutGroup.origin.transform.parent.gameObject.GetComponent<JointElement>();
			if (jointElement != null) {
				UpdateJointNode(cutGroup, jointElement);
			}
		}

		Debug.LogError("During Process: update target and co-target for cutGroup");
		foreach (CutGroup cutGroup in listCutGroup) {
			JointElement jointElement = cutGroup.origin.transform.parent.gameObject.GetComponent<JointElement>();
			if (jointElement != null) {
				ProcessJointNode(cutGroup, jointElement);
			}
		}

		Debug.LogError("End Process: remove unuse body");
		foreach (CutGroup cutGroup in listCutGroup) {
			JointElement jointElement = cutGroup.origin.transform.parent.gameObject.GetComponent<JointElement>();
			if (jointElement != null) {
				jointElement.ClearOldObject();
			}
		}
	}

	void UpdateJointNode(CutGroup cutGroup, JointElement jointElement) {
		bool isLeft = blade.GetSide(jointElement.transform.position);
		GameObject replace = isLeft? cutGroup.left : cutGroup.right;
		jointElement.UpdateData(replace.GetComponent<Rigidbody>());
//		jointElement.UpdateData(cutGroup.left.GetComponent<Rigidbody>(), cutGroup.right.GetComponent<Rigidbody>());
	}

	void ProcessJointNode(CutGroup cutGroup, JointElement jointElement) {
		// check side

		if (jointElement.mJoint != null) {
			// TODO - body is new body
			Rigidbody targetBody = jointElement.mJoint.connectedBody;

			if (targetBody != null) {
				JointElement targetElement = targetBody.gameObject.GetComponent<JointElement>();
				if (targetElement != null) {
					Debug.LogError(jointElement.mJoint.name + " Update joint vs " + targetElement.mNewBody.name);
					jointElement.mJoint.connectedBody = targetElement.mNewBody;
				}
			}

		}


		foreach (JointElement otherElement in listJointElement) {
			if (otherElement != jointElement) {
				if (otherElement.mJoint != null && otherElement.mJoint.connectedBody != null) {
					if (otherElement.mJoint.connectedBody.transform == jointElement.transform) {
						// set new target for other
						bool isOtherLeft = blade.GetSide(otherElement.transform.position);
						GameObject targetOfOther = isOtherLeft? cutGroup.left : cutGroup.right;
						otherElement.mJoint.connectedBody = targetOfOther.GetComponent<Rigidbody>();
//						otherElement.mJoint.transform.SetParent(targetOfOther.transform);
					}
				}
			}
		}
	}
}
