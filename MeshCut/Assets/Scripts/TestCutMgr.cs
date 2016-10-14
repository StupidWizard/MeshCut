using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TestCutMgr : MonoBehaviour {

	public Blade blade;

	public GameObject boneObject;
	public GameObject meshObject;
	public GameObject cutObject;

	public static bool onDoubleTouch = false;
	public float timeRemainDecideRestart = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.K)) {
			ChangeToCutMode();
		}

		CheckReset ();
	}

	void CheckReset() {
		if (timeRemainDecideRestart > 0) {
			timeRemainDecideRestart -= Time.deltaTime;
		}

		if (HandInputManager.Instance.leftHand.onPressTrigger && HandInputManager.Instance.rightHand.onPressTrigger) {
			RoomController.Instance.ResetPosition ();
			if (!onDoubleTouch) {
				timeRemainDecideRestart = 1.5f;
			}
		}

		if (onDoubleTouch) {
			if (!HandInputManager.Instance.leftHand.onPressTouchPad || !HandInputManager.Instance.rightHand.onPressTouchPad) {
				onDoubleTouch = false;
			}
		} else {
			if (HandInputManager.Instance.leftHand.onPressTouchPad && HandInputManager.Instance.rightHand.onPressTouchPad) {
				onDoubleTouch = true;
				if (timeRemainDecideRestart > 0) {
					SceneManager.LoadScene ("MeshCut");
				}
			}
		}

	}
		
	public bool cutFinished = false;
	void ChangeToCutMode() {
		if (cutFinished) {
			return;
		}
		meshObject.SetActive(false);
		cutObject.SetActive(true);
		Transform[] listBone = boneObject.GetComponentsInChildren<Transform>();
		Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
		foreach (Transform pTrans in listBone) {
			dict.Add(pTrans.name, pTrans);
		}
		Transform[] listBoneCut = cutObject.GetComponentsInChildren<Transform>(true);
		foreach (Transform cutTrans in listBoneCut) {
			string childName = cutTrans.name;
			if (dict.ContainsKey(childName)) {
				Transform mTransTarget = dict[childName];
				cutTrans.localRotation = mTransTarget.localRotation;
				cutTrans.localPosition = mTransTarget.localPosition;
				cutTrans.localScale = mTransTarget.localScale;
			}
		}
		boneObject.SetActive(false);
		cutFinished = true;
		StartCoroutine(Cut());
	}

	IEnumerator Cut() {
		yield return new WaitForEndOfFrame();
		blade.Cut();
	}

//	void OnGUI() {
//		if (GUI.Button(new Rect(10, 10, Screen.width/6, Screen.height/8), "Cut")) {
//			ChangeToCutMode();
//		}
//	}
}
