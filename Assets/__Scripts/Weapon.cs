using UnityEngine;
using System.Collections;

// Enum of the various possible weapon types
// Also includes a "shield" type to allow a shield power-up
public enum WeaponType {
	none, 			// the default / no weapons
	blaster, 		// a simple blaster
	spread, 		// two shots simultaneously
	phaser, 		// shots that move in waves (NI)
	missile, 		// homing missiles (NI)
	laser, 			// damage over time (NI)
	shield 			// Raises shieldLevel
}

[System.Serializable]
public class WeaponDefinition {
	public WeaponType 	type = WeaponType.none;
	public string 		letter; // letter to show on powerup
	public Color 		color = Color.white; // color of collar and powerup
	public GameObject 	projectilePrefab;
	public Color 		projectileColor = Color.white;
	public float 		damageOnHit = 0;
	public float 		continuousDamage = 0; // Damage per second (laser)
	public float 		delayBetweenShots = 0;
	public float 		velocity = 20; // projectile speed
}
// Note: weapon prefabs, colors, etc are set in the class main

public class Weapon : MonoBehaviour {
	static public Transform 	PROJECTILE_ANCHOR;

	public bool _________________;
	[SerializeField]
	private WeaponType 			_type = WeaponType.blaster;
	public WeaponDefinition 	def;
	public GameObject 			collar;
	public float 				lastShot; // time of last shot fired

	void Awake() {
		collar = transform.Find("Collar").gameObject;
	}

	void Start() {
		SetType(_type);

		if (PROJECTILE_ANCHOR == null) {
			GameObject go = new GameObject ("_Projectile_Anchor");
			PROJECTILE_ANCHOR = go.transform;
		}
		// Find the fireDelegate of the parent
		GameObject parentGO = transform.parent.gameObject;
		if (parentGO.tag == "Hero") {
			Hero.S.fireDelegate += Fire;
		}
	}

	public WeaponType type {
		get { return _type; }
		set { SetType(value); }
	}

	public void SetType(WeaponType wt) {
		_type = wt;
		if (type == WeaponType.none) {
			this.gameObject.SetActive (false);
			return;
		} else {
			this.gameObject.SetActive(true);
		}
		def = Main.GetWeaponDefinition(_type);
		collar.GetComponent<Renderer>().material.color = def.color;
		lastShot = 0;
	}

	public void Fire() {
		// if this.gameObject is inactive, return
		if (!gameObject.activeInHierarchy) return;

		// if it hasn't been enough time between shots, return
		if (Time.time - lastShot < def.delayBetweenShots) {
			return;
		}
		Projectile p;
		switch (type) {
		case WeaponType.blaster:
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			break;

		case WeaponType.spread:
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = new Vector3(-.2f, 0.9f, 0) * def.velocity;
			p = MakeProjectile();
			p.GetComponent<Rigidbody>().velocity = new Vector3(.2f, 0.9f, 0) * def.velocity;
			break;
		}
	}

	public Projectile MakeProjectile() {
		GameObject go = Instantiate(def.projectilePrefab) as GameObject;
		if (transform.parent.gameObject.tag == "Hero") {
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer("ProjectileHero");
		} else {
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer("ProjectileEnemy");
		}
		go.transform.position = collar.transform.position;
		go.transform.parent = PROJECTILE_ANCHOR;
		Projectile p = go.GetComponent<Projectile>();
		p.type = type;
		lastShot = Time.time;
		return p;
	}
}
