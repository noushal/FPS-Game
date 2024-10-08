using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_GameManager : MonoBehaviour {

    public static scr_GameManager Instance;

    public TMP_Text gameOverText;

    public int totalEnemies;
    private int remainingEnemies;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        remainingEnemies = totalEnemies;
    }

    public void EnemyKilled() {
        remainingEnemies--;
        if (remainingEnemies <= 0) {
            GameOver();
            remainingEnemies = totalEnemies;
        }
    }

    private void GameOver() {
        StartCoroutine(GameEnd());
    }

    private IEnumerator GameEnd() {
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("EndMenu");
    }

}