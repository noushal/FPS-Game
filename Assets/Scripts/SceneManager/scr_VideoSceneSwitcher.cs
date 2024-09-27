using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class scr_VideoSceneSwitcher : MonoBehaviour {

    private VideoPlayer videoPlayer;

    void Start() {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp) {
        SceneManager.LoadScene("MainMenu");
    }

}
