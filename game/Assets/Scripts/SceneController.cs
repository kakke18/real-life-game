using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public static bool regi = false;
    public static bool log = false;

    public void StartUp () {
        SceneManager.LoadScene("StartUp");
    }

    public void Registration () {
        SceneManager.LoadScene("Registration");
    }

    public void SystemDescription () {
        SceneManager.LoadScene("SystemDescription");
        regi = true;
        log = false;
    }

    public void Login () {
        SceneManager.LoadScene("Login");
        regi = false;
        log = true;
    }

    public void MyPage () {
        SceneManager.LoadScene("MyPage");
    }

    public void GameTitle () {
        SceneManager.LoadScene("GameTitle");
    }

    public void GameDescription () {
        SceneManager.LoadScene("GameDescription");
    }

    public void Ranking () {
        SceneManager.LoadScene("Ranking");
    }

    public void Game () {
        SceneManager.LoadScene("Game");
    }
}
