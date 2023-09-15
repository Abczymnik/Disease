using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingLayers : MonoBehaviour
{
    private void Start()
    {
        Camera camera = Camera.main;
        float[] distances = new float[32];
        distances[20] = 5f;
        camera.layerCullDistances = distances;
    }
}
