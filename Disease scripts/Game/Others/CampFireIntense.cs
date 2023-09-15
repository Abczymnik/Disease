using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CampFireIntense : MonoBehaviour
{
    public GameObject fire;
    private HDAdditionalLightData lightData;

    void Start()
    {
        lightData = fire.GetComponent<HDAdditionalLightData>();
    }
    // Update is called once per frame
    void Update()
    {
        lightData.intensity = 12 + Mathf.PingPong(Time.time * 8, 4);
    }
}
