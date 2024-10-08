using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ScopeScript : MonoBehaviour {

    public GameObject ZoomedScope;
    public GameObject Sniper;
    public GameObject ScopeImage;
    public GameObject Crosshair;

    public bool isSniper = false;

    void Start() {

    }
    void Update() {
        if (isSniper) {
            if (Input.GetMouseButtonDown(1)) {
                ZoomIn();
            }

            if (Input.GetMouseButtonUp(1)) {
                ZoomOut();
            }
        }
    }

    void ZoomIn() {
        ScopeImage.SetActive(true);
        ZoomedScope.SetActive(true);
        Crosshair.SetActive(false);
    }

    void ZoomOut() {
        ScopeImage.SetActive(false);
        ZoomedScope.SetActive(false);
        Crosshair.SetActive(true);
    }

}