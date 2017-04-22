using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	public float additionalSpeedOnRun;

	public SpriteRenderer PlayerGrowShrinkRenderer;

	private float activeMoveSpeed;
	private float currentAdditionalSpeedOnRun;

	public float additionalJumpSpeed;
	private float currentAdditionalJumpSpeed;

	public bool canMove;

	[HideInInspector]
	public Rigidbody2D myRigidbody;

	public float jumpSpeed;

	public Transform groundCheck;
	public float groundCheckRadius;
	public LayerMask whatIsGround;

	public bool isGrounded;

	bool PauseControl = false;

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
	public AudioSource JumpBigAudioSource;
	public AudioSource HurtAudioSource;
	public AudioSource PowerupAudioSource;

	private bool onPlatform;
	public float onPlatformSpeedModifier;

	BoxCollider2D BoxCollider;
	CircleCollider2D CircleCollider;

	Vector2 saveVelocity = new Vector2 ();
	//Vector2 saveAngularVelocity = new Vector2 ();

	public enum PlayerStates
	{
		Small, Big, BigFire
	};

	public static PlayerStates PlayerState;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();
		myAnim = GetComponent<Animator> ();
		mySpriteRenderer = GetComponent<SpriteRenderer> ();

		respawnPosition = transform.position;
		theLevelManager = FindObjectOfType<LevelManager> ();

		BoxCollider = GetComponent<BoxCollider2D> ();
		CircleCollider = GetComponent<CircleCollider2D> ();

		activeMoveSpeed = moveSpeed;

		currentAdditionalSpeedOnRun = 0;
		currentAdditionalJumpSpeed = 0;

		PlayerState = PlayerStates.Big;

		TransformMarioToNewSize ();

		canMove = true;
	}

	public void TransformMarioToNewSize() {
		Vector2 v = new Vector2 ();

		switch (PlayerState) {
		case PlayerStates.Small:
			myAnim.Play ("PlayerIdle");

			v.x = 0;
			v.y = 0.03f;

			BoxCollider.offset = v;

			v.x = 0.5f;
			v.y = 0.59f;

			BoxCollider.size = v;

			v.x = 0;
			v.y = -0.09f;

			CircleCollider.offset = v;

			CircleCollider.radius = 0.23f;
			break;
		case PlayerStates.Big:
			myAnim.Play ("BigPlayerIdle");

			v.x = 0;
			v.y = 0.02f;

			BoxCollider.offset = v;

			v.x = 0.58f;
			v.y = 1.25f;

			BoxCollider.size = v;

			v.x = 0f;
			v.y = -0.38f;

			CircleCollider.offset = v;

			CircleCollider.radius = 0.28f;
			break;
		case PlayerStates.BigFire:

			break;

		}
	}

	// Update is called once per frame
	void Update () {
		if (!PauseControl) {
			isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, whatIsGround);

			if (knockBackCounter <= 0 && canMove) {
				if (onPlatform) {
					activeMoveSpeed = (moveSpeed + currentAdditionalSpeedOnRun) * onPlatformSpeedModifier;
				} else {
					activeMoveSpeed = moveSpeed + currentAdditionalSpeedOnRun;
				}
				if (Input.GetAxisRaw ("Horizontal") > 0f) {
					myRigidbody.velocity = new Vector3 (activeMoveSpeed, myRigidbody.velocity.y, 0f);
					mySpriteRenderer.flipX = false;
					PlayerGrowShrinkRenderer.flipX = false;
				} else if (Input.GetAxisRaw ("Horizontal") < 0f) {
					myRigidbody.velocity = new Vector3 (-activeMoveSpeed, myRigidbody.velocity.y, 0f);
					mySpriteRenderer.flipX = true;
					PlayerGrowShrinkRenderer.flipX = true;
				} else {
					myRigidbody.velocity = new Vector3 (0f, myRigidbody.velocity.y, 0f);
				}
				if (Input.GetButtonDown ("Jump") && isGrounded) {
					myRigidbody.velocity = new Vector3 (myRigidbody.velocity.x, jumpSpeed + currentAdditionalJumpSpeed, 0f);
					if (PlayerState == PlayerStates.Small)
						JumpAudioSource.Play ();
					else
						JumpBigAudioSource.Play ();
				}
				if (Input.GetButtonDown ("Run") && isGrounded) {
					Debug.Log ("Pressing on run buttton!");
					currentAdditionalSpeedOnRun = additionalSpeedOnRun;
					currentAdditionalJumpSpeed = additionalJumpSpeed;
				}
				if (Input.GetButtonUp ("Run")) {
					Debug.Log ("slow down!");
					currentAdditionalSpeedOnRun = 0;
					currentAdditionalJumpSpeed = 0;
				}
				theLevelManager.invincible = false;
			}

			if (knockBackCounter > 0) {
				knockBackCounter -= Time.deltaTime;

				if (mySpriteRenderer.flipX)
					myRigidbody.velocity = new Vector3 (knockBackForce, knockBackForce / 2, 0f);
				else
					myRigidbody.velocity = new Vector3 (-knockBackForce, knockBackForce / 2, 0f);
			}

			if (invincibilityCounter > 0) {
				invincibilityCounter -= Time.deltaTime;

			} else {
				theLevelManager.invincible = false;
			}

			myAnim.SetFloat ("Speed", Mathf.Abs (myRigidbody.velocity.x));
			myAnim.SetBool ("Grounded", isGrounded);

			if (myRigidbody.velocity.y < 0) {
				stompBox.SetActive (true);

			} else {
				stompBox.SetActive (false);
			}
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
			//theLevelManager.Respawn ();

		}
		if (other.tag == "Checkpoint") {
			respawnPosition = other.transform.position;
		}
		if (other.tag == "Mushroom") {
			Destroy (other.gameObject);

			PlayerState = PlayerStates.Big;

			PauseMarioWhileGrowOrShrink ();

			//TransformMarioToNewSize ();
			myAnim.Play ("GrowPlayerIdle");
		}
	}
	void PauseMarioWhileGrowOrShrink() {
		PowerupAudioSource.Play ();
		PauseControl = true;
		saveVelocity = myRigidbody.velocity;
		//saveAngularVelocity = myRigidbody.angularVelocity;
		myRigidbody.velocity = Vector2.zero;
		myRigidbody.isKinematic = true;
	}
	void OnMarioGrowFinish() {
		TransformMarioToNewSize ();
		UnpauseMario ();
		PauseControl = false;
	}
	void UnpauseMario() {
		myRigidbody.isKinematic = false;
		myRigidbody.velocity = saveVelocity;
		//myRigidbody.angularVelocity = saveAngularVelocity;
		myRigidbody.WakeUp ();
	}
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "MovingPlatform") {
			transform.parent = other.transform;
			onPlatform = true;
		}
		if (other.gameObject.tag == "Mushroom") {
			Debug.Log ("in collision");
			Destroy (other.gameObject);

			PlayerState = PlayerStates.Big;

			PauseMarioWhileGrowOrShrink ();

			//TransformMarioToNewSize ();
			myAnim.Play ("GrowPlayerIdle");
		}
	}
	void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.tag == "MovingPlatform") {
			transform.parent = null;
			onPlatform = false;
		}
	}
}
