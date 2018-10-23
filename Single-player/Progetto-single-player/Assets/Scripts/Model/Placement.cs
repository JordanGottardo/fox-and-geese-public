using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxAndGeese;

public class Placement {

	public PawnType pawnType; // type of the placed pawn
	public int x; // x coordinate of the placement
	public int z; // z coordinate of the placement

	public Placement() {
	}

	public Placement(PawnType pawnType, int x, int z) {
		this.pawnType = pawnType;
		this.x = x;
		this.z = z;
	}

}
