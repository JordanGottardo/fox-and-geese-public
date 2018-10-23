using UnityEngine;
using System.Collections;

public class ChooseGeeseGameState : BaseGameState {

	public GameObject serverGuiContainer { get { return owner.serverGuiContainer; } }

	public override void Enter() {
		base.Enter();
		if (localPlayer.isServer) {
			if (owner.serverGuiContainer != null) {
				owner.serverGuiContainer.SetActive(true);
			}
				gameStateLabel.text = "Choose geese number";
		} else {
			gameStateLabel.text = "Host is choosing geese number";
		}
		
		RefreshPlayerLabels();
	}
} 