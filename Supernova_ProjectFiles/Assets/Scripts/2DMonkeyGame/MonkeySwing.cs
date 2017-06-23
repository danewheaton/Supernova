using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeySwing : MonoBehaviour
{
    [SerializeField]
    float swingForce = 50;

    Rigidbody2D rb;

    void OnEnable()
    {
        GestureManager.OnLeanLeftDetected += SwingLeft;
        GestureManager.OnLeanRightDetected += SwingRight;
    }
    void OnDisable()
    {
        GestureManager.OnLeanLeftDetected -= SwingLeft;
        GestureManager.OnLeanRightDetected -= SwingRight;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void SwingLeft()
    {
        rb.AddForce(Vector2.left * swingForce);
    }

    void SwingRight()
    {
        rb.AddForce(Vector2.right * swingForce);
    }
}
