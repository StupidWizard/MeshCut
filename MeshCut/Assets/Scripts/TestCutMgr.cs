using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TestCutMgr : MonoBehaviour {

	public Blade blade;

	public GameObject boneObject;
	public GameObject meshObject;
	public GameObject cutObject;

	public SwordTracker swordTracker;

	float timeStart = 1.0f;

	// Use this for initialization
	void Start () {
	
	}

	public bool onTrigger = false;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.K)) {
			ChangeToCutMode();
			StartCoroutine (Cut(null));
		}

		if (timeStart > 0) {
			timeStart -= Time.deltaTime;
		} else {
			CheckCut ();
		}



		CheckReset ();
	}

	void CheckCut() {
		if (onTrigger) {
			if (!HandInputManager.Instance.rightHand.onPressTrigger) {
				onTrigger = false;
				swordTracker.StopTracker ();
				ChangeToCutMode ();
				StartCoroutine(Cut(swordTracker.GetTrackerData ()));
			}
		} else {
			if (HandInputManager.Instance.rightHand.onPressTrigger) {
				onTrigger = true;
				swordTracker.StartTracker ();
			}
		}
	}

	void CheckReset() {
		if (HandInputManager.Instance.leftHand.onPressTouchPad && HandInputManager.Instance.rightHand.onPressTouchPad) {
			RoomController.Instance.ResetPosition ();

			if (HandInputManager.Instance.leftHand.onPressTouchPad && HandInputManager.Instance.rightHand.onPressTouchPad) {
				SceneManager.LoadScene ("MeshCut");
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

	}

	IEnumerator Cut(Vector3[] dataTracker) {
		yield return new WaitForEndOfFrame();
		if (dataTracker != null) {
			blade.Cut (dataTracker [0], dataTracker [1], dataTracker [2]);
		} else {
			blade.Cut ();
		}

	}

//	void OnGUI() {
//		if (GUI.Button(new Rect(10, 10, Screen.width/6, Screen.height/8), "Cut")) {
//			ChangeToCutMode();
//		}
//	}
}
