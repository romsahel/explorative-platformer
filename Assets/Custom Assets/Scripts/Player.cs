using UnityEngine;
using System.Collections;
using PC2D;

[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public enum State
    {
        ALIVE,
        DRINKING,
        PICKING,
        HIT,
        DEAD,
    }
    public int health;

    public BoxCollider2D boxCollider { get; private set; }
    public State CurrentState { get; private set; }

	private AudioClip snoring;
	private AudioClip hurt;
	//private AudioClip drinking;

	AudioSource audioSource;

    // Use this for initialization
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
		snoring = Resources.Load ("Audio/snoring") as AudioClip;
		hurt = Resources.Load ("Audio/hurt") as AudioClip;
		//drinking = Resources.Load ("Audio/drinking") as AudioClip;
		audioSource = GetComponent<AudioSource>();

	}

    public void damage(int damage)
    {

		if (health <= 0)
            return;
        Debug.Log("Player was hit by " + damage);
        health -= damage;
		audioSource.PlayOneShot (hurt, 0.1F);
        CurrentState = State.HIT;
        //if (GUIDisplayer._hurtMask != null)
        //    GUIDisplayer._hurtMask.Play("Show");

        if (health <= 0 && CurrentState != State.DEAD)
            kill();
    }

    private void kill()
    {
        CurrentState = State.DEAD;
        GetComponent<PlayerController2D>().enable(false);
        var startScreen = FindObjectOfType<StartScreen>();
        startScreen.setShown(true);
        startScreen.GetComponent<Animator>().Play("gameover");
		audioSource.PlayOneShot (snoring, 0.5F);

    }

    public void setCurrentState(State state)
    {
        CurrentState = state;
        GetComponent<PlayerController2D>().enable(CurrentState == State.ALIVE);
        //GetComponent<PlatformerMotor2D>().enabled = (CurrentState == State.ALIVE);
    }
}
