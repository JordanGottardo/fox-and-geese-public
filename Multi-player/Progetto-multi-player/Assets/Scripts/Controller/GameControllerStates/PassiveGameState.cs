using UnityEngine;
using System.Collections;

public class PassiveGameState : BaseGameState {
	public override void Enter() {
		base.Enter();
		if (owner.serverGuiContainer != null) {
			owner.serverGuiContainer.SetActive(false);
		}
		gameStateLabel.text = "Opponent's Turn!";
		RefreshPlayerLabels();
	}
}