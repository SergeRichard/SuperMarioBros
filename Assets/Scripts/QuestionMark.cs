using UnityEngine;
using System.Collections;

public class QuestionMark : MonoBehaviour {

	Animator theAnimator;

	public enum Prize {Coin, MushroomOrFlower}

	public Prize prize = Prize.Coin; 

	private bool hitLeft = true;

	public Mushroom mushroom;

	public GameObject mushroomPrefab;

	public AudioSource PowerupAppearsAudioSource;

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
				if (PlayerController.PlayerState == PlayerController.PlayerStates.Small)
					theAnimator.SetTrigger ("HitMushroom");
				else
					theAnimator.SetTrigger ("HitFlower");
				
				PowerupAppearsAudioSource.Play ();
			}
			hitLeft = false;
		}
	}
	void OnMushroomAnimationEnd() {
		Transform mushTransform = GetComponent<Transform> ();

		GameObject mushroomInstance = (GameObject)Instantiate (mushroomPrefab, (mushTransform.position - new Vector3(0,-0.8f,0)), Quaternion.identity);
		mushroomInstance.GetComponent<Transform> ().parent = null;

	}
	void OnFlowerAnimationEnd() {

	}
}
