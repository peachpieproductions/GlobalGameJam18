using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sound {
    public AudioClip audioClip;
    public float vol;
    public bool pitchChange;
}

public class AudioManager : MonoBehaviour {

    public Sound[] snds;
    internal AudioSource AS;

	void Start () {
        AS = gameObject.AddComponent<AudioSource>();
	}
	
	void Update () {
		
	}

    public void PlaySound(int index) {
        var clip = snds[index].audioClip;
        if (snds[index].pitchChange) AS.pitch = Random.Range(.8f, 1.2f);
        else AS.pitch = 0;
        AS.PlayOneShot(clip, snds[index].vol);
    }

}
