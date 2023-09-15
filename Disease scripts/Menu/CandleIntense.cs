using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CandleIntense : MonoBehaviour
{
    private HDAdditionalLightData lightData;

    private void Start()
    {
        lightData = GetComponent<HDAdditionalLightData>();
    }
    
    void Update()
    {
        lightData.intensity = 20 + Mathf.PingPong(Time.time*100, 10);
    }
}
