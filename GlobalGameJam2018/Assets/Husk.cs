using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Husk : MonoBehaviour {

    Rigidbody2D rb;
    SpriteRenderer spr;
    internal bool idle = true;
    internal Player master;

	void Start () {
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        StartCoroutine(Idle());
	}

    private void Update() {
        if (master != null) {
            rb.velocity = master.transform.position - transform.position;
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

}
