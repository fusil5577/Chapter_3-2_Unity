using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public Transform slotPanel;

    [Header("Selected Item")]
    public ItemSlot selectedItem;
    private int selectedItemIndex;

    private PlayerCondition condition;
    private PlayerController controller;

    void Start()
    {
        condition = CharacterManager.Instance.Player.condition;
        controller = CharacterManager.Instance.Player.controller;

        CharacterManager.Instance.Player.addItem += AddItem;

        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    public void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        CharacterManager.Instance.Player.itemData = null;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;
    }

    void ClearSelectedItemWindow()
    {
        selectedItem = null;
    }

    public void OnItemUse(InputAction.CallbackContext context)
    {
        var control = context.control;

        if (context.phase == InputActionPhase.Started)
        {
            if (control.name == "1")
            {
                SelectItem(0);
            }
            else if (control.name == "2")
            {
                SelectItem(1);
            }

            if (selectedItem.item.type == ItemType.Consumable)
            {
                for (int i = 0; i < selectedItem.item.consumables.Length; i++)
                {
                    switch (selectedItem.item.consumables[i].type)
                    {
                        case ConsumableType.Health:
                            condition.Heal(selectedItem.item.consumables[i].value); break;
                        case ConsumableType.Speed:
                            controller.IncreaseSpeed(selectedItem.item.consumables[i].value); break;
                        case ConsumableType.Power:
                            controller.IncreasePower(selectedItem.item.consumables[i].value); break;
                        case ConsumableType.Double:
                            controller.OnItemCollected(); break;
                    }
                }
                RemoveSelctedItem();
            }
        }
    }

    void RemoveSelctedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public bool HasItem(ItemData item, int quantity)
    {
        return false;
    }
}