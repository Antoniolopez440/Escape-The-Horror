using UnityEngine;

public class EquippedItemHnadler : MonoBehaviour
{

    [SerializeField] private Transform handItemRoots;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerInventory.Instance != null)
      PlayerInventory.Instance.SelectedItemChanged += OnSelectedItemChanged;

        Equip(null);
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.SelectedItemChanged -= OnSelectedItemChanged;
    }

    private void OnSelectedItemChanged(string id) => Equip(id);

    private void Equip(string itemId)
    {
        if (handItemRoots == null)
            return;

        for (int i = 0; i < handItemRoots.childCount; i++)
        handItemRoots.GetChild(i).gameObject.SetActive(false);

        if (string.IsNullOrEmpty(itemId))
            return;

        Transform t = handItemRoots.Find(itemId);
        if (t != null)
            t.gameObject.SetActive(true);


    }


}
