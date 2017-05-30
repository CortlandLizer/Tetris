using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Tetromino : MonoBehaviour {


	float Fall = 0;
	private float FallSpeed; //see public fallSpeed
	public bool allowRotation = true;
	public bool limitRotation = false;



	public AudioClip moveSound;
	public AudioClip rotateSound;
	public AudioClip landSound;


	public float continuousVerticalSpeed = 0.05f;  //speed to move when down arrow i pressed
	public float continuousHorizontalSpeed = 0.1f; // speed hat left or right moves        0.1f is default make smaller for faster (.01 is to fast .05 is to slow) [BEST RANGE][   ]
	public float buttonDownWaitMax = 0.2f; // how long to wait before tetrimion recognizes butto is held 

	public float buttonDownWaitTimerHorizontal = 0; //probably shoul be private b/c there is no reason to change this ...also it cant really be changed
	public float buttonDownWaitTimerVirtical = 0; //probably shoul be private b/c there is no reason to change this ...also it cant really be changed


	private bool movedImmediateHorizontal = false;
	private bool movedImmediateVertical = false;

	private float verticalTimer = 0;
	private float horizonalTimer = 0;

	public int individualScore = 100;
	public float individualScoreTime;

	private AudioSource audioSource;
	// Use this for initialization
	void Start() {

		audioSource = GetComponent<AudioSource>();

		FallSpeed = GameObject.Find("GameScript").GetComponent<Game>().fallSpeed; //this might be a good way to change controls in a menu 
	}

	// Update is called once per frame
	void Update() {

		CheckUserInput();
		UpdateIndividualScore();
	}

	void UpdateIndividualScore() {

		if (individualScoreTime < 1) {

			individualScoreTime += Time.deltaTime;

		}
		else {

			individualScoreTime = 0;

			individualScore = Mathf.Max(individualScore - 10, 0);
		}
	}

	void CheckUserInput() {
		bool right = Input.GetKey(KeyCode.RightArrow);
		bool left = Input.GetKey(KeyCode.LeftArrow);
		bool down = Input.GetKey(KeyCode.DownArrow);
		bool up = Input.GetKeyDown(KeyCode.UpArrow);
		bool rotate = Input.GetKeyDown(KeyCode.Space);

		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) {

			movedImmediateHorizontal = false;
			horizonalTimer = 0;
			buttonDownWaitTimerHorizontal = 0;			
		}

		if (Input.GetKeyUp(KeyCode.DownArrow)) {

			movedImmediateVertical = false;
			verticalTimer = 0;
			buttonDownWaitTimerVirtical = 0;
		}

		if (left) { // move peice LEFT

			MoveLeft();
		}
		 if (right) { // move peice Right

			MoveRight();
		}

		if (rotate) { //Rotate the peice

			Rotate();
		}
		if (down || Time.time - Fall >= FallSpeed) { // move peice Down by button and by time

			MoveDown();
		}
	}

	void MoveRight() {
		if (movedImmediateHorizontal) {

			if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {

				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}

			if (horizonalTimer < continuousHorizontalSpeed) {
				horizonalTimer += Time.deltaTime;
				return;
			}
		}

		if (!movedImmediateHorizontal) {
			movedImmediateHorizontal = true;
		}

		horizonalTimer = 0;

		transform.position += new Vector3(1, 0, 0);

		if (CheckIsValidPosition()) {

			FindObjectOfType<Game>().UpdateGrid(this);
			PlayMoveAudio();

		}
		else {
			transform.position += new Vector3(-1, 0, 0);
		}
	}

	void MoveLeft() {
		if (movedImmediateHorizontal) {

			if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {

				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}

			if (horizonalTimer < continuousHorizontalSpeed) {
				horizonalTimer += Time.deltaTime;
				return;
			}
		}

		if (!movedImmediateHorizontal) {
			movedImmediateHorizontal = true;
		}

		horizonalTimer = 0;

		transform.position += new Vector3(-1, 0, 0);

		if (CheckIsValidPosition()) {

			FindObjectOfType<Game>().UpdateGrid(this);
			PlayMoveAudio();
		}
		else {

			transform.position += new Vector3(1, 0, 0);
		}
	}

	void MoveDown() {
		bool down = Input.GetKey(KeyCode.DownArrow);
		if (movedImmediateVertical) {

			if (buttonDownWaitTimerVirtical < buttonDownWaitMax) {

				buttonDownWaitTimerVirtical += Time.deltaTime;
				return;
			}

			if (verticalTimer < continuousVerticalSpeed) {
				verticalTimer += Time.deltaTime;
				return;
			}
		}

		if (!movedImmediateVertical) {
			movedImmediateVertical = true;
		}

		verticalTimer = 0;

		transform.position += new Vector3(0, -1, 0);

		Fall = Time.time;

		if (CheckIsValidPosition()) {

			FindObjectOfType<Game>().UpdateGrid(this);
			if (down) {
				PlayMoveAudio();

			}
		}
		else {
			transform.position += new Vector3(0, 1, 0);

			FindObjectOfType<Game>().DeleteRow();

			if (FindObjectOfType<Game>().CheckIsAboveGrid(this)) {

				FindObjectOfType<Game>().GameOver();
			}

			enabled = false;

			Game.currentScore += individualScore;

			PlayLandAudio();
			//spawn next peice
			FindObjectOfType<Game>().SpawnNextTetromino();
		}
	
}

	void Rotate() {
		if (allowRotation) {
			if (limitRotation) {
				if (transform.rotation.eulerAngles.z >= 90) {

					transform.Rotate(0, 0, -90);
				}
				else {

					transform.Rotate(0, 0, 90);
				}
			}
			else {

				transform.Rotate(0, 0, 90);
			}

			if (CheckIsValidPosition()) {

				FindObjectOfType<Game>().UpdateGrid(this);
				PlayRotateAudio();

			}
			else {
				if (limitRotation) {
					if (transform.rotation.eulerAngles.z >= 90) {
						transform.Rotate(0, 0, -90);
					}
					else {
						transform.Rotate(0, 0, 90);
					}
				}
				else {
					transform.Rotate(0, 0, -90);
				}
			}
		}
	}


	void PlayMoveAudio() {

		audioSource.PlayOneShot(moveSound);
	}

	void PlayRotateAudio() {

		audioSource.PlayOneShot(rotateSound);
	}

	void PlayLandAudio() {

		audioSource.PlayOneShot(landSound);
	}

	bool CheckIsValidPosition() {

		foreach (Transform mino in transform) {

			Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
			if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false) {
				return false;
			}

			if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform) {

				return false;

			}
		}
		return true;
	}

}