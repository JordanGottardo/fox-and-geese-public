using UnityEngine;
using System.Collections;
using FoxAndGeese;

public class EndGameState : BaseGameState {
	public override void Enter() {
		base.Enter();
		string winner = game.winner == PawnType.Fox ? "Fox wins!" : "Geese win!";
		Debug.LogWarning("endGameState ChecKForWin winner is = " + game.winner);

		gameStateLabel.text = "The " + winner;
		owner.humanPlayerScore++;
		RefreshPlayerLabels();
		StartCoroutine(Restart());
	}

	IEnumerator Restart() {
		yield return new WaitForSeconds(5);
		owner.SetGameToNull();
	}
}