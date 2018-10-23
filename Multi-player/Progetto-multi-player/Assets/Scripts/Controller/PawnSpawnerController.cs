using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using FoxAndGeese;

public class PawnSpawnerController : NetworkBehaviour {

	public GameBoard gameBoard;
	public GameObject foxPrefab;
	public GameObject goosePrefab;
	public GameObject graveOfTheGeese;
	public Tile refTile;

	/** Invoked before Start() */
	private void OnEnable() {
		this.AddObserver(OnPlacePawn, Game.placePawnNotification);
		this.AddObserver(OnPawnEaten, Game.pawnEatenNotification);
	}

	private void OnDisable() {
		this.RemoveObserver(OnPlacePawn, Game.placePawnNotification);
		this.AddObserver(OnPawnEaten, Game.pawnEatenNotification);
	}

	/** Invoked whenever a pawn is placed at the start of the game */
	private void OnPlacePawn(object sender, object args) {
		Placement placement = (Placement)args;
		GameObject pawnToSpawn = gameBoard.PlacePawn(placement, ref refTile);
		PawnData pawnData = pawnToSpawn.GetComponent<PawnData>();
		pawnData.x = refTile.x;
		pawnData.z = refTile.z;
		NetworkServer.Spawn(pawnToSpawn);
	}

	/** Invoked whenever a pawn is eaten */
	private void OnPawnEaten(object sender, object args) {
		Vector2 coord = (Vector2)args;
		int x = (int)coord.x;
		int z = (int)coord.y;
		PawnData pawnData = gameBoard.GetPawnAtCoord(x, z);
		GameObject pawn = pawnData.gameObject;
		SendToGrave sendToGrave = pawn.GetComponent<SendToGrave>();
		pawnData.x = -1;
		pawnData.z = -1;
		sendToGrave.horizontalTargetPos = graveOfTheGeese.transform.position 
			+ sendToGrave.verticalElevation;
	}

	/** Destroys every pawn */
	public void ClearBoard() {
		PawnData[] pawns =  FindObjectsOfType<PawnData>();
		foreach (PawnData pawn in pawns) {
			Destroy(pawn.gameObject);
		}
	}


}
