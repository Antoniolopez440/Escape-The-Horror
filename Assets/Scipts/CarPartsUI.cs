using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;


public class CarPartsUI : MonoBehaviour
{
    public Transform container;
    public GameObject iconPrefab;

    public void Refresh(List<CarPart> parts)
    {
        foreach (Transform c in container)
            Destroy(c.gameObject);

        foreach (var part in parts)
        {
            Image img = Instantiate(iconPrefab, container).GetComponent<Image>();
            img.sprite = part.uIIcon;
        }
    }
    


}
