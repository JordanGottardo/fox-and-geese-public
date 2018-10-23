using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePawnAnimation : MonoBehaviour {

	public bool moveToDestination = false;
	public float travelTime;
	public Vector3 velocity = Vector3.zero;

	void Update() {
		if (!moveToDestination) {
			return;
		}
		GameBoard gameBoard = FindObjectOfType<GameBoard>();
		PawnData pawnData = GetComponent<PawnData>();
		Tile tile = gameBoard.GetCorrespondingTile(pawnData.x, pawnData.z);
		Vector3 actualPos = transform.position;
		Vector3 finalPos = tile.transform.position;
		if (Vector3.Distance(actualPos, finalPos) > 0.01f) {
			transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref velocity, travelTime);
		}
		else {
			velocity = Vector3.zero;
			moveToDestination = false;
		}

	}
}
