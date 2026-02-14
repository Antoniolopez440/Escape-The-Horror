using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class CarRepair : MonoBehaviour
{
    [System.Serializable]
    public class RequiredPart
    {
        public CarPartsType type;
        public int requiredAmount;
        public Transform[] attachPoints;
    }

    public List<RequiredPart> requiredParts;

    public Dictionary<CarPartsType, int> installedParts = new();

    public void Awake()
    {
        foreach (var part in requiredParts)
            installedParts[part.type] = 0;
    }

    public bool TryInstallPart(CarPart part)
    {
      

        RequiredPart req = requiredParts.Find(r => r.type == part.partType);
        if (req == null)
            return false;

        int installed = installedParts.ContainsKey(part.partType)
            ? installedParts[part.partType]
            : 0;

        if (installed >= req.attachPoints.Length)
            return false;

        Transform point = req.attachPoints[installed];

        if (part.placedModel && point)
            Instantiate(part.placedModel, point.position, point.rotation, point);

        installedParts[part.partType] = installed + 1;

        CheckIfComplete();
        return true;
    }

    void CheckIfComplete()
    {
        foreach (var req in requiredParts)
        {
            int installed = installedParts.ContainsKey(req.type)
                ? installedParts[req.type]
                : 0;

            if (installed < req.attachPoints.Length)
                return;
        }

        Debug.Log("Car fully repaired!");
    }


}
