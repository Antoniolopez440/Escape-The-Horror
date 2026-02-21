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
            var go = Instantiate(iconPrefab, container, false);
            Image img = go.GetComponent<Image>();

            img.sprite = part.uIIcon;
            img.preserveAspect = true;
            //go.transform.SetParent(container, false);
            //Image img = go.GetComponent<Image>();
        }
    }
    


}
