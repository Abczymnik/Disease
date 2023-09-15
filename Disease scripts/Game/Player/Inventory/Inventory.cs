using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private List<Item> characterItems = new List<Item>();
    private UIInventory inventoryUI;
    private Transform inventoryPanel;
    private bool inventoryEnabled;

    private InputAction inventorySwitch;
    public int NotesOnMap { get; set; }
    public ItemDatabase ItemDatabase { get; private set; }

    private int _ownedNotes;
    public int OwnedNotes
    {
        get { return _ownedNotes; }
        private set
        {
            if (value < _ownedNotes) { NotesOnMap--; }
            _ownedNotes = value;
        }
    }

    private void Awake()
    {
        inventorySwitch = PlayerUI.inputActions.Gameplay.Inventory;
        inventorySwitch.performed += InventoryPerformed;
    }

    private void OnDisable()
    {
        inventorySwitch.performed -= InventoryPerformed;
    }

    private void Start()
    {
        ItemDatabase = GameObject.Find("ItemDatabase").GetComponent<ItemDatabase>();
        inventoryUI = GetComponent<UIInventory>();
        inventoryPanel = transform.GetChild(0);

        GiveItem(0);
        GiveItem(1);
        GiveItem(2);
        NotesOnMap += 3;
    }

    private void InventoryPerformed(InputAction.CallbackContext context)
    {
        InventoryVisibility();
    }

    public void GiveItem()
    {
        Item itemToAdd = ItemDatabase.GetItem(OwnedNotes);
        characterItems.Add(itemToAdd);
        inventoryUI.AddNewItem(itemToAdd);
        OwnedNotes++;
    }

    public void GiveItem(int id)
    {
        Item itemToAdd = ItemDatabase.GetItem(id);
        characterItems.Add(itemToAdd);
        inventoryUI.AddNewItem(itemToAdd);
        OwnedNotes++;
    }

    public void GiveItem(string itemName)
    {
        Item itemToAdd = ItemDatabase.GetItem(itemName);
        characterItems.Add(itemToAdd);
        inventoryUI.AddNewItem(itemToAdd);
        OwnedNotes++;
    }

    public Item CheckForItem(int id)
    {
        return characterItems.Find(item => item.Id == id);
    }

    public void RemoveItem(int id)
    {
        Item itemToRemove = CheckForItem(id);
        if (itemToRemove != null)
        {
            characterItems.Remove(itemToRemove);
            inventoryUI.RemoveItem(itemToRemove);
            OwnedNotes--;
        }
    }

    //Enable/disable Inventory w/o tooltip component
    private void InventoryVisibility()
    {
        inventoryEnabled = !inventoryEnabled;
        inventoryPanel.GetComponent<Image>().enabled = inventoryEnabled; //Enable/disable inventory panel
        inventoryPanel.GetChild(0).gameObject.SetActive(inventoryEnabled); //Enable/disable slots grid
        if (inventoryEnabled) { CursorSwitch.SwitchSkin("note"); }
        else
        {
            UIHelper.EnableGUI();
            CursorSwitch.SwitchSkin("standard");
            inventoryPanel.GetChild(1).gameObject.SetActive(inventoryEnabled); //Disable tooltip
        }
    }
}
