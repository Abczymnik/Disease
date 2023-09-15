using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExperienceInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Experience experience;
    private TMPro.TMP_Text myText;
    private Canvas thisCanvas;
    private Coroutine refreshExp;

    private void Awake()
    {
        myText = transform.GetChild(0).GetComponentInChildren<TMPro.TMP_Text>();
        thisCanvas = transform.GetChild(0).GetComponent<Canvas>();
    }

    private void Start()
    {
        experience = GetComponentInParent<Experience>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        thisCanvas.enabled = true;
        refreshExp = StartCoroutine(RefreshExp());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        thisCanvas.enabled = false;
        StopCoroutine(refreshExp);
    }

    IEnumerator RefreshExp()
    {
        while(true)
        {
            myText.text = ((int)experience.CurrentExp).ToString() + "/" + ((int)experience.MaxExp).ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
