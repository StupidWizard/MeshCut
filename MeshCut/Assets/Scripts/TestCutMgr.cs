using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCutMgr : MonoBehaviour {

	public Blade blade;

	public GameObject boneObject;
	public GameObject meshObject;
	public GameObject cutObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void ChangeToCutMode() {
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

		StartCoroutine(Cut());
	}

	IEnumerator Cut() {
		yield return new WaitForEndOfFrame();
		blade.Cut();
	}

	void OnGUI() {
		if (GUI.Button(new Rect(10, 10, Screen.width/6, Screen.height/8), "Cut")) {
			ChangeToCutMode();
//			blade.Cut();
		}
	}
}
