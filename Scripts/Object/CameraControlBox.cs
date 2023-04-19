using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlBox : MonoBehaviour
{
    [SerializeField]
    CameraController cam;

    public float worldMin, worldMax, worldLeft, worldRight;
    public bool lockX, lockY;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerController pc = collider.GetComponent<PlayerController>();
        if(pc != null)
        {
            if (!used)
            {
                cam.worldMin = worldMin;
                cam.worldMax = worldMax;
                cam.worldLeft = worldLeft;
                cam.worldRight = worldRight;
                used = true;
                StartCoroutine(locks());
            }
        }
    }

    private IEnumerator locks()
    {
        yield return new WaitForEndOfFrame();
        cam.lockX = lockX;
        cam.lockY = lockY;
        Destroy(gameObject);
    }
}
