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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SwingLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) SwingRight();
    }

    void SwingLeft()
    {
        if (MonkeyStates.currentState == MonkeyIs.HOLDING_VINE)
        {
            print("monkey should swing left");

            rb = GetComponentInParent<Rigidbody2D>();

            //rb.MovePosition(transform.position += (Vector3.left * swingForce));
            rb.AddForce(Vector2.left * swingForce);
        }
    }

    void SwingRight()
    {
        if (MonkeyStates.currentState == MonkeyIs.HOLDING_VINE)
        {
            print("monkey should swing right");

            rb = GetComponentInParent<Rigidbody2D>();

            //rb.MovePosition(transform.position += (Vector3.right * swingForce));
            rb.AddForce(Vector2.right * swingForce);
        }
    }


}
