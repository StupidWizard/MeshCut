using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class HandInputData {

	public string nameHand = "none";

	public Vector2 axis = new Vector2(0, 0);
	public float triggerFog = 0;
	public bool onPressTrigger = false;
	public bool onPressTouchPad = false;
	public bool onPressGrip = false;
	public bool onPressSystem = false;
	public bool onPressMenu = false;

	public bool onPressDownTrigger;
	public bool onPressUpTrigger;

}
