using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Husk : MonoBehaviour {

    Rigidbody2D rb;
    internal SpriteRenderer spr;
    internal bool idle = true;
    public Player master;
    public int type = 0; //0 human, 1 p1, 2 p2
    internal bool controlling;
    internal float possessProgress;
    internal bool holdingGold;
    internal Husk enemyTarget;
    internal Transform enemyTower;
    float attackTimer;
    public float attackingTimer;
    float hit;
    float hp = 10;

	void Awake () {
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        StartCoroutine(Idle());
        StartCoroutine(SlowUpdate());
	}

    private void Update() {
        if (master != null) {
            if (master.towerDestroyed) { Instantiate(C.c.prefabs[2], transform.position, Quaternion.identity); Destroy(gameObject); return; }
            if (hit > 0) { hit -= Time.deltaTime; if (hit <= 0) spr.sprite = C.c.sprites[master.p + 1]; }
            else if (enemyTarget != null || enemyTower != null) {
                if (attackingTimer > 0) attackingTimer -= Time.deltaTime;
                if (attackTimer <= 0) { //attack
                    C.am.PlaySound(6);
                    if (enemyTarget != null) rb.velocity = (enemyTarget.transform.position - transform.position).normalized * 6;
                    else rb.velocity = (enemyTower.transform.position - transform.position).normalized * 6;
                    attackTimer = 1f + Random.Range(0,.5f);
                    attackingTimer = .5f;
                } else {
                    attackTimer -= Time.deltaTime;
                }
            }
            else if (controlling) {
                rb.velocity += master.lStickAxis;
                if (rb.velocity.magnitude > 3) rb.velocity = rb.velocity.normalized * 3;
            } else {
                if (Vector3.Distance(transform.position, master.tower.position) > 2) {
                    idle = false;
                    if (rb.velocity.magnitude < .5f) rb.velocity += Vector2.down * 7;
                    rb.velocity += (Vector2)(master.tower.position - transform.position).normalized;
                    if (rb.velocity.magnitude > 3) rb.velocity = rb.velocity.normalized * 3;
                } else {
                    if (holdingGold) { //store gold
                        C.am.PlaySound(4);
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
                transform.GetChild(4).GetChild(0).localScale = new Vector3(possessProgress / 5, 1);
                spr.transform.localPosition = new Vector3(Random.Range(-.05f, .05f), Random.Range(-.05f, .05f));
                possessProgress -= Time.deltaTime;
                if (possessProgress <= 0) {
                    spr.transform.localPosition = Vector3.zero;
                    transform.GetChild(4).gameObject.SetActive(false);
                }
            }
        }
    }

    public void controlStartStop(bool start) {
        controlling = start;
        transform.GetChild(2 + master.p).gameObject.SetActive(start);
    }

    public void Possess(int p) {
        master = C.c.players[p];
        idle = false;
        spr.sprite = C.c.sprites[1 + p];
        type = p + 1;
        spr.transform.localPosition = Vector3.zero;
        var comp = gameObject.AddComponent<CircleCollider2D>();
        comp.isTrigger = true;
        comp.radius = .7f;
        C.c.players[p].husks.Add(this);
        transform.GetChild(4).gameObject.SetActive(false);
    }

    IEnumerator SlowUpdate() {
        while (true) {
            var pos = transform.position;
            pos.z = 1 + pos.y * .01f;
            transform.position = pos;
            yield return new WaitForSeconds(.25f);
        }
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
            else {
                if (collision.transform.GetComponent<Husk>().type != 0 && collision.transform.GetComponent<Husk>().type != type) {
                    if (collision.transform.GetComponent<Rigidbody2D>().velocity.magnitude > rb.velocity.magnitude) {
                        hit = .25f;
                        hp--;
                        spr.sprite = C.c.sprites[3];
                        if (hp <= 0) {
                            Destroy(gameObject);
                            C.am.PlaySound(8);
                        } else C.am.PlaySound(7);
                    }
                }
            }
        } else if (collision.transform.CompareTag("Gold")) {
            if (master != null) {
                if (controlling) {
                    if (!holdingGold) {
                        C.am.PlaySound(3);
                        holdingGold = true;
                        transform.GetChild(1).gameObject.SetActive(true);
                        controlStartStop(false);
                        master.controllingList.Remove(this);
                        collision.transform.localScale *= .99f;
                        if (collision.transform.localScale.x < 2f) Destroy(collision.gameObject);
                    }
                }
            }
        } else if (collision.transform.CompareTag("Tower")) {
            if (type != 0) {
                if (enemyTower == collision.transform) {
                    if (rb.velocity.magnitude > 1 && enemyTarget == null) {
                        C.c.players[(type == 1 ? 1 : 0)].towerHp--;
                        C.am.PlaySound(9);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.CompareTag("Husk")) {
            if (type > 0 && collision.GetComponent<Husk>().type > 0 && collision.GetComponent<Husk>().type != type) {
                enemyTarget = collision.GetComponent<Husk>();
            }
        } else if (collision.transform.CompareTag("Tower")) {
            if (type != 0) {
                if (collision.transform == C.c.towers[(type == 1 ? 1 : 0)]) {
                    enemyTower = collision.transform;
                }
            }
        }
    }

}
