using UnityEngine;
using System.Collections;
using FoxAndGeese;

public class EndGameState : BaseGameState {
	public override void Enter() {
		base.Enter();
			string winner = game.winner == PawnType.Fox ? "Fox wins!" : "Geese win!";
			gameStateLabel.text = "The " + winner;
			localPlayer.score++;
		RefreshPlayerLabels();
		StartCoroutine(Restart());
	}

	IEnumerator Restart() {
		yield return new WaitForSeconds(5);
		owner.SetGameToNull();
	}
}