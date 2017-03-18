using UnityEngine;
using System.Collections;

public class FlyThroughSpace : MonoBehaviour
{
    [SerializeField]
    float minSpeed = .01f, maxSpeed = .5f, rearZDist = -10, frontZDist = 100;

    float speed;

    Vector3 startPos;

    void Start()
    {
        speed = Random.Range(minSpeed * 100, maxSpeed * 100) / 100;
        startPos = transform.position;
    }

    void Update()
    {
        if (transform.position.z < rearZDist)
        {
            transform.position = new Vector3(startPos.x, startPos.y, frontZDist);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        transform.position -= transform.forward * speed;
    }
}
