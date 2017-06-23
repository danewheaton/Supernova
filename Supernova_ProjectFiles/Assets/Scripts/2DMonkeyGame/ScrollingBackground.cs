using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField]
    float speed = 2, leftEndpoint = -20.47f, rightEndpoint = 32.69f;

    void Update()
    {
        transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        if (transform.position.x < leftEndpoint) transform.position = new Vector3(rightEndpoint, transform.position.y, transform.position.z);
    }
}
