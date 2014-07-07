using UnityEngine;
using System.Collections;

public static class VectorExtensions {

	public static Vector2 XY (this Vector3 v) {
		return new Vector2 (v.x, v.y);
	}

	public static Vector3 to3 (this Vector2 v) {
		return new Vector3 (v.x, v.y, 0f);
	}
		
	public static Vector2 Rotate(this Vector2 v, float degrees) {
		Vector3 v3 = Quaternion.AngleAxis(degrees, Vector3.back) * v;
		return v3.XY();
	}

	public static void LookAt2D (this Transform t, Vector2 target) {
		Vector2 direction = target.to3() - t.position;
		float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		t.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
	}

}
