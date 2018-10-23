using UnityEngine;
using System.Collections;

public class ChoosePlayerGameState : BaseGameState {

	public GameObject serverGuiContainer { get { return owner.choosePlayerGUIContainer; } }

	public override void Enter() {
		base.Enter();
		if (owner.choosePlayerGUIContainer != null) {
			owner.choosePlayerGUIContainer.SetActive(true);
		}
		gameStateLabel.text = "Choose player";
		RefreshPlayerLabels();
	}
}