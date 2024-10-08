using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_Scenes : MonoBehaviour {

    
    private void Start() {
        UpdateCursorState();
    }

    private void Update() {
        UpdateCursorState();
    }

    private void UpdateCursorState() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "MainMenu" || sceneName == "EndMenu") {
            UnlockCursor();
        } else {
            LockCursor();
        }
    }

    private void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play() {
        SceneManager.LoadScene("GameScene");
    }

    public void Quit() {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

}