using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxAndGeese;

public class MatchController : MonoBehaviour {

	public const string matchReadyNotification = "MatchController.MatchReadyNotification";
	public const string matchNoLongerReadyNotification = "MatchController.MatchNoLongerReadyNotification";
	public const string configureNotification = "MatchController.ConfigureNotification";

	public bool IsReady { get { return localPlayer != null && remotePlayer != null; } }
	public bool IsOffline { get { return players.Count == 0; } }
	public PlayerController localPlayer;
	public PlayerController remotePlayer;
	public PlayerController hostPlayer;
	public PlayerController clientPlayer;
	public List<PlayerController> players = new List<PlayerController>();
	public GameBoard gameBoard;

	/** Invoked before Start() */
	private void OnEnable() {
		this.AddObserver(OnPlayerStarted, PlayerController.startedNotification);
		this.AddObserver(OnPlayerStartedLocal, PlayerController.startedLocalNotification);
		this.AddObserver(OnPlayerDestroyed, PlayerController.destroyedNotification);
	}

	private void OnDisable() {
		this.RemoveObserver(OnPlayerStarted, PlayerController.startedNotification);
		this.RemoveObserver(OnPlayerStartedLocal, PlayerController.startedLocalNotification);
		this.RemoveObserver(OnPlayerDestroyed, PlayerController.destroyedNotification);
	}

	/** Invoked whenever a player prefab is created */
	private void OnPlayerStarted(object sender, object args) {
		PlayerController player = (PlayerController)sender;
		players.Add(player);
		player.gameBoard = this.gameBoard;
		Configure();
	}

	/** Invoked when the local player prefab is created */
	private void OnPlayerStartedLocal(object sender, object args) {
		localPlayer = (PlayerController)sender;
		localPlayer.gameBoard = this.gameBoard;
		Configure();
	}

	/** Invoked whenever a player prefab is destroyed */
	private void OnPlayerDestroyed(object sender, object args) {
		PlayerController pc = (PlayerController)sender;
		if (localPlayer == pc) {
			localPlayer = null;
		}
		if (remotePlayer == pc) {
			remotePlayer = null;
		}
		if (hostPlayer == pc) {
			hostPlayer = null;
		}
		if (clientPlayer == pc) {
			clientPlayer = null;
		}
		if (players.Contains(pc)) {
			players.Remove(pc);
		}
		this.PostNotification(matchNoLongerReadyNotification);
	}

	private void Configure() {
		this.PostNotification(configureNotification);
		if (localPlayer == null || players.Count < 2) {
			return;
		}
		for (int i = 0; i < players.Count; ++i) {
			if (players[i] != localPlayer) {
				remotePlayer = players[i];
				break;
			}
		}
		hostPlayer = (localPlayer.isServer) ? localPlayer : remotePlayer;
		clientPlayer = (localPlayer.isServer) ? remotePlayer : localPlayer;
		this.PostNotification(matchReadyNotification);
	}

}
