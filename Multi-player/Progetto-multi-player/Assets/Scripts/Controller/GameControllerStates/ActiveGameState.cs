using UnityEngine;
using System.Collections;

public class ActiveGameState : BaseGameState {

	public override void Enter() {
		base.Enter();
		if (owner.serverGuiContainer != null) {
			owner.serverGuiContainer.SetActive(false);
		}
		gameStateLabel.text = "Your Turn!";
		RefreshPlayerLabels();
	}

	protected override void AddListeners() {
		base.AddListeners();
		this.AddObserver(OnMovePawn, Draggable.movePawnNotification);
		this.AddObserver(OnConfirmMovePawn, Draggable.confirmMovePawnNotification);
	}

	protected override void RemoveListeners() {
		base.RemoveListeners();
		this.RemoveObserver(OnMovePawn, Draggable.movePawnNotification);
		this.RemoveObserver(OnConfirmMovePawn, Draggable.confirmMovePawnNotification);
	}

	void OnMovePawn(object sender, object args) {
		owner.MovePawn(sender, args);
	}

	void OnConfirmMovePawn(object sender, object args) {
		owner.ConfirmMovePawn(sender, args);
	}
}