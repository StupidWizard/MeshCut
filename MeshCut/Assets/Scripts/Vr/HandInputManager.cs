using UnityEngine;
using System.Collections;
using Valve.VR;


public class HandInputManager : MonoBehaviour {

	public static HandInputManager Instance;

	[SerializeField]
	SteamVR_ControllerManager steamVR_ControllerMgr;

	SteamVR_TrackedObject leftTrack = null;
	SteamVR_TrackedObject rightTrack = null;

	public HandInputData leftHand;
	public HandInputData rightHand;

	// Use this for initialization
	void Start () {
		Instance = this;

//		leftHand = gameObject.AddComponent<HandInputData>();
		leftHand = new HandInputData();
		leftHand.nameHand = "Left";
//		rightHand = gameObject.AddComponent<HandInputData> ();
		rightHand = new HandInputData();
		rightHand.nameHand = "right";
	}
	
	// Update is called once per frame
	void Update () {
		CheckDevice ();

		GetHandInputData (leftTrack, leftHand);
		GetHandInputData (rightTrack, rightHand);
	}

	public HandInputData HandData(SteamVR_TrackedObject.EIndex index) {
		if (leftTrack != null && leftTrack.index == index) {
			return leftHand;
		}

		return rightHand;
	}

	public void VibrateTrigger(SteamVR_TrackedObject.EIndex index, ushort microSeconds) {
		var device = SteamVR_Controller.Input ((int)index);
		device.TriggerHapticPulse (microSeconds, EVRButtonId.k_EButton_SteamVR_Trigger);
	}

	public void VibrateTouchPad(SteamVR_TrackedObject.EIndex index, ushort microSeconds) {
		var device = SteamVR_Controller.Input ((int)index);
		device.TriggerHapticPulse (microSeconds, EVRButtonId.k_EButton_SteamVR_Touchpad);
	}

	void GetHandInputData(SteamVR_TrackedObject handTrack, HandInputData data) {
		if (handTrack != null && (int)handTrack.index > 0) {
			var device = SteamVR_Controller.Input ((int)handTrack.index);
			data.triggerFog = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
			data.onPressTrigger = device.GetPress (SteamVR_Controller.ButtonMask.Trigger);
			data.onPressDownTrigger = device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger);
			data.onPressUpTrigger = device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger);
			data.onPressTouchPad = device.GetPress (SteamVR_Controller.ButtonMask.Touchpad);
			if (data.onPressTouchPad) {
				data.axis = device.GetAxis ();
			}
			data.onPressGrip = device.GetPress (SteamVR_Controller.ButtonMask.Grip);
			data.onPressSystem = device.GetPress (SteamVR_Controller.ButtonMask.System);
			data.onPressMenu = device.GetPress (SteamVR_Controller.ButtonMask.ApplicationMenu);
		}
	}

	void CheckDevice() {
		if (leftTrack == null) {
			if (steamVR_ControllerMgr.left != null) {
				leftTrack = steamVR_ControllerMgr.left.GetComponent<SteamVR_TrackedObject> ();
			}
		}

		if (rightTrack == null) {
			if (steamVR_ControllerMgr.right != null) {
				rightTrack = steamVR_ControllerMgr.right.GetComponent<SteamVR_TrackedObject> ();
			}
		}
	}

}
