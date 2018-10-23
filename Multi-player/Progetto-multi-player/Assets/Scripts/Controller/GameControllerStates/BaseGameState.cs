using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FoxAndGeese;

public abstract class BaseGameState : State {
	public GameController owner;
	public GameBoard board { get { return owner.gameBoard; } }
	public Text localPlayerLabel { get { return owner.localPlayerLabel; } }
	public Text remotePlayerLabel { get { return owner.remotePlayerLabel; } }
	public Text gameStateLabel { get { return owner.gameStateLabel; } }
	public Game game { get { return owner.game; } }
	public PlayerController localPlayer { get { return owner.matchController.localPlayer; } }
	public PlayerController remotePlayer { get { return owner.matchController.remotePlayer; } }

	protected virtual void Awake() {
		owner = GetComponent<GameController>();
	}

	protected void RefreshPlayerLabels() {
		if (localPlayer != null) {
			localPlayerLabel.text = string.Format("You: {0}\nWins: {1}", localPlayer.pawnType, localPlayer.score);
		}
		if (remotePlayer != null) {
			remotePlayerLabel.text = string.Format("Opponent: {0}\nWins: {1}", remotePlayer.pawnType, remotePlayer.score);
		}
	}
}