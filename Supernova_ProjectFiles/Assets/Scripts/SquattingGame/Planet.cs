using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour
{
    [SerializeField]
    float minSpeed = .1f, maxSpeed = 1, rearZDist = -10, frontZDist = 500, slowDownFactor = 8;

    [SerializeField]
    Transform[] planetTransforms;

    Vector3[] startPositions;

    Rigidbody rb;
    Vector3 randomRotation;
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

        randomRotation = new Vector3
            (Random.Range(0, 2) * Random.Range(0, 50),
            Random.Range(0, 2) * Random.Range(0, 50),
            Random.Range(0, 2) * Random.Range(0, 50));

        startPositions = new Vector3[planetTransforms.Length];

        for (int i = 0; i < planetTransforms.Length; i++)
            startPositions[i] = planetTransforms[i].position;
    }

    void Update()
    {
        if (transform.position.z < rearZDist)
        {
            Vector3 newPos = startPositions[Random.Range(0, startPositions.Length)];
            transform.position = new Vector3(newPos.x, newPos.y, frontZDist);
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
