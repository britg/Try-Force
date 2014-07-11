using UnityEngine;
using System.Collections;

public interface IProjectile {

	int damage { get; }
	float force { get; }
	float lifetime { get; }
}
