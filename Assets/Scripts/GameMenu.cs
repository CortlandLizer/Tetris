using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {

    public Text levelText;
    // Use this for initialization
    void Start() {
        levelText.text = "0";
    }

    // Update is called once per frame
    void Update() {

    }

    public void PlayGame() {

        if( Game.startingLevel == 0) {
            Game.startingAtLevelZero = true;
        }
        else {
            Game.startingAtLevelZero = false;
        }

        SceneManager.LoadScene("Level");
    }

    public void ChangeValue(float value) {

        Game.startingLevel = (int)value;
        levelText.text = value.ToString();
    }

    public void LaunchGameMenu() {

        SceneManager.LoadScene("GameMenu");
    }




}
