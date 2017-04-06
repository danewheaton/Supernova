using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour
{
	void Awake()
    {
        DontDestroyOnLoad(gameObject);
	}
}
