using System;
using System.ComponentModel;

#if ENABLE_UNET

namespace UnityEngine.Networking {
	[AddComponentMenu("Network/NetworkManagerHUD")]
	[RequireComponent(typeof(NetworkManager))]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class MyNetworkManagerHUD : MonoBehaviour {
		public NetworkManager manager;
		[SerializeField] public bool showGUI = true;
		[SerializeField] public int offsetX;
		[SerializeField] public int offsetY;

		// Runtime variable
		bool m_ShowServer;
        //public GameObject panel;
        public Texture myTexture;

		void Awake() {
			manager = GetComponent<NetworkManager>();

		}

		void Update() {
			if (!showGUI)
				return;

			if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null) {
				if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer) {
					if (Input.GetKeyDown(KeyCode.S)) {
						manager.StartServer();
					}
					if (Input.GetKeyDown(KeyCode.H)) {
						manager.StartHost();
					}
				}
				if (Input.GetKeyDown(KeyCode.C)) {
					manager.StartClient();
				}
			}
			if (NetworkServer.active) {
				if (manager.IsClientConnected()) {
					if (Input.GetKeyDown(KeyCode.X)) {
						manager.StopHost();
					}
				}
				else {
					if (Input.GetKeyDown(KeyCode.X)) {
						manager.StopServer();
					}
				}
			}
		}

		void OnGUI() {
			if (!showGUI)
				return;

			int xpos = 30*5+ offsetX*5;
			int ypos = 30*5 + offsetY*5;
			const int spacing = 24*5;
            GUIStyle customButton = new GUIStyle("button");
            GUIStyle customLabel = new GUIStyle("label");
            GUIStyle customBox = new GUIStyle("box");
            GUIStyle customTextField = new GUIStyle("textfield");
            customButton.fontSize = 38;
            customLabel.fontSize = 38;
            customBox.fontSize = 38;
            customTextField.fontSize = 38;
            //GIULIA DA FIXARE
            //customTextField.padding = 24;


			bool noConnection = (manager.client == null || manager.client.connection == null ||
								 manager.client.connection.connectionId == -1);

			if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null) {
				if (noConnection) {
					if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer) {
						if (GUI.Button(new Rect(xpos, ypos, 200*3, 20*6), "LAN Host (H)" ,customButton)) {
							manager.StartHost();
						}
						ypos += spacing;
					}

					if (GUI.Button(new Rect(xpos, ypos, 105*3, 20*6), "LAN Client (C)", customButton)) {
						manager.StartClient();
					}

					manager.networkAddress = GUI.TextField(new Rect(xpos + 110*3, ypos, 90*3, 20*6), manager.networkAddress, customTextField);
					ypos += spacing;

					if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer) {
						// cant be a server in webgl build
						GUI.Box(new Rect(xpos, ypos, 200*3, 25*6), "(  WebGL cannot be server  )", customBox);
						ypos += spacing;
					}
					else {
						if (GUI.Button(new Rect(xpos, ypos, 200*3, 20*6), "LAN Server Only (S)", customButton)) {
							manager.StartServer();
						}
						ypos += spacing;
					}
				}
				else {
					GUI.Label(new Rect(xpos, ypos, 200*3, 20*6), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..", customLabel);
					ypos += spacing;


					if (GUI.Button(new Rect(xpos, ypos, 200*3, 20*6), "Cancel Connection Attempt", customButton)) {
						manager.StopClient();
					}
				}
			}
			else {
				if (NetworkServer.active) {
                    /*
					string serverMsg = "Server: port=" + manager.networkPort;
					if (manager.useWebSockets) {
						serverMsg += " (Using WebSockets)";
					}
					GUI.Label(new Rect(xpos, ypos, 300*3, 20*6), serverMsg, customLabel);
					ypos += spacing;
                    */
				}
				if (manager.IsClientConnected()) {
                    /*
					GUI.Label(new Rect(xpos, ypos, 300*3, 20*6), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort, customLabel);
					ypos += spacing;
                    */
				}
			}

			if (manager.IsClientConnected() && !ClientScene.ready) {
				if (GUI.Button(new Rect(xpos, ypos, 200*3, 20*6), "Client Ready", customButton)) {
					ClientScene.Ready(manager.client.connection);

					if (ClientScene.localPlayers.Count == 0) {
						ClientScene.AddPlayer(0);
					}
				}
				ypos += spacing;
			}

			if (NetworkServer.active || manager.IsClientConnected()) {
                //GUI.DrawTexture(new Rect(Screen.width/4, Screen.height/4, Screen.width/4*2, Screen.height/4*2), myTexture);
				if (GUI.Button(new Rect(30, 30, 200*3, 20*6), "Stop (X)", customButton)) {
					manager.StopHost();
				}
				ypos += spacing;
			}

			if (!NetworkServer.active && !manager.IsClientConnected() && noConnection) {
				ypos += 10;

				if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer) {
					GUI.Box(new Rect(xpos - 5, ypos, 220*3, 25*6), "(WebGL cannot use Match Maker)", customBox);
					return;
				}

				if (manager.matchMaker == null) {
					if (GUI.Button(new Rect(xpos, ypos, 200*3, 20*6), "Enable Match Maker (M)", customButton)) {
						manager.StartMatchMaker();
					}
					ypos += spacing;
                    manager.StartMatchMaker();
				}
				else {
					if (manager.matchInfo == null) {
                        if (manager.matches == null) {
                        GUI.DrawTexture(new Rect(Screen.width/16*3, Screen.height/16*2, Screen.width/16*10, Screen.height/16*12), myTexture);
							if (GUI.Button(new Rect(Screen.width/32*10, Screen.height/32*13, Screen.width/32*12, Screen.height/32*4), "Find matches", customButton)) {
								manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
							}
							ypos += spacing + 70;

							GUI.Label(new Rect(Screen.width/32*10, Screen.height/32*20+7, Screen.width/32*5, Screen.height/32*2), "Match name:", customLabel);
							manager.matchName = GUI.TextField(new Rect(Screen.width/32*15, Screen.height/32*20, Screen.width/32*7, Screen.height/32*2), manager.matchName, customTextField);
							ypos += 70;

							if (GUI.Button(new Rect(Screen.width/32*10, Screen.height/32*23, Screen.width/32*12, Screen.height/32*3), "Create new match", customButton)) {
								manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
							}
							ypos += spacing;

							ypos += 10;
						}
						else {
                            if (GUI.Button(new Rect(30, 30, 200 * 3, 20 * 6), "Back to menu", customButton)) {
                                manager.matches = null;
                            }
                            ypos += spacing;
                            ypos += 50;

                            if (manager.matches != null) {
                                for (int i = 0; i < manager.matches.Count; i++) {
                                    var match = manager.matches[i];
                                    if (GUI.Button(new Rect(30, ypos, 200 * 3, 20 * 5), "Join match: " + match.name, customButton)) {
                                        manager.matchName = match.name;
                                        manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
                                    }
                                    ypos += spacing;
                                }
                            }
						}
					}
                    /*
					if (GUI.Button(new Rect(xpos, ypos, 200*3, 20*6), "Change MM server", customButton)) {
						m_ShowServer = !m_ShowServer;
					}
					if (m_ShowServer) {
						ypos += spacing;
						if (GUI.Button(new Rect(xpos, ypos, 100*3, 20*6), "Local", customButton)) {
							manager.SetMatchHost("localhost", 1337, false);
							m_ShowServer = false;
						}
						ypos += spacing;
						if (GUI.Button(new Rect(xpos, ypos, 100*3, 20*6), "Internet", customButton)) {
							manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
							m_ShowServer = false;
						}
						ypos += spacing;
						if (GUI.Button(new Rect(xpos, ypos, 100*3, 20*6), "Staging", customButton)) {
							manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
							m_ShowServer = false;
						}
					}
                    
					ypos += spacing;
                    
					GUI.Label(new Rect(xpos, ypos, 300*3, 20*6), "MM Uri: " + manager.matchMaker.baseUri, customLabel);
					ypos += spacing;
                    */
                    /*
					if (GUI.Button(new Rect(xpos, ypos, 200*3, 20*6), "Disable Match Maker", customButton)) {
						manager.StopMatchMaker();
					}
					ypos += spacing;
                    */
				}
			}
		}
	}
}
#endif //ENABLE_UNET