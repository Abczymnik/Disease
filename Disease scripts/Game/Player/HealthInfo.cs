using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HealthInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private HealthOrb healthOrb;
    private TMPro.TMP_Text myText;
    private Canvas thisCanvas;
    private Coroutine refreshHealth;

    private void Awake()
    {
        myText = transform.GetChild(0).GetComponentInChildren<TMPro.TMP_Text>();
        thisCanvas = transform.GetChild(0).GetComponent<Canvas>();
    }

    private void Start()
    {
        healthOrb = GetComponentInParent<HealthOrb>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        thisCanvas.enabled = true;
        refreshHealth = StartCoroutine(RefreshHealth());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(refreshHealth);
        thisCanvas.enabled = false;
    }

    IEnumerator RefreshHealth()
    {
        while(true)
        {
            myText.text = ((int)healthOrb.CurrentHealth).ToString() + "/" + ((int)healthOrb.MaxHealth).ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
