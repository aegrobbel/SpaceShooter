using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero S; // singleton

	public float gameRestartDelay = 2f;

	// Controls for movement of the ship
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;

	// Ship status info
	[SerializeField]
	private float _shieldLevel = 1;

	// Weapon fields
	public Weapon[] weapons;

	public bool ___________________;

	public Bounds bounds;

	// Declare a new delegate type WeaponFireDelagate
	public delegate void WeaponFireDelegate();
	// Create a weaponfiredelegate field named fireDelegate
	public WeaponFireDelegate fireDelegate;

	void Awake() {
		S = this;
		bounds = Utils.CombineBoundsOfChildren(this.gameObject);
	}

	void Start() {
		// Reset the weapons to start _Hero with 1 blaster
		ClearWeapons();
		weapons[0].SetType(WeaponType.blaster);
	}

	void Update () {
		// Pull in info from the input class
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		// change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		bounds.center = transform.position;

		// Keep the ship constrained to the screen bounds
		Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
		if (off != Vector3.zero) {
			pos -= off;
			transform.position = pos;
		}

		// Rotate the ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);

		// Use the fireDelegate to fire Weapons
		// 1. make sure the Axis("jump") button is pressed
		// 2. then ensure that firedelegate isn't null to avoid an error
		if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
			fireDelegate();
		}
	}

	// Holds a reference to the last triggering gameObject
	public GameObject lastTriggerGo = null;

	void OnTriggerEnter(Collider other) {
		// Find the tag of other.gameObject or its parent gameObjects
		GameObject go = Utils.FindTaggedParent(other.gameObject);
		// If there is a parent with a tag
		if (go != null) {
			// Make sure it's not the same triggering go as last time
			if (go == lastTriggerGo) {
				return;
			}
			lastTriggerGo = go;

			if (go.tag == "Enemy") {
				
				shieldLevel--;
				// Destroy the enemy
				Destroy (go);
			} else if (go.tag == "PowerUp") {
				// If the shield was triggered by a powerup
				AbsorbPowerUp(go);
			} else {
				// Announce it
				print ("Triggered: " + go.name);
			}
		} else {
			print ("Triggered: " + other.gameObject.name);
		}
	}

	public float shieldLevel {
		get {
			return _shieldLevel;
		}
		set {
			_shieldLevel = Mathf.Min (value, 4);
			// If the shield is going to be set less than zero
			if (value < 0) {
				Destroy(this.gameObject);
				// tell Main.S to restart the game after a delay
				Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}

	public void AbsorbPowerUp(GameObject go) {
		PowerUp pu = go.GetComponent<PowerUp>();
		switch (pu.type) {
		case WeaponType.shield: // if it's the shield
			shieldLevel++;
			break;

		default: // check curr weapon type
			if (pu.type == weapons[0].type) {
				Weapon w = GetEmptyWeaponSlot();
				if (w != null) {
					// set it to pu.type
					w.SetType(pu.type);
				}
			} else {
				// a different weapon
				ClearWeapons();
  				weapons[0].SetType(pu.type);
			}
			break;
		}
		pu.AbsorbedBy(this.gameObject);
	}

	Weapon GetEmptyWeaponSlot() {
		for (int i = 0; i < weapons.Length; i++) {
			if (weapons[i].type == WeaponType.none) {
				return weapons [i];
			}
		}
		return null;
	}

	void ClearWeapons() {
		foreach (Weapon w in weapons) {
			w.SetType(WeaponType.none);
		}
	}
}
