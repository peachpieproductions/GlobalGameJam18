using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour {

    Rigidbody2D rb;
    SpriteRenderer spr;
    public int p;
    float speed = 1;
    List<Husk> nearbyHusks = new List<Husk>();
    public Transform tower;
    public bool controllingHusks;
    public List<Husk> controllingList = new List<Husk>();
    public List<Husk> husks = new List<Husk>();
    internal Vector2 lStickAxis;
    public int gold;

    //controller stuff
    bool aHeld;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        var gp = GamePad.GetState((PlayerIndex)p);

        if (nearbyHusks.Count != 0) {
            if (gp.Buttons.A == ButtonState.Pressed && !aHeld) { //possess
                nearbyHusks[0].possessProgress += .5f;
                if (nearbyHusks[0].possessProgress > 6f) {
                    nearbyHusks[0].master = this;
                    nearbyHusks[0].idle = false;
                    nearbyHusks[0].spr.sprite = C.c.sprites[1+p];
                    nearbyHusks[0].type = p + 1;
                    husks.Add(nearbyHusks[0]);
                    nearbyHusks.RemoveAt(0);
                }
            }
        }

        if (Vector3.Distance(transform.position,tower.position) < 2) { //control peeps
            if (gp.Buttons.A == ButtonState.Pressed && !aHeld && controllingList.Count < husks.Count) {
                Husk husk = husks[0];
                int attempt = 0;
                while (husk == null || husk.holdingGold || husk.controlling) {
                    husk = husks[Random.Range(0, husks.Count)];
                    attempt++; if (attempt > 100) break;
                }
                husk.controlStartStop(true);
                controllingList.Add(husk);
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
