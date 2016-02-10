using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
	public AudioClip walk;
	AudioSource audioSource;

	void Start(){
		walk = Resources.Load ("Audio/walk") as AudioClip;
	}

	public void playFootsteps() {
		audioSource = GetComponent<AudioSource>();
		audioSource.PlayOneShot(walk, 0.05F);
	}
}
