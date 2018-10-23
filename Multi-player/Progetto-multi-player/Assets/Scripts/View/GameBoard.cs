using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using FoxAndGeese;

public class GameBoard : MonoBehaviour {

	public List<Tile> tiles;
	public GameObject foxPrefab;
	public GameObject goosePrefab;
	public float nearDiff;
	
	/** Invoked before Start(). Finds and saves all tiles */
	void Awake () {
		Tile[] tileArray = GetComponentsInChildren<Tile>();
		tiles = new List<Tile>(tileArray);
	}

	/** Returns the tile positioned in (x, z) */
	public Tile GetCorrespondingTile(int x, int z) {
		foreach (Tile tile in tiles) {
			if (tile.IsAffectedByPlacement(new Placement(PawnType.None, x, z))) {
				return tile;
			}
		}
		return null;
	}

    /** Creates a pawn on the coordinates specified by move */
	public GameObject PlacePawn(Placement move, ref Tile refTile) {
		GameObject obj = null;
		foreach (Tile tile in tiles) {
			if (tile.IsAffectedByPlacement(move)) {
				refTile = tile;
				GameObject prefabToInstantiate = null;

				if (move.pawnType == PawnType.Fox) {
					prefabToInstantiate = foxPrefab;
				}
				else if (move.pawnType == PawnType.Goose) {
					prefabToInstantiate = goosePrefab;
				}
				Vector3 position = new Vector3(tile.transform.localPosition.x,
					prefabToInstantiate.transform.localPosition.y, tile.transform.localPosition.z);
				obj = Instantiate(prefabToInstantiate, position, prefabToInstantiate.transform.rotation);
				obj.GetComponent<PawnData>().containingTile = tile;
				tile.containedPawn = obj;
				break;
			}
		}
		return obj;
	}

	/** Returns a tile near worldPos, null if there is none */
	public Tile GetTileNearWorldPosition(Vector3 worldPos) {
		foreach (Tile tile in tiles) {
			Bounds tileSize = tile.GetComponent<Renderer>().bounds;
			Vector3 extents = tileSize.extents;
			Vector3 tilePosition = tile.transform.position;
			if (IsNear(worldPos.x, tilePosition.x, extents.x) && IsNear(worldPos.z, tilePosition.z, extents.z)) {
				return tile;
			}
		}
		return null;
	}

	/** Returns the pawn at (x, z) */
	public PawnData GetPawnAtCoord(int x, int z) {
		PawnData[] pawns = FindObjectsOfType<PawnData>();
		foreach (PawnData pawn in pawns) {
			if (pawn.IsAtCoord(x, z)) {
				return pawn;
			}
			
		}
		return null;
	}

	/** Returns true if worldCoord is inside the interval [center-extent-nearDiff, center+extent+nearDiff] */
	private bool IsNear(float worldCoord, float center, float extent) {
		return (worldCoord <= (center + extent + nearDiff) && worldCoord >= (center - extent - nearDiff));
	}

}
