using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyCam : MonoBehaviour
{
    [SerializeField]
    Transform monkey;

    [SerializeField] float speed = 2, xOffset = 6.877117f;

    void Update()
    {
        Vector3 targetPos = new Vector3(monkey.transform.position.x + xOffset, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
    }
}
