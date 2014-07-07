using UnityEngine;
using System.Collections;

public interface IInputController {

	// Movement
	bool forward { get; } 
	bool right { get; }
	bool back { get; }
	bool left { get; }
	bool rotateRight { get; }
	bool rotateLeft { get; }
	Vector3 resultantInputVector { get; }

	// Aiming
	Vector2 aimFrom { get; }
	Vector2 aimTo { get; }
	Vector2 aimDiff { get; }

	// Attacking
	bool attack { get; }


}
