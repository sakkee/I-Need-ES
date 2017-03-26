using UnityEngine;
using System.Collections;

public class ImpactSound : MonoBehaviour {
    public AudioClip impactSound;
    private AudioSource audioSource;
    private bool playing = false;
    void OnCollisionEnter(Collision hit)
    {
        
        if (!audioSource.isPlaying)
            playing = false;
        if (playing)
            return;
        //if (hit.collider.tag == "Player")
        //    return;
        if (hit.relativeVelocity.magnitude >= 0.5f)
        {
            audioSource.PlayOneShot(impactSound);
            playing = true;
            audioSource.volume = Mathf.Min(hit.relativeVelocity.magnitude / 5f, 1f);
        }
    }
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	void OnCollisionStay(Collision hit)
    {
        if (!audioSource.isPlaying)
            playing = false;
        if (playing)
            return;
        if (hit.relativeVelocity.magnitude>= 0.5f)
        {
            audioSource.PlayOneShot(impactSound);
            playing = true;
            audioSource.volume = Mathf.Min(hit.relativeVelocity.magnitude / 5f, 1f);
        }
    }
}
