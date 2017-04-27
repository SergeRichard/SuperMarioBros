using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour {

	Animator theAnimator;
	public AudioSource BumpAudioSource; 
	public int coinAmount;

	// Use this for initialization
	void Start () {
		theAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			theAnimator.Play ("Idle");
			theAnimator.SetBool ("HitAnimation",true);
				
			BumpAudioSource.Play ();
		}

	}
	void OnTriggerExit2D(Collider2D other) {
		if (coinAmount > 0) {
			coinAmount--;
			theAnimator.SetInteger ("Coins", coinAmount);
			theAnimator.SetBool ("HasOrHadCoins", true);
		}


	}
	public void HitNoCoinDone() {
		theAnimator.SetBool ("HitAnimation",false);

	}
	public void HitLastCoinDone() {
		theAnimator.SetBool ("HitAnimation", false);

	}
	public void HitCoinsDone() {
		theAnimator.SetBool ("HitAnimation", false);
	}
}
