using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineGameState : BaseGameState {

	public GameObject serverGuiContainer { get { return owner.serverGuiContainer; } }

	public override void Enter() {
		base.Enter();
		gameStateLabel.text = "";
		localPlayerLabel.text = "";
		remotePlayerLabel.text = "";
		if (serverGuiContainer != null) {
			serverGuiContainer.SetActive(false);
		}
	}
}
