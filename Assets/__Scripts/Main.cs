﻿using UnityEngine; // Required for Unity
using System.Collections; // Required for Arrays & other Collections
using System.Collections.Generic; // Required to use Lists or Dictionaries
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f; // # Enemies/second
	public float enemySpawnPadding = 1.5f; // Padding for position
	public WeaponDefinition[] weaponDefinitions;

	public GameObject prefabPowerUp;

	public WeaponType[] powerUpFrequency = new WeaponType[] {
		WeaponType.blaster, WeaponType.blaster,
		WeaponType.spread,
		WeaponType.shield } ;

	public bool ________________;

	public WeaponType[] activeWeaponTypes;
	private float enemySpawnRate; // Delay between Enemies


	void Awake() {
		S = this;
		// Set Utils.camBounds
		Utils.SetCameraBounds(this.GetComponent<Camera>());
		// 0.5 enemies/second = enemySpawnRate of 2
		enemySpawnRate = 1f/enemySpawnPerSecond;
		// Invoke 1 call to SpawnEnemy() in 2 seconds
		Invoke( "SpawnEnemy", enemySpawnRate );

		// A generic Dictionary with WeaponType as the key
		W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
		foreach( WeaponDefinition def in weaponDefinitions ) {
			W_DEFS[def.type] = def;
		}
	}

	static public WeaponDefinition GetWeaponDefinition( WeaponType wt ) {
		// Check to make sure that the key exists in the Dictionary
		if (W_DEFS.ContainsKey(wt)) {
			// Attempting to retrieve a key that doesn't exist, throws an error
			return( W_DEFS[wt]);
		}
		// This will return a definition for WeaponType.none,
		// which means it has failed to find the WeaponDefinition
		return( new WeaponDefinition() );
	}

	void Start() {
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for ( int i=0; i<weaponDefinitions.Length; i++ ) {
			activeWeaponTypes[i] = weaponDefinitions[i].type;
		}
	}

	public void SpawnEnemy() {
		// Pick a random Enemy prefab to instantiate
		int ndx = Random.Range(0, prefabEnemies.Length);
		GameObject go = Instantiate( prefabEnemies[ ndx ] ) as GameObject;
		// Position the Enemy above the screen with a random x position
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x+enemySpawnPadding;
		float xMax = Utils.camBounds.max.x-enemySpawnPadding;
		pos.x = Random.Range( xMin, xMax );
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		// Call SpawnEnemy() again in a couple seconds
		Invoke( "SpawnEnemy", enemySpawnRate );
	}

	public void DelayedRestart( float delay ) {
		// Invoke the Restart() method in delay seconds
		Invoke("Restart", delay);
	}

	public void Restart() {
		// Reload _Scene_0 to restart the game
		SceneManager.LoadScene("__Scene_0");
	}

	public void ShipDestroyed( Enemy e ) {
		// Potentially generate a PowerUp
		if (Random.value <= e.powerUpDropChance) {
			// Random.value generates a value between 0 & 1 (though never == 1)
			// If the e.powerUpDropChance is 0.50f, a PowerUp will be generated
			// 50% of the time. For testing, it's now set to 1f.

			// Choose which PowerUp to pick
			// Pick one from the possibilities in powerUpFrequency
			int ndx = Random.Range(0,powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency[ndx];

			// Spawn a PowerUp
			GameObject go = Instantiate( prefabPowerUp ) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp>();
			// Set it to the proper WeaponType
			pu.SetType( puType );

			// Set it to the position of the destroyed ship
			pu.transform.position = e.transform.position;
		}
	}
}