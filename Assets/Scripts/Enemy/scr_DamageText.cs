using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scr_DamageText : MonoBehaviour {

    public float moveSpeed = 1.5f;
    public float fadeDuration = 1f;
    private TextMeshProUGUI textMesh;
    private Color originalColor;

    private void Awake() {
        textMesh = GetComponent<TextMeshProUGUI>();
        originalColor = textMesh.color;
    }

    public void Initialize(int damageAmount) {
        textMesh.text = damageAmount.ToString();
        originalColor.a = 1;
        textMesh.color = originalColor;
        Destroy(gameObject, fadeDuration);
    }

    private void Update() {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        originalColor.a -= Time.deltaTime / fadeDuration;
        textMesh.color = originalColor;
    }

}