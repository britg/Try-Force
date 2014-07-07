using UnityEngine;
using System.Collections;

public interface IProjectile {

	int damage { get; }
	float speed { get; }
	float lifetime { get; }
}
