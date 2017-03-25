using UnityEngine;
using System.Collections;

public class SquattableZone : MonoBehaviour
{
    PlanetSquat planetSquat;

    void Start()
    {
        planetSquat = GetComponentInParent<PlanetSquat>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Planet") planetSquat.planetsInTrigger.Add(other.gameObject.GetComponent<Rigidbody>());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Planet") planetSquat.planetsInTrigger.Remove(other.gameObject.GetComponent<Rigidbody>());
    }
}
