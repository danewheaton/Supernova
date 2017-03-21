using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class FlyThroughSpace : MonoBehaviour
{
    [SerializeField]
    float minSpeed = .01f, maxSpeed = .5f, rearZDist = -10, frontZDist = 100, slowDownFactor = 8;

    Rigidbody rb;
    Vector3 startPos, randomRotation;
    float speed, startSpeed;
    bool canSlow = true;

    void OnEnable()
    {
        //GestureManager.OnPushDetected += SlowDown;
    }
    void OnDisable()
    {
        //GestureManager.OnPushDetected -= SlowDown;
    }

    void Start()
    {
        speed = Random.Range(minSpeed * 100, maxSpeed * 100) / 100;
        startSpeed = speed;
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

    void SlowDown()
    {
        if (canSlow) StartCoroutine(LerpSpeed());
    }

    IEnumerator LerpSpeed()
    {
        canSlow = false;

        float timer = 2;
        float elapsedTime = 0;
        while (elapsedTime < timer)
        {
            speed = Mathf.Lerp(startSpeed, startSpeed / slowDownFactor, elapsedTime / timer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        speed = startSpeed / slowDownFactor;

        elapsedTime = 0;
        while (elapsedTime < timer)
        {
            speed = Mathf.Lerp(startSpeed / slowDownFactor, startSpeed, elapsedTime / timer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        speed = startSpeed;

        canSlow = true;
    }
}
