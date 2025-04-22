using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampGlow : MonoBehaviour
{
    private Light[] allLights;
    private float[] defaultIntensities;
    public float glowIntensity = 20f;
    public float glowDuration = 3f;

    private float timer;

    void Start()
    {
        // Get all Light components in this prefab's children
        allLights = GetComponentsInChildren<Light>();

        // Save their default intensities
        defaultIntensities = new float[allLights.Length];
        for (int i = 0; i < allLights.Length; i++)
        {
            defaultIntensities[i] = allLights[i].intensity;
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // Reset intensities
                for (int i = 0; i < allLights.Length; i++)
                {
                    allLights[i].intensity = defaultIntensities[i];
                }
            }
        }
    }

    public void Glow()
    {
        for (int i = 0; i < allLights.Length; i++)
        {
            allLights[i].intensity = glowIntensity;
        }
        timer = glowDuration;
    }
}
