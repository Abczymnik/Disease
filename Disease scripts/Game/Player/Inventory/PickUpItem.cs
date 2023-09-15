using UnityEngine;
using UnityEngine.EventSystems;

public class PickUpItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Inventory inventory;
    private Item item;
    private Tooltip toolTip;

    private void Start()
    {
        inventory = GameObject.Find("/Player/Inventory").GetComponent<Inventory>();
        item = inventory.ItemDatabase.GetItem(inventory.NotesOnMap-1);
        toolTip = GameObject.Find("/Player/Inventory/InventoryPanel/Tooltip").GetComponent<Tooltip>();
    }

    //Generate item tooltip while pointer over note
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            toolTip.GenerateTooltip(item);
        }

        CursorSwitch.SwitchSkin("note");
    }
    
    //Disable item tooltip on pointer exit
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        CursorSwitch.SwitchSkin("standard");
    }

    //Pick up note from Zombie and disable item tooltip
    public void OnPointerClick(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
        PickUp();
        CursorSwitch.SwitchSkin("standard");
    }

    //Add item to inventory and disable note object
    private void PickUp()
    {
        inventory.GiveItem(item.Id);
        transform.parent.gameObject.SetActive(false);
    }
}
