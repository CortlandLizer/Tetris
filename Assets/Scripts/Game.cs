﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingAtLevelZero;
    public static int startingLevel;

    public int scoreOneLine = 40;
    public int ScoreTwoLine = 100;
    public int ScoreThreeLine = 300;
    public int ScoreFourLine = 800;

    public int currentLevel = 0;
    public int numLinesCleared = 0; //private?

    public static float fallSpeed = 1.0f;

    public AudioClip clearedLinesSound;

    public Text hud_score;
    public Text hud_Level;
    public Text hud_Lines;

    private int numberOfRowsThisTurn = 0;

    private AudioSource audioSource;

    public static int currentScore = 0;

    private GameObject previewTetromino;
    private GameObject previewTetromino1;
    private GameObject previewTetromino2;
    private GameObject nextTetromino;

    private bool gameStarted = false;

    //starting point for Tetriminos
    public static float startPositionX = 6.0f;
    public static float startPositionY = 20.0f;

    public static float previewXPositon = -3.5f;
    public static int previewYPosition = 10;



    private Vector2 previewTetrominoPostion = new Vector2(previewXPositon, previewYPosition); //make float and in vars and add some later
    private Vector2 previewTetrominoPostion1 = new Vector2(previewXPositon, previewYPosition + 3);
    private Vector2 previewTetrominoPostion2 = new Vector2(previewXPositon, previewYPosition + 6);


    //saved Tetriminos
    private Vector2 savedTetrominPosition = new Vector2(-11.0f, 17);
    private GameObject savedTetromino;
    public int maxSwaps = 2;
    private int currentSwaps = 0;

    // Use this for initialization
    void Start() {

        currentLevel = startingLevel;

        SpawnNextTetromino();
        audioSource = GetComponent<AudioSource>();
        //numLinesCleared = currentLevel * 10;
    }

    void Update() {

        UpdateScore();

        CheckUserInput();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
    }


    void CheckUserInput() {
      //  bool right = Input.GetKey(KeyCode.RightArrow);
      //  bool left = Input.GetKey(KeyCode.LeftArrow);
      //  bool down = Input.GetKey(KeyCode.DownArrow);
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
      //  bool rotate = Input.GetKeyDown(KeyCode.Space);

        if (up) {

            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");
            SaveTetromino (tempNextTetromino.transform);
        }
    }

        void UpdateLevel() {
        if ((startingAtLevelZero == true) || (startingAtLevelZero == false && numLinesCleared / 10 > startingLevel)) {
            currentLevel = numLinesCleared / 10;
            // Debug.Log("Current Level : " + currentLevel);
        }
    }

    void UpdateSpeed() {

        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
        //Debug.Log("Fall Speed :" + fallSpeed);
    }

    public void UpdateUI() {

        hud_score.text = currentScore.ToString();
        hud_Level.text = currentLevel.ToString();
        hud_Lines.text = numLinesCleared.ToString();
    }
    public void UpdateScore() {

        if (numberOfRowsThisTurn > 0) {

            if (numberOfRowsThisTurn == 1) {

                ClearedOneLine();

            }
            else if (numberOfRowsThisTurn == 2) {

                ClearedTwoLines();

            }
            else if (numberOfRowsThisTurn == 3) {

                ClearedThreeLines();

            }
            else if (numberOfRowsThisTurn == 4) {

                ClearedFourLines();
            }

            numberOfRowsThisTurn = 0;

            PlayLineClearedSound();
        }

    }

    public void ClearedOneLine() {

        currentScore += scoreOneLine + (currentLevel * 20); //can chan num later to make points multiplier variable 
        numLinesCleared++;
    }

    public void ClearedTwoLines() {

        currentScore += ScoreTwoLine + (currentLevel * 25);
        numLinesCleared += 2;
    }

    public void ClearedThreeLines() {

        currentScore += ScoreThreeLine + (currentLevel * 30);
        numLinesCleared += 3;
    }

    public void ClearedFourLines() {

        currentScore += ScoreFourLine + (currentLevel * 45);
        numLinesCleared += 4;
    }

    public void PlayLineClearedSound() {

        audioSource.PlayOneShot(clearedLinesSound);
    }

    bool CheckIsValidPosition(GameObject tetrimino) {
        foreach (Transform mino in tetrimino.transform) {
            Vector2 pos = Round(mino.position);

            if (CheckIsInsideGrid(pos)) {
                return false;
            }

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetrimino.transform) {
                return false;
            }
            // return true;
        }
        return true;

    }

    public bool CheckIsAboveGrid(Tetromino tetromino) {

        for (int x = 0; x < gridWidth; ++x) {

            foreach (Transform mino in tetromino.transform) {

                Vector2 pos = Round(mino.position);

                if (pos.y > gridHeight - 1) {
                    return true;
                }

            }
        }

        return false;
    }


    public bool IsFullRowAt(int y) {
        for (int x = 0; x < gridWidth; ++x) {
            if (grid[x, y] == null) {

                return false;
            }
        }

        // since found full row increment the full row var.
        numberOfRowsThisTurn++;

        return true;
    }

    public void DeleatMinoAt(int y) {
        for (int x = 0; x < gridWidth; ++x) {

            Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y) {
        for (int x = 0; x < gridWidth; ++x) {
            if (grid[x, y] != null) {

                grid[x, y - 1] = grid[x, y];

                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y) {

        for (int i = y; i < gridHeight; ++i) {

            MoveRowDown(i);
        }
    }

    public void DeleteRow() {
        for (int y = 0; y < gridHeight; ++y) {

            if (IsFullRowAt(y)) {

                DeleatMinoAt(y);

                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }

    public void UpdateGrid(Tetromino tetromino) {

        for (int y = 0; y < gridHeight; ++y) {

            for (int x = 0; x < gridWidth; ++x) {
                if (grid[x, y] != null) {

                    if (grid[x, y].parent == tetromino.transform) {

                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform) {
            Vector2 pos = Round(mino.position);

            if (pos.y < gridHeight) {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos) {
        if (pos.y > gridHeight - 1) {
            return null;
        }
        else {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetromino() {

        if (!gameStarted) {

            gameStarted = true;

            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(startPositionX, startPositionY), Quaternion.identity);

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPostion, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;


            previewTetromino1 = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPostion1, Quaternion.identity);
            previewTetromino1.GetComponent<Tetromino>().enabled = false;

            previewTetromino2 = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPostion2, Quaternion.identity);
            previewTetromino2.GetComponent<Tetromino>().enabled = false;


            nextTetromino.tag = "currentActiveTetromino";

        }
        else {
            previewTetromino.transform.localPosition = new Vector2(startPositionX, startPositionY);
            previewTetromino1.transform.localPosition = previewTetrominoPostion;//2
            previewTetromino2.transform.localPosition = previewTetrominoPostion1;//3

            nextTetromino = previewTetromino;//1
            nextTetromino.GetComponent<Tetromino>().enabled = true;//1

            previewTetromino = previewTetromino1;//2
            previewTetromino1 = previewTetromino2;//3
                                                  //previewTetromino.GetComponent<Tetromino>().enabled = true;//1

            //previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPostion, Quaternion.identity);//1
            //previewTetromino.GetComponent<Tetromino>().enabled = false;//1

            //previewTetromino1 = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPostion1, Quaternion.identity);//2
            //previewTetromino1.GetComponent<Tetromino>().enabled = false;//2

            previewTetromino2 = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPostion2, Quaternion.identity); //3
            previewTetromino2.GetComponent<Tetromino>().enabled = false;//3

            nextTetromino.tag = "currentActiveTetromino";
        }

        currentSwaps = 0;

    }


    public void SaveTetromino(Transform t) { //aaaaaaaaaaaaaaaaaaaaa

        currentSwaps++;

        if (currentSwaps > maxSwaps) {
            return;
        }

        if (savedTetromino != null) {//there is currently a tetromino being held 

            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavedTetromino");

            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight); //puts new peice at top of board ?

            if(!CheckIsValidPosition(tempSavedTetromino)) {

                tempSavedTetromino.transform.localPosition = savedTetrominPosition;
            }

            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominPosition;
            savedTetromino.tag = "currentSavedTetromino";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);
        }
        else {//there is currently NO tetromino being held 

            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominPosition;
            savedTetromino.tag = ("currentSavedTetromino");

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));

            SpawnNextTetromino();
        }
    }

    public bool CheckIsInsideGrid(Vector2 pos) {

        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos) {

        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetromino() {
        int randomTetromino = Random.Range(1, 8);

        string randomeTetrominoName = "PreFabs/Tetromino_T";

        switch (randomTetromino) {
            case 1:
                randomeTetrominoName = "PreFabs/Tetromino_T";
                break;
            case 2:
                randomeTetrominoName = "PreFabs/Tetromino_I";
                break;
            case 3:
                randomeTetrominoName = "PreFabs/Tetromino_B";
                break;
            case 4:
                randomeTetrominoName = "PreFabs/Tetromino_J";
                break;
            case 5:
                randomeTetrominoName = "PreFabs/Tetromino_L";
                break;
            case 6:
                randomeTetrominoName = "PreFabs/Tetromino_S";
                break;
            case 7:
                randomeTetrominoName = "PreFabs/Tetromino_Z";
                break;
        }
        return randomeTetrominoName;
    }

    public void GameOver() {

        SceneManager.LoadScene("Game Over");
        // was Application.LoadLevel(string) but thats depricaated
    }
}

