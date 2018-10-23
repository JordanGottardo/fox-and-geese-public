using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxAndGeese;

public class RotatePawn : MonoBehaviour {

	public Vector3 rotationWhenFox;
	public Vector3 rotationWhenGoose;

	/** Rotates this pawn to the correct rotation */
	public void Rotate(PawnType pawnType) {
		Vector3 correctRotation = Vector3.zero;
		if (pawnType == PawnType.Fox) {
			correctRotation = rotationWhenFox;
		}
		else if (pawnType == PawnType.Goose) {
			correctRotation = rotationWhenGoose;
		}
		gameObject.transform.localRotation = Quaternion.Euler(correctRotation);
	}

}
