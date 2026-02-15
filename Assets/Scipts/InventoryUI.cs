using NUnit;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text slot1;
    [SerializeField] private TMP_Text slot2;
    [SerializeField] private TMP_Text slot3;

    private void OnEnable()
    {
        if (PlayerInventory.Instance != null) return;

        PlayerInventory.Instance.InventoryChanged += Refresh;
        PlayerInventory.Instance.SelectedItemChanged += _ => Refresh();

        Refresh();
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null) return;
        PlayerInventory.Instance.InventoryChanged -= Refresh;
        PlayerInventory.Instance.SelectedItemChanged -= _ => Refresh();
    }

    private void Refresh()
    {
        var inventory = PlayerInventory.Instance;
        var items = inventory.GetItems();

        SetSlot(slot1, items, 0, inventory.SelectedIndex);
        SetSlot(slot2, items, 1, inventory.SelectedIndex);
        SetSlot(slot3, items, 2, inventory.SelectedIndex);
    }

    private void SetSlot(TMP_Text t, System.Collections.Generic.IReadOnlyList<string> items, int index, int selected)
    {
        if (t == null) return;

        string name = (index < items.Count) ? items[index] : "---";
        bool isSelected = (index == selected);

        // Simple highlight
        t.text = isSelected ? $"> {index + 1}: {name}" : $"{index + 1}: {name}";
    }
}

