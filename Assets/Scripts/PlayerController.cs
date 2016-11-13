using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	private float activeMoveSpeed;

	public bool canMove;

	[HideInInspector]
	public Rigidbody2D myRigidbody;

	public float jumpSpeed;

	public Transform groundCheck;
	public float groundCheckRadius;
	public LayerMask whatIsGround;

	public bool isGrounded;

	private Animator myAnim;
	private SpriteRenderer mySpriteRenderer;

	public Vector3 respawnPosition;

	private LevelManager theLevelManager;

	public GameObject stompBox;

	public float knockBackForce;
	public float knockBackLength;
	private float knockBackCounter;

	public float invincibilityLength;
	private float invincibilityCounter;

	public AudioSource JumpAudioSource;
	public AudioSource HurtAudioSource;

	private bool onPlatform;
	public float onPlatformSpeedModifier;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();
		myAnim = GetComponent<Animator> ();
		mySpriteRenderer = GetComponent<SpriteRenderer> ();

		respawnPosition = transform.position;
		theLevelManager = FindObjectOfType<LevelManager> ();

		activeMoveSpeed = moveSpeed;

		canMove = true;
	}
	
	// Update is called once per frame
	void Update () {
		isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, whatIsGround);

		if (knockBackCounter <= 0 && canMove) {
			if (onPlatform) {
				activeMoveSpeed = moveSpeed * onPlatformSpeedModifier;
			} else {
				activeMoveSpeed = moveSpeed;
			}
			if (Input.GetAxisRaw ("Horizontal") > 0f) {
				myRigidbody.velocity = new Vector3 (activeMoveSpeed, myRigidbody.velocity.y, 0f);
				mySpriteRenderer.flipX = false;
			} else if (Input.GetAxisRaw ("Horizontal") < 0f) {
				myRigidbody.velocity = new Vector3 (-activeMoveSpeed, myRigidbody.velocity.y, 0f);
				mySpriteRenderer.flipX = true;
			} else {
				myRigidbody.velocity = new Vector3 (0f, myRigidbody.velocity.y, 0f);
			}
			if (Input.GetButtonDown ("Jump") && isGrounded) {
				myRigidbody.velocity = new Vector3 (myRigidbody.velocity.x, jumpSpeed, 0f);
				JumpAudioSource.Play ();
			}

			theLevelManager.invincible = false;
		}

		if (knockBackCounter > 0) {
			knockBackCounter -= Time.deltaTime;

			if (mySpriteRenderer.flipX) 
				myRigidbody.velocity = new Vector3 (knockBackForce, knockBackForce/2, 0f);
			else
				myRigidbody.velocity = new Vector3 (-knockBackForce, knockBackForce/2, 0f);
		}

		if (invincibilityCounter > 0) {
			invincibilityCounter -= Time.deltaTime;

		} else {
			theLevelManager.invincible = false;
		}

		myAnim.SetFloat ("Speed", Mathf.Abs(myRigidbody.velocity.x));
		myAnim.SetBool ("Grounded", isGrounded);

		if (myRigidbody.velocity.y < 0) {
			stompBox.SetActive (true);

		} else {
			stompBox.SetActive (false);
		}
	}
	public void KnockBack() {
		knockBackCounter = knockBackLength;
		invincibilityCounter = invincibilityLength;
		theLevelManager.invincible = true;
	}
	public void OnEnable() {
		knockBackCounter = 0;

	}
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "KillPlane") {
			theLevelManager.healthCount = 0;
			theLevelManager.UpdateHealthMeter ();
			//theLevelManager.Respawn ();

		}
		if (other.tag == "Checkpoint") {
			respawnPosition = other.transform.position;
		}

	}
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "MovingPlatform") {
			transform.parent = other.transform;
			onPlatform = true;
		}
	}
	void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.tag == "MovingPlatform") {
			transform.parent = null;
			onPlatform = false;
		}
	}
}
