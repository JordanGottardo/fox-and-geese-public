using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxAndGeese;
using UnityEngine.UI;
using System;

public class GameController : StateMachine {

	public Game game;
	public GameBoard gameBoard;
	public Settings settings;
	public MatchController matchController;
	public PawnSpawnerController pawnSpawnerController;
	public Text localPlayerLabel;
	public Text remotePlayerLabel;
	public Text gameStateLabel;
	public Text hintLabel;
	public GameObject serverGuiContainer;
	public GameObject endTurnButton;
	public GameObject mainCamera;

	/** Called when the components enabled, before Start() */
	private void OnEnable() {
		this.AddObserver(OnMatchReady, MatchController.matchReadyNotification);
		this.AddObserver(OnMatchNoLongerReady, MatchController.matchNoLongerReadyNotification);
		this.AddObserver(OnConfigure, MatchController.configureNotification);
		this.AddObserver(OnCoinToss, PlayerController.coinTossNotification);
		this.AddObserver(OnPawnMoved, PlayerController.pawnMovedNotification);
		this.AddObserver(OnChangeTurn, Game.changeTurnNotification);
		this.AddObserver(OnEndGame, Game.endGameNotification);
		this.AddObserver(OnBeginGame, Game.beginGameNotification);
		this.AddObserver(OnConfirmNumberOfGeese, Menu.confirmNumberOfGeeseNotification);
		this.AddObserver(OnNumberOfGeeseConfirmed, PlayerController.numberOfGeeseConfirmedNotification);
		this.AddObserver(OnEatAgain, Game.canEatAnotherTimeNotification);
		this.AddObserver(OnEndTurn, PlayerController.endTurnNotification);
	}

	private void OnDisable() {
		this.RemoveObserver(OnMatchReady, MatchController.matchReadyNotification);
		this.RemoveObserver(OnMatchNoLongerReady, MatchController.matchNoLongerReadyNotification);
		this.RemoveObserver(OnConfigure, MatchController.configureNotification);
		this.RemoveObserver(OnCoinToss, PlayerController.coinTossNotification);
		this.RemoveObserver(OnPawnMoved, PlayerController.pawnMovedNotification);
		this.RemoveObserver(OnChangeTurn, Game.changeTurnNotification);
		this.RemoveObserver(OnEndGame, Game.endGameNotification);
		this.RemoveObserver(OnBeginGame, Game.beginGameNotification);
		this.RemoveObserver(OnConfirmNumberOfGeese, Menu.confirmNumberOfGeeseNotification);
		this.RemoveObserver(OnNumberOfGeeseConfirmed, PlayerController.numberOfGeeseConfirmedNotification);
		this.RemoveObserver(OnEatAgain, Game.canEatAnotherTimeNotification);
		this.RemoveObserver(OnEndTurn, PlayerController.endTurnNotification);
	}

	private void Awake() {
		CheckState();
	}

	private void OnBeginGame(object sender, object args) {
		StartCoroutine(this.RotatePawns(matchController.localPlayer.pawnType));
	}

	/** Called when the clients disconnects before the host has chosen geese number*/
	private void OnMatchNoLongerReady(object sender, object args) {
		ClearBoard();
		CheckState();
	}

	private void OnChangeTurn(object sender, object args) {
		if (endTurnButton.activeSelf) {
			endTurnButton.SetActive(false);
		}
		CheckState();
	}

	private void OnEndGame(object sender, object args) {
		if (endTurnButton.activeSelf) {
			endTurnButton.SetActive(false);
		}
		CheckState();
	}

	private void OnConfigure(object sender, object args) {
		CheckState();
	}

	private void CheckState() {
		if (matchController.IsOffline) {
			ChangeState<OfflineGameState>();
		}
		else if (!matchController.IsReady) {
			ChangeState<LoadGameState>();
		}
		else if (game == null) {
			ChangeState<ChooseGeeseGameState>();
		}
		else if (game.turn == PawnType.None) {
			ChangeState<EndGameState>();
		}
		else if (game.turn == matchController.localPlayer.pawnType) {
			ChangeState<ActiveGameState>();
		}
		else {
			ChangeState<PassiveGameState>();
		}
	}

	private void OnMatchReady(object sender, object args) {
		game = null;
		CheckState();
	}					

	/** Receives the results of the coin toss*/
	private void OnCoinToss(object sender, object args) {
		bool coinToss = (bool)args;
		matchController.hostPlayer.pawnType = coinToss ? PawnType.Fox : PawnType.Goose;
		matchController.clientPlayer.pawnType = coinToss ? PawnType.Goose : PawnType.Fox;
		ClearBoard();
		PawnType localPawnType = matchController.localPlayer.pawnType;
		mainCamera.GetComponent<MoveCamera>().PositionCamera(localPawnType);
		game = new Game(settings.geeseNumber);
		CheckState();
	}

	/** Invoked host side when the number of geese has been chosen */
	private void OnConfirmNumberOfGeese(object sender, object args) {
		PlayerController pc = matchController.localPlayer;
		if (pc != null && pc.isServer) {
			int numberOfGeese = (int)args;
			pc.CmdSendNumberOfGeese(numberOfGeese);
		}
	}

	/** Invoked client side when the number of geese has been chosen */
	private void OnNumberOfGeeseConfirmed(object sender, object args) {
		int numberOfGeese = (int)args;
		settings = new Settings();
		settings.geeseNumber = numberOfGeese;
		PlayerController clientPlayer = matchController.clientPlayer;

		if (clientPlayer != null && clientPlayer.isLocalPlayer) {
			matchController.clientPlayer.CmdCoinToss();
		}
	}

	public void SetGameToNull() {
		game = null;
		CheckState();
	}

	/** Manages the drag of the pawns and their physical movement */
	public void MovePawn(object sender, object args) {
		Draggable draggablePawn = (Draggable)sender;
		PawnData pawn = draggablePawn.gameObject.GetComponent<PawnData>();
		PlayerController localPlayer = matchController.localPlayer;
		if (!IsPawnTypeValid(pawn, localPlayer.pawnType)) {
			//StartCoroutine(WriteHint("You can only move a pawn of your type!", 3));
			return;
		}
		GameObject obj = pawn.gameObject;
		Vector3 pos = (Vector3)args;
        pawn.gameObject.transform.localPosition = pos; // moves the pawn locally (client side)
		matchController.localPlayer.PlayerMovesThePawn(draggablePawn.gameObject, pos); // moves the pawn over the network
	}

	/** Manages when the user releases the mouse and tries to make a move */
	public void ConfirmMovePawn(object sender, object args) {
		PlayerController localPlayer = matchController.localPlayer;
		GameObject obj = ((Draggable)sender).gameObject;
		PawnData pawn = obj.GetComponent<PawnData>();
		Tile initialTile = pawn.GetContainingTile(); // gets the pawn previous position (Tile)
		Vector3 finalPos = (Vector3)args;
		Tile finalTile = gameBoard.GetTileNearWorldPosition(finalPos); // finds the Tile where the user wants to move the pawn
		Vector3 oldPos = ((Draggable)sender).oldPos;
		Vector3 realFinalPos = oldPos;
		if (finalTile != null) {
			Move move = new Move(pawn.pawnType, initialTile.x, initialTile.z, finalTile.x, finalTile.z);
			if (game.IsMoveValid(move)) {
				realFinalPos = finalTile.transform.localPosition;
				localPlayer.UpdatePawnCoordinates(pawn, finalTile.x, finalTile.z);
				localPlayer.PawnMoved(move); 
			}
		}
		realFinalPos.y = oldPos.y;
		obj.transform.localPosition = realFinalPos; // moves the pawn locally (client side)
		localPlayer.PlayerMovesThePawn(pawn.gameObject, realFinalPos); // moves the pawn over the network
	}

	/** Called when the Fox can eat again */
	private void OnEatAgain(object sender, object args) {
		if (matchController.localPlayer.pawnType == PawnType.Fox) {
			endTurnButton.SetActive(true);
		}
	}

	public void EndTurn() {
		endTurnButton.SetActive(false);
		matchController.localPlayer.CmdEndTurn();
	}

	private void OnEndTurn(object sender, object args) {
		game.ChangeTurn();
	}

	private void OnPawnMoved(object sender, object args) {
		Move move = (Move)args;
		game.MovePawn(move);
	}

	private bool IsPawnTypeValid(PawnData pawnData, PawnType pawnType) {
		return pawnData.pawnType == pawnType;
	}

	/** Clear the (graphical) gameBoard */
	private void ClearBoard() {
		PlayerController localPlayer = matchController.localPlayer;
		if (localPlayer != null && localPlayer.isServer) {
			pawnSpawnerController.ClearBoard();
		}
	}

	/** Writes a hint on the screen */
	private IEnumerator WriteHint(String text, int duration) {
		hintLabel.text = text;
		yield return new WaitForSeconds(duration);
		hintLabel.text = "";
	}

	/** Rotates pawns */
	private IEnumerator RotatePawns(PawnType pawnType) {
		yield return new WaitForSeconds(0.3f);
		RotatePawn[] pawns = FindObjectsOfType<RotatePawn>();
		foreach (RotatePawn pawn in pawns) {
			pawn.Rotate(matchController.localPlayer.pawnType);
		}
	}
}
