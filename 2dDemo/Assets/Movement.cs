using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float speed = 3;
    Rigidbody2D rb;
    float xDir;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        xDir = Input.GetAxis("Horizontal");
        rb.MovePosition(transform.position + new Vector3(xDir * speed, transform.position.y, transform.position.z));
	}
}
