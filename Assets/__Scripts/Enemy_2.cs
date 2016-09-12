using UnityEngine;
using System.Collections;

public class Enemy_2 : Enemy {
	// uses a sin wave to modify a 2-point linear interpolation
	public Vector3[] 		points;
	public float 			birthTime;
	public float			lifeTime = 10;
	public float			sinEccentricity = 0.6f;

	void Start () {
		points = new Vector3[2];

		// Find Utils.camBounds
		Vector3 cbMin = Utils.camBounds.min;
		Vector3 cbMax = Utils.camBounds.max;

		Vector3 v = Vector3.zero;
		// pick any point on the left side fo the screen
		v.x = cbMin.x - Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [0] = v;

		// pick any point on the right side
		v = Vector3.zero;
		v.x = cbMax.x + Main.S.enemySpawnPadding;
		v.y = Random.Range (cbMin.y, cbMax.y);
		points [1] = v;

		// possibly swap sides
		if (Random.value < 0.5f) {
			// Setting the .x of each point to it's negative will move it
			// to the other side of the screen
			points[0].x *= -1;
			points[1].x *= -1;
		}

		// Set the birthTime to the curr time
		birthTime = Time.time;
	}
	
	public override void Move() {
		float u = (Time.time - birthTime) / lifeTime;

		// if u>1 => it's been longer than lifetime
		if (u > 1) {
			Destroy (this.gameObject);
			return;
		}

		// Adjust u by adding an easing curve based on a sin wave
		u = u + sinEccentricity*(Mathf.Sin(u*Mathf.PI*2));

		// Interpolate the two linear points
		pos = (1 - u) * points[0] + u*points[1];
	}
}
