using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gunDisplayUI : MonoBehaviour
{
    [SerializeField] public Image icon;
    [SerializeField] public Image highLight;
    [SerializeField] public TMP_Text ammoText;

    ProjectileGun gun;

    public void SetGun(ProjectileGun gun, bool isSelected)
    {
        this.gun = gun;

        icon.sprite = gun.gunModelSprite;
        highLight.enabled = isSelected;

        RefreshAmmo();
    }

    public void RefreshAmmo()
    {
        if (gun == null) return;

        ammoText.text = $"{gun.bulletsLeft} / {gun.magazineSize}";
    }

}
