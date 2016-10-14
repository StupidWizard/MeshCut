using UnityEngine;
using System.Collections;

public class RoomController : MonoBehaviour {

	public static RoomController Instance;

	[SerializeField]
	Transform originPos;		// origin pos of CameraView

	[SerializeField]
	float originRotY = 0;		// origin direction of CameraView

	[SerializeField]
	Transform head;				// (Head MUST be child of Room).

	[SerializeField]
	float offsetFloor = 0.0f;	// offset of Vr_Floor (Vr Tracked) vs Real Floor. Only use when debug, at Prod, this parameter must be 0.0f

	void Awake() {
		Instance = this;

		StartCoroutine (IAutoReset());
	}

	IEnumerator IAutoReset() {
		yield return new WaitForSeconds (0.25f);
		ResetPosition ();
	}
	
	void Update() {
		if (Input.GetKeyDown (KeyCode.R)) {
			ResetPosition ();
		}
	}

	public void ResetPosition() {
		float rotY = originRotY - head.localRotation.eulerAngles.y;
		transform.rotation = Quaternion.Euler (0, rotY, 0);

		Vector3 headPos = head.localPosition;
		headPos.y = offsetFloor;
		headPos = transform.position + transform.rotation * headPos;
		Vector3 deltaPos = originPos.position - headPos;

		transform.position = transform.position + deltaPos;
	}
}
