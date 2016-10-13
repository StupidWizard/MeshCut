using UnityEngine;
using System.Collections;

public class JointElement : MonoBehaviour {
	public Collider mCollider;		// hard set: the body which can be cut to left Part and right Part.
	public JointPlane mJointPlane;	// hard set: check the plane of Joint be cut or not. (if cut -> joint both of left and right part)

	Rigidbody mBody;				// Rigidbody of object
	public CharacterJoint mJoint;	// joint of object
	public bool onLeftSide;

	public bool beCut = false;

	public Rigidbody mLeftBody = null;
	CharacterJoint mLeftJoint = null;

	public Rigidbody mRightBody = null;
	CharacterJoint mRightJoint = null;

	// Use this for initialization
	void Start () {
		InitPara();
	}

	void InitPara() {
		if (mBody == null) {
			mBody = gameObject.GetComponent<Rigidbody>();
		}

		if (mJoint == null) {
			mJoint = gameObject.GetComponent<CharacterJoint>();
		}
	}
		

	/// <summary>
	/// Cuts to parts. mBody had been cut to 2 parts: Left and Right.
	/// </summary>
	/// <param name="leftPart">Left part.</param>
	/// <param name="rightPart">Right part.</param>
	public void CutToParts(GameObject leftPart, GameObject rightPart) {
		beCut = true;
		mLeftBody = leftPart.GetComponent<Rigidbody>();
		mRightBody = rightPart.GetComponent<Rigidbody>();
		if (mLeftBody == null) {
			mLeftBody = leftPart.AddComponent<Rigidbody>();
		}
		if (mRightBody == null) {
			mRightBody = rightPart.AddComponent<Rigidbody>();
		}
	}


	/// <summary>
	/// Checks the side. if mBody has not been cut to 2 parts (Left and Right), => check it side vs the blade
	/// </summary>
	/// <param name="blade">Blade cut the other object. </param>
	public void CheckSide(Plane blade) {
		onLeftSide = blade.GetSide(transform.position);
	}
	


	/// <summary>
	/// Joints the with parent. mBody HAD BEEN CUT to L_Part and R_Part but parent has not been cut.
	/// if Parent on Left -> select LeftPart to joint with parent and Else. (joint data will be copy from mJoint)
	/// </summary>
	/// <param name="parent">Parent.</param>
	/// <param name="onLeftSide">If set to <c>true</c>, parent is on left side.</param>
	public void JointWithParent(Rigidbody parent, bool onLeftSide) {
		if (!beCut) return;		// only process when mBody had been cut
		if (onLeftSide) {
			mLeftJoint = mLeftBody.gameObject.GetComponent<CharacterJoint>();
			if (mLeftJoint == null) {
				mLeftJoint = mLeftBody.gameObject.AddComponent<CharacterJoint>();
			}

			// copy
			mLeftJoint.connectedBody = parent;
			mLeftJoint.anchor = mJoint.anchor;
			mLeftJoint.axis = mJoint.axis;
		} else {
			mRightJoint = mRightBody.gameObject.GetComponent<CharacterJoint>();
			if (mRightJoint == null) {
				mRightJoint = mRightBody.gameObject.AddComponent<CharacterJoint>();
			}

			// copy
			mRightJoint.connectedBody = parent;
			mRightJoint.anchor = mJoint.anchor;
			mRightJoint.axis = mJoint.axis;
		}
	}


	/// <summary>
	/// Joints the with parent. parent had been cut to L_Part and R_Part.
	/// </summary>
	/// <param name="leftPartOfParent">Left part of parent.</param>
	/// <param name="rightPartOfParent">Right part of parent.</param>
	public void JointWithParent(Rigidbody leftPartOfParent, Rigidbody rightPartOfParent) {
		if (beCut) {
			// joint L_part vs L_partOfParent and R_part vs R_partOfParent

			// LEFT
			if (mJointPlane.beCut || onLeftSide) {
				mLeftJoint = mLeftBody.gameObject.GetComponent<CharacterJoint>();
				if (mLeftJoint == null) {
					mLeftJoint = mLeftBody.gameObject.AddComponent<CharacterJoint>();
				}
				mLeftJoint.connectedBody = leftPartOfParent;
				mLeftJoint.anchor = mJoint.anchor;
				mLeftJoint.axis = mJoint.axis;
			}

			// RIGHT
			if (mJointPlane.beCut || !onLeftSide) {
				mRightJoint = mRightBody.gameObject.GetComponent<CharacterJoint>();
				if (mRightJoint == null) {
					mRightJoint = mRightBody.gameObject.AddComponent<CharacterJoint>();
				}
				mRightJoint.connectedBody = rightPartOfParent;
				mRightJoint.anchor = mJoint.anchor;
				mRightJoint.axis = mJoint.axis;
			}
		} else {
			// if onLeftSide -> joint vs L_partOfParent and else.
			mJoint.connectedBody = onLeftSide? leftPartOfParent : rightPartOfParent;
		}
	}


	public void ClearOldObject() {
		if (beCut) {
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
	}
}
