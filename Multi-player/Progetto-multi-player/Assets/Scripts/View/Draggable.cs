using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using FoxAndGeese;

public class Draggable : NetworkBehaviour {

    private Vector3 dist;
    private float posX;
    private float posY;

    public Vector3 oldPos;
    public const string movePawnNotification = "Draggable.MovePawnNotification";
    public const string confirmMovePawnNotification = "Draggable.ConfirmMovePawnNotification"; //whenever the user releases the mouse

	/** Saves starting position whenever the user clicks on a pawn */
    void OnMouseDown() {
        oldPos = transform.localPosition;
        dist = Camera.main.WorldToScreenPoint(transform.position);
        posX = Input.mousePosition.x - dist.x;
        posY = Input.mousePosition.y - dist.y;
    }
	
	/** Moves the pawn whenever the user drags it after having clicked on it */
    void OnMouseDrag() {
        Vector3 curPos =
                  new Vector3(Input.mousePosition.x - posX,
                  Input.mousePosition.y - posY, dist.z);
        Vector3 worldpos = Camera.main.ScreenToWorldPoint(curPos);
        worldpos.y = transform.localPosition.y;
        this.PostNotification(movePawnNotification, worldpos);
    }

	/** Tries to move the pawn whenever the user releases the mouse */
    private void OnMouseUp() {
        this.PostNotification(confirmMovePawnNotification, transform.position);
    }

	/** Moves the pawn to the pos position */
    public void MovePawn(Vector3 pos) {
		transform.localPosition = pos;
	}
}