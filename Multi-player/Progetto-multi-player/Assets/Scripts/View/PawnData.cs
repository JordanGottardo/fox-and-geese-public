using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using FoxAndGeese;

public class PawnData : NetworkBehaviour {

	public PawnType pawnType;
	public Tile containingTile;
	[SyncVar]
	public int x;
	[SyncVar]
	public int z;

	/** Returns the tile this pawn is on */
	public Tile GetContainingTile() {
		GameBoard gameBoard = FindObjectOfType<GameBoard>();
		return gameBoard.GetCorrespondingTile(x, z);
	}

	/** Returns true if this pawn is at (x, z) */
	public bool IsAtCoord(int x, int z) {
		return (this.x == x && this.z == z);
	}

	/** Returns true if this pawn has already been eaten */
	public bool IsDead() {
		return (x == -1 && z == -1);
	}
}
