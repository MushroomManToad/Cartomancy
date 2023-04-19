using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource source;

   void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "Title")
        {
            source.Stop();
        }
    }
}
