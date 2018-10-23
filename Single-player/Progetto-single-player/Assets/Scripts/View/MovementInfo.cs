using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInfo {

	public Vector3 pos;
	public bool isLocalAndClient;

	public MovementInfo() {
	}

	public MovementInfo(Vector3 pos) {
		this.pos = pos;
	}

	public MovementInfo(Vector3 pos, bool isLocalAndClient) {
		this.pos = pos;
		this.isLocalAndClient = isLocalAndClient;
	}
}
