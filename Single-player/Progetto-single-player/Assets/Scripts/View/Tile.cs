using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public int x; // x (logic) coordinate of the tile 
	public int z; //z (logic) coordinate of the tile 
	public GameObject containedPawn;

	// Returns true if this tile is affected by move
	public bool IsAffectedByPlacement(Placement move) {
		return (x == move.x && z == move.z);
	}
}
