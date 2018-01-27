using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C : MonoBehaviour {

    public static C c;
    public int size = 32;
    public GameObject[] prefabs;

	// Use this for initialization
	void Start () {
        c = this;

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
	
	
}
