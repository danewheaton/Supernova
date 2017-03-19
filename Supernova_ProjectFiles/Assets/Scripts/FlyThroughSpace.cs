using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class FlyThroughSpace : MonoBehaviour
{
    [SerializeField]
    float minSpeed = .01f, maxSpeed = .5f, rearZDist = -10, frontZDist = 100;

    Rigidbody rb;
    Vector3 startPos, randomRotation;
    float speed;

    void Start()
    {
        speed = Random.Range(minSpeed * 100, maxSpeed * 100) / 100;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        startPos = transform.position;

        randomRotation = new Vector3
            (Random.Range(0, 2) * Random.Range(0, 50),
            Random.Range(0, 2) * Random.Range(0, 50),
            Random.Range(0, 2) * Random.Range(0, 50));
    }

    void Update()
    {
        if (transform.position.z < rearZDist)
        {
            transform.position = new Vector3(startPos.x, startPos.y, frontZDist);
            rb.velocity = Vector3.zero;
        }
        transform.position -= Vector3.forward * speed;
        transform.Rotate(randomRotation * Time.deltaTime);
    }
}
