using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 500;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    void Jump()
    {
        if (MonkeyStates.currentState != MonkeyIs.JUMPING)
        {
            //print("monkey should jump");
            Collider[] vineColliders = GetComponentsInParent<Collider>();
            foreach (Collider c in vineColliders) c.enabled = false;
            transform.SetParent(null);
            rb.isKinematic = false;
            rb.AddForce(Vector2.up * jumpForce);
            rb.AddForce(Vector2.right * jumpForce);

            MonkeyStates.currentState = MonkeyIs.JUMPING;
        }
    }
}
