using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Tetromino : MonoBehaviour {


    float Fall = 0;
    public float FallSpeed = 1;
    public bool allowRotation = true;
    public bool limitRotation = false;


    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip landSound;


    public static float verticalMoveSpeed = 0.05f;  //speed to move when down arrow i pressed
    public static float horizontalMoveSpeed = 0.1f; // speed hat left or right moves        0.1f is default make smaller for faster (.01 is to fast .05 is to slow) [BEST RANGE][   ]
    public static float buttonDownMax = 0.2f; // how long to wait before tetrimion recognizes butto is held 
    public static float buttonDownTimer = 0;

    private float continuousVerticalSpeed = verticalMoveSpeed;
    private float continuousHorizontalSpeed = horizontalMoveSpeed;
    private float buttonDownWaitMax = buttonDownMax;
    private float buttonDownWaitTimer = buttonDownTimer;

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
        //bool down = Input.GetKeyDown(KeyCode.DownArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        bool turn = Input.GetKeyDown(KeyCode.Space);

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.DownArrow)) {
            horizonalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimer = 0;
            movedImmediateHorizontal = false;
            movedImmediateVertical = false;
        }

        if (left) { // move peice LEFT

            if (movedImmediateHorizontal) {

                if (buttonDownWaitTimer < buttonDownWaitMax) {

                    buttonDownWaitTimer += Time.deltaTime;
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
        else if (right) { // move peice Right
            if (movedImmediateHorizontal) {

                if (buttonDownWaitTimer < buttonDownWaitMax) {

                    buttonDownWaitTimer += Time.deltaTime;
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
        else if (up) {
            // transform.position += new Vector3( 0, , 0);

        }
        else if (turn) {
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
        else if (down || Time.time - Fall >= FallSpeed) { // move peice Down by button and by time
            if (movedImmediateVertical) {

                if (buttonDownWaitTimer < buttonDownWaitMax) {

                    buttonDownWaitTimer += Time.deltaTime;
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
