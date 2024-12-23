using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_GameManager : MonoBehaviour {

    public static scr_GameManager Instance;

    public TMP_Text gameOverText;
    private AudioSource winningSFX;

    public int totalEnemies;
    private int remainingEnemies;

    public TextMeshProUGUI enemyKilledCount;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        //enemyKilledCount.text = "Killed" + remainingEnemies + "/" + totalEnemies;
    }
    private void Start() {
        remainingEnemies = totalEnemies;
        enemyKilledCount = GetComponent<TextMeshProUGUI>();
        winningSFX = GetComponent<AudioSource>();
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
        winningSFX.Play();
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("EndMenu");
    }

}