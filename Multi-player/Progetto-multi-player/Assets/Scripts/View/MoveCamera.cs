using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxAndGeese;

public class MoveCamera : MonoBehaviour {

	public GameObject cameraTarget;
	public GameObject mainCamera { get { return this.gameObject; } }
	public Vector3 foxCameraTargetPos;
	public Vector3 foxCameraTargetRot;
	public Vector3 foxCameraPos;
	public Vector3 foxCameraRot;
	public Vector3 gooseCameraTargetPos;
	public Vector3 gooseCameraTargetRot;
	public Vector3 gooseCameraPos;
	public Vector3 gooseCameraRot;
	public Vector3 velocity;
	public float floatVelocity = 0;
	private PawnType pawnType;
	private bool startRotating;
		
	/** Sets pawnType and startRotating to true. This method starts the rotation carried out in the Update method */
	public void PositionCamera(PawnType pawnType) {
		this.pawnType = pawnType;
		startRotating = true;
	}

	/** Rotates the camera when needed */
	private void Update() {
		if (pawnType == PawnType.None) {
			return;
		}
		if (!startRotating) {
			return;
		}

		Vector3 correctRotation = Vector3.zero;
		if (pawnType == PawnType.Fox) {
			correctRotation = foxCameraRot;
		}
		else {
			correctRotation = gooseCameraRot;
		}

		if (pawnType == PawnType.Fox) {
			cameraTarget.transform.localPosition = foxCameraTargetPos;
			cameraTarget.transform.localRotation = Quaternion.Euler(foxCameraTargetRot);
			mainCamera.transform.localPosition = foxCameraPos;
			
		}
		else if (pawnType == PawnType.Goose) {
			cameraTarget.transform.localPosition = gooseCameraTargetPos;
			cameraTarget.transform.localRotation = Quaternion.Euler(gooseCameraTargetRot);
			mainCamera.transform.localPosition = gooseCameraPos;
		}
		mainCamera.transform.localRotation = Quaternion.Euler(
				new Vector3(correctRotation.x, correctRotation.y,
				mainCamera.transform.localRotation.eulerAngles.z));

		if (Mathf.Abs(mainCamera.transform.localRotation.eulerAngles.z - correctRotation.z) > 0.1f) {
			mainCamera.transform.localRotation =
			Quaternion.Euler(new Vector3(correctRotation.x, correctRotation.y, Mathf.SmoothDamp(mainCamera.transform.localRotation.eulerAngles.z,
			correctRotation.z, ref floatVelocity, 0.2f)));
		} else {
			startRotating = false;
		}
	}
}
