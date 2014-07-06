﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Player {

	public enum Orientation {
		Warrior,
		Thief,
		Mage
	}

	public int life;
	public float moveSpeed;
	public float rotateSpeed;
	public Player.Orientation orientation = Player.Orientation.Warrior;

	public Warrior warrior;
	public Thief thief;
	public Mage mage;

	public bool warriorFace { get { return orientation == Player.Orientation.Warrior; } }
	public bool thiefFace { get { return orientation == Player.Orientation.Thief; } }
	public bool mageFace { get { return orientation == Player.Orientation.Mage; } }

	public void RotateRight () {
		if (warriorFace) {
			orientation = Player.Orientation.Thief;
			return;
		}

		if (thiefFace) {
			orientation = Player.Orientation.Mage;
			return;
		}

		if (mageFace) {
			orientation = Player.Orientation.Warrior;
			return;
		}
	}

	public void RotateLeft () {
		if (warriorFace) {
			orientation = Player.Orientation.Mage;
			return;
		}

		if (thiefFace) {
			orientation = Player.Orientation.Warrior;
			return;
		}

		if (mageFace) {
			orientation = Player.Orientation.Thief;
			return;
		}
	}

}
