using UnityEngine;
using System.Collections;

public class LoadGameState : BaseGameState {

	public GameObject serverGuiContainer { get { return owner.serverGuiContainer; } }

	public override void Enter() {
		base.Enter();
		if (localPlayer != null && localPlayer.isServer) {
			if (serverGuiContainer != null) {
				serverGuiContainer.SetActive(false);
			}
		}
		gameStateLabel.text = "Waiting for players to connect";
		localPlayerLabel.text = "";
		remotePlayerLabel.text = "";
	}

	public override void Exit() {
		base.Exit();
		if (localPlayer != null) {
			localPlayer.score = 0;
		}
		if (remotePlayer != null) {
			remotePlayer.score = 0;
		}
		RefreshPlayerLabels();
	}

}