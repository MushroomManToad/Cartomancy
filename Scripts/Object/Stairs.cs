using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public int bottomLayer, topLayer;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerController controller = collider.GetComponent<PlayerController>();
        if(controller != null)
        {
            controller.setLayer(topLayer);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        PlayerController controller = collider.GetComponent<PlayerController>();
        if (controller != null)
        {
            if(controller.getMoving(Facing.SOUTH))
            {
                controller.setLayer(bottomLayer);
            }
        }
    }
}
