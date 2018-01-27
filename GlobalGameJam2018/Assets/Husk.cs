using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Husk : MonoBehaviour {

    Rigidbody2D rb;
    internal SpriteRenderer spr;
    internal bool idle = true;
    internal Player master;
    public int type = 0; //0 human, 1 p1, 2 p2
    internal bool controlling;
    internal float possessProgress;
    internal bool holdingGold;

	void Awake () {
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        StartCoroutine(Idle());
	}

    private void Update() {
        if (master != null) {
            if (controlling) {
                rb.velocity += master.lStickAxis;
                if (rb.velocity.magnitude > 3) rb.velocity = rb.velocity.normalized * 3;
            } else {
                if (Vector3.Distance(transform.position, master.tower.position) > 2) {
                    idle = false;
                    if (rb.velocity.magnitude < .5f) rb.velocity += Vector2.down * 7;
                    rb.velocity += (Vector2)(master.tower.position - transform.position).normalized;
                    if (rb.velocity.magnitude > 3) rb.velocity = rb.velocity.normalized * 3;
                } else {
                    if (holdingGold) {
                        holdingGold = false;
                        transform.GetChild(1).gameObject.SetActive(false);
                        master.gold++;
                        C.c.goldTexts[master.p].text = master.gold.ToString();
                    }
                    idle = true;
                }
            }
            
            if (rb.velocity.x < 0) spr.flipX = true;
            if (rb.velocity.x > 0) spr.flipX = false;
        } 
        else {
            if (possessProgress > 0) {
                spr.transform.localPosition = new Vector3(Random.Range(-.05f, .05f), Random.Range(-.05f, .05f));
                possessProgress -= Time.deltaTime;
            }
        }
    }

    public void controlStartStop(bool start) {
        controlling = start;
        transform.GetChild(2 + master.p).gameObject.SetActive(start);
    }
	
	IEnumerator Idle() {
        while (true) {
            if (idle) {
                if (Random.value < .25f) {
                    rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    if (rb.velocity.x < 0) spr.flipX = true;
                    if (rb.velocity.x > 0) spr.flipX = false;
                }
            }
            yield return new WaitForSeconds(.5f + Random.value * .5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Husk")) {
            if (type == 0) rb.velocity += Vector2.up * Random.Range(-5, 5);
        } else if (collision.transform.CompareTag("Gold")) {
            if (master != null) {
                if (controlling) {
                    if (!holdingGold) {
                        holdingGold = true;
                        transform.GetChild(1).gameObject.SetActive(true);
                        controlStartStop(false);
                        master.controllingList.Remove(this);
                    }
                }
            }
        }
    }

}
