using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Rigidbody2D rb;
    SpriteRenderer spr;
    float speed = 1;
    List<Husk> nearbyHusks = new List<Husk>();
    public Transform tower;
    public bool controllingHusks;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        if (nearbyHusks.Count != 0) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                nearbyHusks[0].master = this;
                nearbyHusks[0].idle = false;
                nearbyHusks[0].spr.sprite = C.c.sprites[1];
                nearbyHusks.RemoveAt(0);
            }
        }

		if (Input.GetKey(KeyCode.W)) {
            rb.velocity += Vector2.up * speed;
        }
        if (Input.GetKey(KeyCode.S)) {
            rb.velocity += Vector2.down * speed;
        }
        if (Input.GetKey(KeyCode.A)) {
            rb.velocity += Vector2.left * speed;
        }
        if (Input.GetKey(KeyCode.D)) {
            rb.velocity += Vector2.right * speed;
        }
        if (rb.velocity.magnitude > 3) rb.velocity = rb.velocity.normalized * 3;

        if (rb.velocity.x < 0) spr.flipX = true;
        if (rb.velocity.x > 0) spr.flipX = false;

        rb.velocity *= .9f;
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Husk")) {
            if (collision.GetComponent<Husk>().idle)
                nearbyHusks.Add(collision.GetComponent<Husk>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Husk")) {
            if (collision.GetComponent<Husk>().idle)
                nearbyHusks.Remove(collision.GetComponent<Husk>());
        }
    }

}
