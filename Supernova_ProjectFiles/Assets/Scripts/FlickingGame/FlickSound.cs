using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class FlickSound : MonoBehaviour
{
    [SerializeField] AudioClip[] whooshes;

    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    public void PlaySound()
    {
        source.clip = whooshes[Random.Range(0, whooshes.Length)];
        source.Play();
    }
}
