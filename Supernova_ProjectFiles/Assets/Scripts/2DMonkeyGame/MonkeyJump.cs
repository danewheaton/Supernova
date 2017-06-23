using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 500;

    Rigidbody2D rb;

    void OnEnable()
    {
        GestureManager.OnFlickDetected += Jump;
    }
    void OnDisable()
    {
        GestureManager.OnFlickDetected -= Jump;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Jump()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.AddForce(Vector2.up * jumpForce);
        rb.AddForce(Vector2.right * jumpForce);
    }
}
