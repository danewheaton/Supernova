using UnityEngine;
using System.Collections;

public class MusicDestroyer : MonoBehaviour
{
	void Start ()
    {
        if (FindObjectOfType<DontDestroyOnLoad>() != null) Destroy(FindObjectOfType<DontDestroyOnLoad>().gameObject);
	}
}
