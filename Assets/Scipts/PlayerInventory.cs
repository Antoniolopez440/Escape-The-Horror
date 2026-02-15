using UnityEngine;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

public class PlayerInventory : MonoBehaviour
{

    public static PlayerInventory Instance { get; private set; }

    private readonly List<string> items = new List<string>();

    public int SelectedIndex { get; private set; } = -1;

        public event Action InventoryChanged;
    public Action<string> SelectedItemChanged;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
    }

    public void AddItems(string Id)
    {

        if (string.IsNullOrEmpty(Id))
            return;

        if (!items.Contains(Id))
        {
            items.Add(Id);
            InventoryChanged?.Invoke();

            if (SelectedIndex < 0)
                SelectIndex(0);
        }
    }

    public void RemoveItems(string Id)
    {
        if (items.Remove(Id))
        {
            InventoryChanged?.Invoke();
            if (SelectedIndex >= items.Count)
                SelectedIndex = items.Count - 1;
        }
    }

    public string GetSelectedItem()
    {
        if (SelectedIndex < 0 || SelectedIndex >= items.Count)
            return null;
        return items[SelectedIndex];
    }

    public void SelectIndex(int index)
    {
        if (items.Count == 0) { SelectedIndex = -1;
            SelectedItemChanged?.Invoke(null);
            return;

            index = Mathf.Clamp(index, 0, items.Count - 1);
            if (SelectedIndex == index) return;
            SelectedItemChanged?.Invoke(GetSelectedItem());
        }
    }


    public IReadOnlyList<string> GetItems()
    {
        return items;
    }


    public bool HasKey(string Id)
    {
        if (string.IsNullOrEmpty(Id)) return false;
        return items.Contains(Id);
    }
}
