using UnityEngine;
using UnityEngine.Networking;

public class SendToGrave : NetworkBehaviour {

	private bool hasStartedElevating = false;
	private bool isDoneElevating = false;
	private bool hasStartedMovingHorizontally = false;
	private bool isDoneMovingHorizontally = false;
	private bool hasBeenDeactivated = false;
	private Vector3 velocity = Vector3.zero;
	private Vector3 verticalTargetPos;
	[SyncVar]
	public Vector3 horizontalTargetPos;
	public Vector3 verticalElevation = new Vector3(0, 1, 0);
	public float travelTime;
	
	/** Animates the pawn and sends it to the grave when the pawn is eaten */
	void Update () {
		PawnData pawnData = gameObject.GetComponent<PawnData>();
		Draggable draggable = gameObject.GetComponent<Draggable>();
		Vector3 actualPosition = transform.position;

		if (pawnData.IsDead() && !hasStartedMovingHorizontally) {
			if (!hasStartedElevating) {
				verticalTargetPos = actualPosition + verticalElevation;
				hasStartedElevating = true;
			}
			if (Vector3.Distance(actualPosition, verticalTargetPos) > 0.01f) {
				transform.position = Vector3.SmoothDamp(transform.position, 
													verticalTargetPos, ref velocity, travelTime);
			} else {
				isDoneElevating = true;
			}
		}

		if (isDoneElevating && !hasBeenDeactivated) {
			if (!hasStartedMovingHorizontally) {
				hasStartedMovingHorizontally = true;
			}
			if (Vector3.Distance(actualPosition, horizontalTargetPos) > 0.01f) {
				transform.position = Vector3.SmoothDamp(transform.position,
			horizontalTargetPos, ref velocity, travelTime);
			} else {
				isDoneMovingHorizontally = true;
			}
		}

		if (isDoneMovingHorizontally && !hasBeenDeactivated) { 
			hasBeenDeactivated = true;
			Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
			draggable.enabled = false;
		}
	}

	/** Returns true if approxDest is near realDest (with a tolerance) */
	private bool AreVectorsNear(Vector3 approxDest, Vector3 realDest, Vector3 tolerance) {
		Vector3 vectorDiff = realDest - tolerance;
		return (approxDest.x >= vectorDiff.x && approxDest.y >= vectorDiff.y
			&& approxDest.z >= vectorDiff.z);
	}


}
