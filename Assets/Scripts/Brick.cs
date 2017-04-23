using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour {

	Animator theAnimator;
	public AudioSource BumpAudioSource; 

	// Use this for initialization
	void Start () {
		theAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" /*&& theAnimator.GetBool("HitAnimation") == false*/) {
			theAnimator.Play ("Idle");
			theAnimator.SetBool ("HitAnimation",true);
			BumpAudioSource.Play ();
		}

	}
	public void HitNoCoinDone() {
		theAnimator.SetBool ("HitAnimation",false);

	}
}
