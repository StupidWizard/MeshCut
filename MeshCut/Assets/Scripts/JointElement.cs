using UnityEngine;
using System.Collections;

public class JointElement : MonoBehaviour {

	[SerializeField]
	public Collider mCollider;		// hard set:

	[SerializeField]
	public Rigidbody mBody;

	public Rigidbody mNewBody;

	[SerializeField]
	public CharacterJoint mJoint;

//	public Rigidbody leftBody;
//	public Rigidbody rightBody;

	// Use this for initialization
	void Start () {
		InitPara();
	}

	void InitPara() {
		if (mBody == null) {
			mBody = gameObject.GetComponent<Rigidbody>();
		}
		mNewBody = mBody;

		if (mJoint == null) {
			mJoint = gameObject.GetComponent<CharacterJoint>();
		}
	}

	public void UpdateData(Rigidbody leftBody, Rigidbody rightBody) {
//		this.leftBody = leftBody;
//		this.rightBody = rightBody;
	}

	public void UpdateData(Rigidbody pNewBody) {
		mNewBody = pNewBody;
		if (mJoint != null) {
			CharacterJoint newJoint = mNewBody.gameObject.AddComponent<CharacterJoint>();
			if (mJoint.connectedBody != null) {
				newJoint.connectedBody = mJoint.connectedBody;
			}

			newJoint.anchor = mJoint.anchor;
			newJoint.axis = mJoint.axis;
			Debug.LogError(gameObject.name + " update data, newBOdy = " + mNewBody + " newJoint = " + newJoint.name);
			mJoint = newJoint;
		}
	}

	public void ClearOldObject() {
//		Debug.LogError("Clear for " + gameObject.name + " nChild = " + transform.childCount);
		if (mNewBody != mBody) {
			Collider collider = mBody.gameObject.GetComponent<Collider>();
			Joint joint = mBody.gameObject.GetComponent<CharacterJoint>();
			if (collider != null) {
				GameObject.Destroy(collider);
			}
			if (joint != null) {
				GameObject.DestroyImmediate(joint);
			}
			GameObject.Destroy(mBody);
		}
		transform.DetachChildren();

//		for (int i = 0; i < transform.childCount; i++) {
//			Transform child = transform.DetachChildren();
//			Debug.LogError("set out for " + child.name + " " + i);
//			child.SetParent(null);
//		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
