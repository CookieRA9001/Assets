using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemuFunctions : MonoBehaviour {
    public static void StartGame() {
        SceneManager.LoadScene("CookieRATestScene", LoadSceneMode.Single);
    }

    public static void ExitGame() {
        Application.Quit();
    }

    public static void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}
