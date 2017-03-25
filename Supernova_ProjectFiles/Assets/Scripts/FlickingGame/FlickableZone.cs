using UnityEngine;
using System.Collections;

public class FlickableZone : MonoBehaviour
{
    AsteroidFlick asteroidFlick;

    void Start()
    {
        asteroidFlick = GetComponentInParent<AsteroidFlick>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Asteroid") asteroidFlick.asteroidsInTrigger.Add(other.gameObject.GetComponent<Rigidbody>());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Asteroid") asteroidFlick.asteroidsInTrigger.Remove(other.gameObject.GetComponent<Rigidbody>());
    }
}
