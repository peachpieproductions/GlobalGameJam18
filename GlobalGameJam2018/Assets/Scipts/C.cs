using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

public class C : MonoBehaviour {

    public static C c;
    public int size = 32;
    public GameObject[] prefabs;
    public Sprite[] sprites;
    public Text[] goldTexts;
    public Transform[] towers;
    public Player[] players;
    public RectTransform[] hpBars;
    float spawnHuskTimer;
    public Image fadePanel;
    public float fadeIn = 1.2f;
    public RectTransform title;
    bool titleShowing = true;
    bool showTutorial = true;
    public static bool gameStarted;
    public static bool gameOver;
    public static AudioManager am;
    public GameObject gameOverPanel;
    public GameObject tutorialImage;
    public bool aHeld;

    // Use this for initialization
    void Start () {
        c = this;
        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        fadePanel.gameObject.SetActive(true);

        //tower z depth
        foreach(Transform t in towers) {
            var pos = t.position;
            pos.z = 1 + pos.y * .01f;
            t.position = pos;
        }

        //ground tiles
        for(var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                Instantiate(prefabs[0], new Vector2(i - size / 2, j - size / 2), Quaternion.identity);
            }
        }
        for (var i = 0; i < 25; i++) {
            Instantiate(prefabs[2], new Vector2(Random.Range(-3, 3), Random.Range(-3, 3)), Quaternion.identity);
        }
        for (var i = 0; i < 25; i++) {
            Instantiate(prefabs[2], new Vector2(Random.Range(-12, 12), Random.Range(-5, 5)), Quaternion.identity);
        }
    }

    private void Update() {

        if (GamePad.GetState((PlayerIndex)0).Buttons.Back == ButtonState.Pressed) {
            ScreenCapture.CaptureScreenshot(Random.Range(0, 1000).ToString());
        }

        if (gameOver) {
        if (GamePad.GetState((PlayerIndex)0).Buttons.Start == ButtonState.Pressed) {
            gameOver = false;
            gameStarted = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        return;
        }

        if (fadeIn > 0) {
            if (title.localPosition.y > 110) title.localPosition = new Vector2(0, title.localPosition.y - Time.deltaTime * 200);
            else if (title.localPosition.y > 0) title.localPosition = new Vector2(0, title.localPosition.y - title.localPosition.y * Time.deltaTime );
            fadeIn -= Time.deltaTime * .25f;
            var col = fadePanel.color;
            col.a = fadeIn;
            fadePanel.color = col;
            if (fadeIn <= 0) fadePanel.gameObject.SetActive(false);
        }

        else if (titleShowing) {
            if (GamePad.GetState((PlayerIndex)0).Buttons.A == ButtonState.Pressed && !aHeld) {
                titleShowing = false;
                tutorialImage.SetActive(true);
                StartCoroutine(CloseTitleImage());
            }
        }

        else if (showTutorial) {
            if (GamePad.GetState((PlayerIndex)0).Buttons.A == ButtonState.Pressed && !aHeld) {
                showTutorial = false;
                gameStarted = true;
                tutorialImage.SetActive(false);
            }
        }

        aHeld = (GamePad.GetState((PlayerIndex)0).Buttons.A == ButtonState.Pressed);

        if (gameStarted) {
            if (spawnHuskTimer > 0) spawnHuskTimer -= Time.deltaTime;
            else {
                spawnHuskTimer = 6f;
                Instantiate(prefabs[2], new Vector3(Random.Range(-8, 8), (Random.value < .5) ? -8 : 8), Quaternion.identity);
            }
        }
    }

    IEnumerator CloseTitleImage() {
        while (true) {
            var scale = title.localScale;
            scale.x *= .9f;
            title.localScale = scale;
            if (scale.x < .05) {
                title.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }


}
