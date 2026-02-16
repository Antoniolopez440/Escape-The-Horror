using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class gunDisplayManagerUI : MonoBehaviour
{
    [SerializeField] public playerControllerNew player;
    [SerializeField] public gunDisplayUI gunIconPrefab;
    [SerializeField] public Transform iconParent;

    public List<gunDisplayUI> icons = new List<gunDisplayUI>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildUI();
        RefreshSelection();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildUI()
    {
        foreach (Transform child in iconParent)
            Destroy(child.gameObject);

        icons.Clear();

        var guns = player.GetGunList();

        for (int i = 0; i < guns.Count; i++)
        {
            gunDisplayUI icon = Instantiate(gunIconPrefab, iconParent);
            icon.SetGun(guns[i], false);
            icons.Add(icon);
        }
    }

    public void RefreshSelection()
    {
        int selected = player.GetCurrentGunIndex();

        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].SetGun(player.GetGunList()[i], i == selected);

        }

    }
}
