using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickerLight : MonoBehaviour
{
    // Can't call this "light" because it hides a parent class name.
    public Light2D lamp;
    private float initialIntensity = 0;

    private void Start()
    {
        initialIntensity = lamp.intensity;
    }

    private void Update()
    {
        // Has a 1/100 chance to flicker each frame.
        if(Random.value < 0.01)
        {
            lamp.intensity = (Random.value > 0.5f ? 1 : -1) * Random.value * (initialIntensity / 10.0f) + initialIntensity;
        }
    }
}
