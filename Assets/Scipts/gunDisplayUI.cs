using UnityEngine;
using UnityEngine.UI;

public class gunDisplayUI : MonoBehaviour
{
    [SerializeField] public Image icon;
    [SerializeField] public Image highLight;

    public void SetGun(ProjectileGun gun, bool isSelected)
    {
        icon.sprite = gun.gunModelSprite;
        highLight.enabled = isSelected;
    }

}
