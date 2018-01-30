using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    Rigidbody2D rb;
    SpriteRenderer spr;
    public int p;
    float speed = 1;
    List<Husk> nearbyHusks = new List<Husk>();
    public Transform tower;
    public float towerHp = 100;
    public bool controllingHusks;
    public List<Husk> controllingList = new List<Husk>();
    public List<Husk> husks = new List<Husk>();
    internal Vector2 lStickAxis;
    public int gold;
    public bool towerDestroyed;

    //controller stuff
    bool aHeld;
    bool yHeld;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        StartCoroutine(SlowUpdate());
	}
	
	// Update is called once per frame
	void Update () {

        if (!C.gameStarted) return;

        var gp = GamePad.GetState((PlayerIndex)p);

        if (nearbyHusks.Count != 0) {
            if (gp.Buttons.A == ButtonState.Pressed && !aHeld) { //possess
                nearbyHusks[0].possessProgress += .5f;
                nearbyHusks[0].transform.GetChild(4).gameObject.SetActive(true);
                if (nearbyHusks[0].possessProgress > 5f) {
                    C.am.PlaySound(1);
                    nearbyHusks[0].Possess(p);
                    nearbyHusks.RemoveAt(0);
                } else { C.am.PlaySound(0); }
            }
        }

        if (!towerDestroyed) {
            if (Vector3.Distance(transform.position, tower.position) < 2) { //near tower
                if (gp.Buttons.A == ButtonState.Pressed && !aHeld && controllingList.Count < husks.Count) { //control peeps
                    for (var i = 0; i < husks.Count; i++) {
                        if (!husks[i].holdingGold && !husks[i].controlling) {
                            C.am.PlaySound(2);
                            husks[i].controlStartStop(true);
                            controllingList.Add(husks[i]);
                            break;
                        }
                    }
                }
                if (gp.Buttons.Y == ButtonState.Pressed && !yHeld) { //buy a Husk
                    if (gold >= 10) {
                        C.am.PlaySound(5);
                        gold -= 10;
                        C.c.goldTexts[p].text = gold.ToString();
                        var inst = Instantiate(C.c.prefabs[2], C.c.towers[p].position + Vector3.down, Quaternion.identity).GetComponent<Husk>();
                        inst.Possess(p);
                    }
                }
            }
        }

        if (controllingList.Count > 0) { //Deselect Peeps
            if (gp.Buttons.B == ButtonState.Pressed) {
                foreach(Husk h in controllingList) {
                    h.controlStartStop(false);
                }
                controllingList.Clear();
            }
        }
        
        lStickAxis = new Vector2(gp.ThumbSticks.Left.X, gp.ThumbSticks.Left.Y);

        if (controllingList.Count == 0) { //not controlling yer bois
            rb.velocity += lStickAxis; //move
        }

        if (rb.velocity.magnitude > 3) rb.velocity = rb.velocity.normalized * 3;

        if (rb.velocity.x < 0) spr.flipX = true;
        if (rb.velocity.x > 0) spr.flipX = false;

        rb.velocity *= .9f;

        aHeld = (gp.Buttons.A == ButtonState.Pressed);
        yHeld = (gp.Buttons.Y == ButtonState.Pressed);

        //tower death
        C.c.hpBars[p].localScale = new Vector2(towerHp / 150, 1);
        if (towerHp <= 0 && !towerDestroyed) {
            towerDestroyed = true;
            C.am.PlaySound(10);
            Destroy(C.c.towers[p].gameObject);
            C.c.gameOverPanel.SetActive(true);
            if (p == 0) C.c.gameOverPanel.transform.GetChild(0).GetComponent<Text>().text = "Player 2 Wins!!!";
            else C.c.gameOverPanel.transform.GetChild(0).GetComponent<Text>().text = "Player 1 Wins!!!";
            C.gameOver = true;
        }

    }

    IEnumerator SlowUpdate() {
        while (true) {
            var pos = transform.position;
            pos.z = 1 + pos.y * .01f;
            transform.position = pos;
            yield return new WaitForSeconds(.25f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Husk")) {
            if (collision.GetComponent<Husk>().type == 0)
                nearbyHusks.Add(collision.GetComponent<Husk>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Husk")) {
            if (collision.GetComponent<Husk>().type == 0)
                nearbyHusks.Remove(collision.GetComponent<Husk>());
        }
    }

}
