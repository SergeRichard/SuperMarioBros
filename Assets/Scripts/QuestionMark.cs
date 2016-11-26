using UnityEngine;
using System.Collections;

public class QuestionMark : MonoBehaviour {

	Animator theAnimator;

	// Use this for initialization
	void Start () {
		theAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			theAnimator.SetTrigger ("Hit");
		}

	}
}
