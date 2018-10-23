using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxAndGeese;
using UnityEngine.UI;
using System;

public class GameController : StateMachine {

	public Game game;
	public GameBoard gameBoard;
	public PawnType humanPlayer;
	public PawnType cpuPlayer;
	public int humanPlayerScore;
	public int cpuPlayerScore;
	public AlphaBeta alphaBeta;
	public PawnSpawnerController pawnSpawnerController;
	public Text humanPlayerLabel;
	public Text cpuPlayerLabel;
	public Text gameStateLabel;
	public Text hintLabel;
	public GameObject choosePlayerGUIContainer;
	public GameObject mainCamera;

	/** Called when the components enabled, before Start() */
	private void OnEnable() {
		this.AddObserver(OnPlayerChosen, Menu.confirmPlayerChosenNotification);
		this.AddObserver(OnFinishedPlacingPawns, Game.finishedPlacingPawnsNotification);

		this.AddObserver(OnChangeTurn, Game.changeTurnNotification);
		this.AddObserver(OnEndGame, Game.endGameNotification);
	}

	private void OnDisable() {
		this.RemoveObserver(OnPlayerChosen, Menu.confirmPlayerChosenNotification);
		this.RemoveObserver(OnFinishedPlacingPawns, Game.finishedPlacingPawnsNotification);

		this.RemoveObserver(OnChangeTurn, Game.changeTurnNotification);
		this.RemoveObserver(OnEndGame, Game.endGameNotification);
		this.RemoveObserver(OnFinishedPlacingPawns, Game.finishedPlacingPawnsNotification);
	}

	private void Awake() {
		CheckState();
	}

	/* Invoked when the user has chosen which player he wants to play as */
	private void OnPlayerChosen(object sender, object args) {
		humanPlayer = (PawnType)args;
		cpuPlayer = humanPlayer == PawnType.Fox ? PawnType.Goose : PawnType.Fox;
		ClearBoard();
		mainCamera.GetComponent<MoveCamera>().PositionCamera(humanPlayer);
		game = new Game(15, false);
        //weights must be in absolute value
		alphaBeta = new AlphaBeta(cpuPlayer, 5, new WeightsForBoardEval(4, 3, 0, -1, 0, 0, 0), false);
		CheckState();
		if (game.turn == cpuPlayer) {
			AiMoves();
		}
	}

	/*Invoked when all pawns have been placed on the board */
	private void OnFinishedPlacingPawns(object sender, object args) {
		StartCoroutine(RotatePawns());
		CheckState();
	}

	private void OnChangeTurn(object sender, object args) {
		if (game.turn == cpuPlayer) {
			AiMoves();
		}
		CheckState();

	}

	private void OnEndGame(object sender, object args) {
		CheckState();
	}

	private void OnConfigure(object sender, object args) {
		CheckState();
	}

	private void OnEndTurn(object sender, object args) {
		game.ChangeTurn();
	}

	private void CheckState() {
		if (game == null) {
			ChangeState<ChoosePlayerGameState>();
		}
		else if (game.turn == PawnType.None) {
			ChangeState<EndGameState>();
		}
		else if (game.turn == humanPlayer) {
			ChangeState<ActiveGameState>();
		}
		else {
			ChangeState<PassiveGameState>();
		}
	}

	/* The ai makes a move */
	private void AiMoves() {
		StartCoroutine(AiMovesCoroutine());
	}

	private IEnumerator AiMovesCoroutine() {
		yield return new WaitForSeconds(0.1f);
		Move move = alphaBeta.RunAlphaBeta(game);
		game.MovePawn(move); // makes move in the model
		PawnData pawn = gameBoard.GetPawnAtCoord(move.startingX, move.startingZ);
		pawn.x = move.finalX;
		pawn.z = move.finalZ;
		pawn.GetComponent<MovePawnAnimation>().moveToDestination = true; // moves pawn in the view (graphically)
	}

	public void SetGameToNull() {
		game = null;
		CheckState();
	}

	/** Manages the drag of the pawns and their physical movement */
	public void MovePawn(object sender, object args) {
		Draggable draggablePawn = (Draggable)sender;
		PawnData pawn = draggablePawn.gameObject.GetComponent<PawnData>();
		if (!IsPawnTypeValid(pawn, humanPlayer)) {
			return;
		}
		GameObject obj = pawn.gameObject;
		Vector3 pos = (Vector3)args;
		pawn.gameObject.transform.localPosition = pos; // moves the pawn locally (client side)
	}

	/** Manages when the user releases the mouse and tries to make a move */
	public void ConfirmMovePawn(object sender, object args) {
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
				pawn.x = finalTile.x;
				pawn.z = finalTile.z;
				game.MovePawn(move);
			}
		}
		realFinalPos.y = oldPos.y;
		obj.transform.localPosition = realFinalPos; // moves the pawn locally (graphically)
	}

	private bool IsPawnTypeValid(PawnData pawnData, PawnType pawnType) {
		return pawnData.pawnType == pawnType;
	}

	/** Clear the (graphical) gameBoard */
	private void ClearBoard() {
		pawnSpawnerController.ClearBoard();
	}

	/** Writes a hint on the screen */
	private IEnumerator WriteHint(String text, int duration) {
		hintLabel.text = text;
		yield return new WaitForSeconds(duration);
		hintLabel.text = "";
	}

	/** Rotates pawns */
	private IEnumerator RotatePawns() {
		yield return new WaitForSeconds(0.3f);
		RotatePawn[] pawns = FindObjectsOfType<RotatePawn>();
		foreach (RotatePawn pawn in pawns) {
			pawn.Rotate(humanPlayer);
		}
	}
}
