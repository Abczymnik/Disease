using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.HighDefinition;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
    private GameObject loadingBar;
    private Slider loadingSlider;
    private List<CanvasGroup> buttonsAlpha = new List<CanvasGroup>();
    private List<HDAdditionalLightData> lightsData = new List<HDAdditionalLightData>();
    private List<float> lightIntensities = new List<float>();

    private GameObject candleLight;
    private SpecialZombie freeZombie;

    private Volume skyLightIntense;

    private void Awake()
    {
        loadingBar = GameObject.Find("/Menu").transform.GetChild(0).gameObject;
        candleLight = GameObject.Find("/Cursor Lantern/Candle/Point light");
        loadingSlider = loadingBar.transform.GetChild(1).GetComponent<Slider>();
        skyLightIntense = GameObject.Find("/Settings/Dim Sky").GetComponent<Volume>();
        GetLights();
        TurnOffLights();
    }

    private void Start()
    {
        freeZombie = GameObject.Find("Zombie/Special Zombie").GetComponent<SpecialZombie>();
        GetButtons();
        StartCoroutine(LoadScene());
    }
    
    //Turn off scene lights
    private void TurnOffLights()
    {
        foreach(HDAdditionalLightData light in lightsData)
        {
            light.intensity = 0f;
        }
    }

    //Load next scene
    public void LoadNextLevel()
    {
        freeZombie.LetZombieFree();
        OffCandleBlinking();
        StartCoroutine(LoadNextScene());
    }

    //Get lights from scene
    private void GetLights()
    {
        GameObject[] lightsObj = GameObject.FindGameObjectsWithTag("Light");
        foreach(GameObject light in lightsObj)
        {
            HDAdditionalLightData lightAddData = light.GetComponent<HDAdditionalLightData>();
            lightsData.Add(lightAddData);
            lightIntensities.Add(lightAddData.intensity);
        }
    }

    //Get buttons from scene
    private void GetButtons()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Menu Buttons");
        foreach(GameObject button in buttons)
        {
            buttonsAlpha.Add(button.GetComponent<CanvasGroup>());
        }
    }

    //Smooth load this scene view
    IEnumerator LoadScene()
    {
        float timer = 0;
        float speed = 3f;

        while (timer < speed)
        {
            timer += Time.deltaTime;

            foreach (CanvasGroup button in buttonsAlpha)
            {
                button.alpha = Mathf.Lerp(0, 1, timer / speed);
            }

            for (int i = 0; i < lightsData.Count; i++)
            {
                lightsData[i].intensity = Mathf.Lerp(0, lightIntensities[i], timer / speed);
            }

            skyLightIntense.weight = Mathf.Lerp(1, 0, timer / speed);

            yield return null;
        }
    }

    //Dim light emitters and buttons in 3 sec and then load intro
    IEnumerator LoadNextScene()
    {
        InputSystem.DisableDevice(Mouse.current);
        loadingBar.SetActive(true);
        float timer = 0;

        while (timer < 3f)
        {
            timer += Time.deltaTime;
            loadingSlider.value = timer;

            foreach (CanvasGroup button in buttonsAlpha)
            {
                button.alpha = Mathf.Lerp(1, 0, timer / 3);
            }

            for (int i = 0; i < lightsData.Count; i++)
            {
                lightsData[i].intensity = Mathf.Lerp(lightIntensities[i], 0, timer / 3);
            }

            skyLightIntense.weight = Mathf.Lerp(0, 1, timer / 3);

            yield return null;
        }
        InputSystem.EnableDevice(Mouse.current);
        SceneManager.LoadScene(1);
    }

    //Disable Lantern candle blinking script
    private void OffCandleBlinking()
    {
        candleLight.GetComponent<CandleIntense>().enabled = false;
    }
}
