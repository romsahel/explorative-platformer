using UnityEngine;
using System.Collections;

public class music : MonoBehaviour {
	private AudioClip monster;
	private AudioClip wolf;
	//private AudioClip ambient_night;

	AudioSource audioSource;

	public float waittimeMonster;
	private float lastPlayedMonster;
	public float waittimeWolf;
	private float lastPlayedWolf;

	// Use this for initialization
	void Start () {
		monster = Resources.Load("Audio/Background/monster") as AudioClip;
		wolf = Resources.Load("Audio/Background/wolf") as AudioClip;
		//ambient_night = Resources.Load ("Audio/Background/ambient_night") as AudioClip;

		audioSource = GetComponent<AudioSource>();

	}


	// Update is called once per frame
	void Update () {
		if (Time.time - lastPlayedMonster > waittimeMonster) {
			audioSource.PlayOneShot (monster, 0.05F);
			lastPlayedMonster = Time.time;
		}
		if (Time.time - lastPlayedWolf > waittimeWolf) {
			audioSource.PlayOneShot (wolf, 0.05F);
			lastPlayedWolf = Time.time;
		}


	}
}
