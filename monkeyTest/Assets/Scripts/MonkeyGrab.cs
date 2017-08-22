using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyGrab : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Vine" && MonkeyStates.currentState == MonkeyIs.JUMPING)
        {
            //print("grabbed vine");
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            transform.SetParent(other.transform);
            transform.position = other.transform.position;

            MonkeyStates.currentState = MonkeyIs.HOLDING_VINE;
        }
    }
}
