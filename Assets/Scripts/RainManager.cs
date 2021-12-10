using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This is the script that generates rain.
 * The mechanism is simple: every 5 sec there is a chance for rain to show up. When it rains, the "fire time" decreases twice as fast. (see Fire.cs)
 */

public class RainManager : MonoBehaviour
{
    #region SINGLETON

    public static RainManager _instance;
    public static RainManager Instance {
        get {
            if(_instance == null) {
                _instance = FindObjectOfType<RainManager>();
                if(_instance == null) {
                    GameObject container = new GameObject("RainManager");
                    _instance = container.AddComponent<RainManager>();
                }
            }

            return _instance;

        }
    }

    #endregion

    public bool isRaining;
    public float rainChanceNormalized;

    SpriteRenderer sp;
    float rainTimer;

    private void Start() {
        sp = GetComponent<SpriteRenderer>();
        StartCoroutine(RainSpawner());
    }

    private void Update() {
        if (isRaining) {
            rainTimer -= Time.deltaTime;
            if (rainTimer <= 0) {
                rainTimer = 0;
                isRaining = false;
                StartCoroutine(MakeRainDisappear());
            }
        }
    }

    IEnumerator RainSpawner() {
        while(true) {
            float rainChance = Random.Range(0f + (GameManager.Instance.difficulty / 13), 1f);
            if(rainChance >= rainChanceNormalized) {
                isRaining = true;
                rainTimer = Random.Range(5 + (GameManager.Instance.difficulty/5), 10 + GameManager.Instance.difficulty);
                StartCoroutine(MakeRainAppear());
            }
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator MakeRainAppear() {

        while(sp.color.a < 0.4f) {

            Color newColor = sp.color;
            newColor.a += Time.deltaTime * 1.5f;
            sp.color = newColor;

            yield return null;
        }

        Color finalColor = sp.color;
        finalColor.a = 0.4f;
        sp.color = finalColor;

    }

    IEnumerator MakeRainDisappear() {

        while (sp.color.a > 0) {

            Color newColor = sp.color;
            newColor.a -= Time.deltaTime * 1.5f;
            sp.color = newColor;

            yield return null;
        }

        Color finalColor = sp.color;
        finalColor.a = 0f;
        sp.color = finalColor;

    }

}
