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
        public Transform attatchPoint;
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
        if (!installedParts.ContainsKey(part.partType))
            return false;

        RequiredPart req = requiredParts.Find(r => r.type == part.partType);
        if (installedParts[part.partType] >= req.requiredAmount)
            return false;

        installedParts[part.partType]++;

        if (part.placedModel && req.attatchPoint)
            Instantiate(part.placedModel, req.attatchPoint.position, req.attatchPoint.rotation, req.attatchPoint);

        CheckIfComplete();
        return true;
    }

    void CheckIfComplete()
    {
        foreach (var req in requiredParts)
        {
            if (installedParts[req.type] < req.requiredAmount)
                return;
        }

        Debug.Log("Car fully repaired!");
    }


}
