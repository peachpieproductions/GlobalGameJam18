using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Husk : MonoBehaviour {

    Rigidbody2D rb;
    internal SpriteRenderer spr;
    internal bool idle = true;
    internal Player master;

	void Awake () {
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        StartCoroutine(Idle());
	}

    private void Update() {
        if (master != null) {
            if (master.controllingHusks) {

            } else {
                if (Vector3.Distance(transform.position, master.tower.position) > 2) {
                    if (rb.velocity.magnitude < .5f) rb.velocity += Vector2.down * 7;
                    rb.velocity += (Vector2)((master.tower.position + Vector3.down * 2) - transform.position).normalized;
                    if (rb.velocity.magnitude > 3) rb.velocity = rb.velocity.normalized * 3;
                }
            }
            
            if (rb.velocity.x < 0) spr.flipX = true;
            if (rb.velocity.x > 0) spr.flipX = false;
        }
    }
	
	IEnumerator Idle() {
        while (idle) {
            if (Random.value < .25f) {
                rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                if (rb.velocity.x < 0) spr.flipX = true;
                if (rb.velocity.x > 0) spr.flipX = false;
            }
            yield return new WaitForSeconds(.5f + Random.value * .5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Husk")) {
            rb.velocity += Vector2.up * Random.Range(-5, 5);
        }
    }

}
