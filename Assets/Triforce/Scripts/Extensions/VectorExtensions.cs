using UnityEngine;
using System.Collections;

public static class VectorExtensions {

	public static Vector2 XY (this Vector3 v) {
		return new Vector2 (v.x, v.y);
	}

		
	public static Vector2 Rotate(this Vector2 v, float degrees) {
		Vector3 v3 = Quaternion.AngleAxis(degrees, Vector3.back) * v;
		return v3.XY();
	}

}
