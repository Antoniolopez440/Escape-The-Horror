using NUnit;
using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text slot1;
    [SerializeField] private TMP_Text slot2;
    [SerializeField] private TMP_Text slot3;

    private bool bound;

    private void OnEnable()
    {
        bound = false;
        StartCoroutine(BindWhenReady());
    }

    private IEnumerator BindWhenReady()
    {
        while (PlayerInventory.Instance == null)
            yield return null;

        if (bound) yield break;
        bound = true;


        PlayerInventory.Instance.InventoryChanged += Refresh;
        PlayerInventory.Instance.SelectedItemChanged += OnSelectedChanged;
        
        Refresh();
    }

    private void OnDisable()
    {
      if (!bound) return;
        if (PlayerInventory.Instance == null) return;
        
            PlayerInventory.Instance.InventoryChanged -= Refresh;
            PlayerInventory.Instance.SelectedItemChanged -= OnSelectedChanged;

        bound = false;

    }

    private void OnSelectedChanged(string id) => Refresh();

    private void Refresh()
    {
        var inventory = PlayerInventory.Instance;
     if (inventory == null) return;

     var items = inventory.GetItems();

        SetSlot(slot1, items, 0, inventory.SelectedIndex);
        SetSlot(slot2, items, 1, inventory.SelectedIndex);
        SetSlot(slot3, items, 2, inventory.SelectedIndex);
    }

    private void SetSlot(TMP_Text t, System.Collections.Generic.IReadOnlyList<string> items, int index, int selected)
    {
        if (t == null) return;

        bool hasItem = (index < items.Count);
        string name = hasItem ? items[index] : "---";
        bool isSelected = (index == selected);

        // Clean look: bullet for selected, indent for others. No numbers, no arrows.
        string prefix = isSelected ? "• " : "  ";
        t.text = prefix + name;
    }
}

