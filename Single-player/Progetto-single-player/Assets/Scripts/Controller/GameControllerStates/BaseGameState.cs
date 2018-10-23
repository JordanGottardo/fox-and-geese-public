using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FoxAndGeese;

public abstract class BaseGameState : State {
	public GameController owner;
	public GameBoard board { get { return owner.gameBoard; } }
	public Text humanPlayerLabel { get { return owner.humanPlayerLabel; } }
	public Text cpuPlayerLabel { get { return owner.cpuPlayerLabel; } }
	public Text gameStateLabel { get { return owner.gameStateLabel; } }
	public Game game { get { return owner.game; } }
	public PawnType humanPlayer { get { return owner.humanPlayer; } }
	public PawnType cpuPlayer { get { return owner.cpuPlayer; } }

	protected virtual void Awake() {
		owner = GetComponent<GameController>();
	}

	protected void RefreshPlayerLabels() {
		if (humanPlayer != PawnType.None) {
			humanPlayerLabel.text = string.Format("You: {0}\nWins: {1}", humanPlayer, owner.humanPlayerScore);
		}
		if (cpuPlayer != PawnType.None) {
			cpuPlayerLabel.text = string.Format("Opponent: {0}\nWins: {1}", cpuPlayer, owner.cpuPlayerScore);
		}
	}
}