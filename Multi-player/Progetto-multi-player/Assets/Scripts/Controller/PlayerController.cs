using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using FoxAndGeese;
using System;

public class PlayerController : NetworkBehaviour {

    public const string startedNotification = "PlayerController.StartNotification";
    public const string startedLocalNotification = "PlayerController.StartedLocalNotification";
    public const string destroyedNotification = "PlayerController.DestroyedNotification";
    public const string coinTossNotification = "PlayerController.CoinTossNotification";
    public const string pawnMovedNotification = "PlayerController.PawnMovedNotification";
    public const string numberOfGeeseConfirmedNotification =
        "PlayerController.NumberOfGeeseConfirmedNotification";
	public const string endTurnNotification =
	   "PlayerController.EndTurnNotification";
	// number of matches won by the player
	public int score;
    public GameBoard gameBoard;
    public PawnType pawnType;

	/** Invoked whenever a client starts */
    public override void OnStartClient() {
        base.OnStartClient();
        this.PostNotification(startedNotification);
    }

	/** Invoked whenever the local player starts */
    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        this.PostNotification(startedLocalNotification);
    }

	/** Invoked when the gameObject is destroyed */
    private void OnDestroy() {
        this.PostNotification(destroyedNotification);
    }

    public void PlayerMovesThePawn(GameObject obj, Vector3 pos) {
        if (!isLocalPlayer) {
            return;
        }
        CmdPlayerMovesThePawn(obj, pos);
    }

    [Command]
    private void CmdPlayerMovesThePawn(GameObject obj, Vector3 pos) {
		MovementInfo moveInfo = new MovementInfo(pos);
		if (isServer && !isLocalPlayer) {
			moveInfo.isLocalAndClient = true;
		}
        RpcPlayerMovesThePawn(obj, moveInfo);
    }

    [ClientRpc]
    private void RpcPlayerMovesThePawn(GameObject obj, MovementInfo moveInfo) { 
        if (isLocalPlayer && !isServer && moveInfo.isLocalAndClient) {
            return; //Movement has already been done in this client; disregard this movement
        }
        obj.GetComponent<Draggable>().MovePawn(moveInfo.pos);
    }

    public void UpdatePawnCoordinates(PawnData pawn, int x, int z) {
        CmdUpdatePawnCoordinates(pawn.gameObject, x, z);
    }

    [Command]
    private void CmdUpdatePawnCoordinates(GameObject pawnObj, int x, int z) {
        PawnData pawn = pawnObj.GetComponent<PawnData>();
        pawn.x = x;
        pawn.z = z;
    }

    public void PawnMoved(Move move) {
        CmdPawnMoved(move);
    }

    [Command]
    private void CmdPawnMoved(Move move) {
        RpcPawnMoved(move);
    }

    [ClientRpc]
    private void RpcPawnMoved(Move move) {
        this.PostNotification(pawnMovedNotification, move);
    }

    [Command]
    public void CmdCoinToss() {
        RpcCoinToss(UnityEngine.Random.value < 0.5);
    }

    [ClientRpc]
    private void RpcCoinToss(bool coinToss) {
        this.PostNotification(coinTossNotification, coinToss);
    }

    [Command]
    public void CmdSendNumberOfGeese(int numberOfGeese) {
        RpcSendNumberOfGeese(numberOfGeese);
    }

    [ClientRpc]
    private void RpcSendNumberOfGeese(int numberOfGeese) {
        this.PostNotification(numberOfGeeseConfirmedNotification, numberOfGeese);
    }

	[Command]
	public void CmdEndTurn() {
		RpcEndTurn();
	}

	[ClientRpc]
	public void RpcEndTurn() {
		this.PostNotification(endTurnNotification);
	}

}
