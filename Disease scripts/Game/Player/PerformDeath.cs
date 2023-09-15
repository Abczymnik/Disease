using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class PerformDeath : MonoBehaviour
{
    private List<HDAdditionalLightData> lightsData = new List<HDAdditionalLightData>();
    private List<float> lightIntensities = new List<float>();
    private List<CanvasGroup> GUICanvas = new List<CanvasGroup>();
    private Volume skyLightIntense;

    private void OnEnable()
    {
        skyLightIntense = GameObject.Find("/Settings/Dim Sky").GetComponent<Volume>();
        BackToMenu();
    }

    private void BackToMenu()
    {
        GetLights();
        GetGUICanvas();
        StartCoroutine(LoadMenu());
    }

    IEnumerator LoadMenu()
    {
        float timer = 0;
        float speed = 5f;

        while(timer < speed)
        {
            Debug.Log(timer);
            timer += Time.deltaTime;

            foreach(CanvasGroup GUI in GUICanvas)
            {
                GUI.alpha = Mathf.Lerp(1, 0, timer / speed);
            }

            for (int i = 0; i < lightsData.Count; i++)
            {
                lightsData[i].intensity = Mathf.Lerp(lightIntensities[i], 0, timer / speed);
            }

            skyLightIntense.weight = Mathf.Lerp(0, 1, timer / speed);

            yield return null;
        }
        SceneManager.LoadScene(0);
    }

    //Get Lights to dim over time
    private void GetLights()
    {
        GameObject[] lightsObj = GameObject.FindGameObjectsWithTag("Light");
        foreach (GameObject light in lightsObj)
        {
            HDAdditionalLightData lightAddData = light.GetComponent<HDAdditionalLightData>();
            lightsData.Add(lightAddData);
            lightIntensities.Add(lightAddData.intensity);
        }
    }

    //Get GUI Elements to fade over time
    private void GetGUICanvas()
    {
        GameObject[] GUIObj = GameObject.FindGameObjectsWithTag("GUI Element");
        foreach(GameObject GUI in GUIObj)
        {
            GUICanvas.Add(GUI.GetComponent<CanvasGroup>());
        }
    }
}
