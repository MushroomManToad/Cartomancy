using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransferBox : MonoBehaviour
{
    [SerializeField]
    private string destScene;

    [SerializeField]
    private float targetX, targetY;

    [SerializeField]
    private int targetLayer;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collider was a player.
        PlayerController controller = collider.GetComponent<PlayerController>();
        PlayerStats stats = collider.GetComponent<PlayerStats>();
        if(controller != null && stats != null)
        {
            // Freeze the player
            controller.setFreezeInputs(true);

            // Start the fadeout Animation.
            controller.setFadeOut(true);

            // Load the next scene.
            SceneTransferManager.Instance.loadScene(destScene, targetX, targetY, targetLayer, stats);
        }
    }
}
