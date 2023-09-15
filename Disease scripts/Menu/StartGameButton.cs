using UnityEngine;
using UnityEngine.EventSystems;

public class StartGameButton : MonoBehaviour, IPointerClickHandler
{
    private SceneLoader sceneLoader;

    private void Awake()
    {
        sceneLoader = GameObject.Find("Level Loader").GetComponent<SceneLoader>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        sceneLoader.LoadNextLevel();
    }
}
