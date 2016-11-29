using UnityEngine;
using System.Collections;

public class QuestionMark : MonoBehaviour {

	Animator theAnimator;

	public enum Prize {Coin, MushroomOrFlower}

	public Prize prize = Prize.Coin; 

	private bool hitLeft = true;

	// Use this for initialization
	void Start () {
		theAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" && hitLeft) {
			if (prize == Prize.Coin) {
				theAnimator.SetTrigger ("Hit");
			} else {
				theAnimator.SetTrigger ("HitMushroom");

			}
			hitLeft = false;
		}

	}
}
