using UnityEngine;
using System.Collections.Generic;



public class CarRepair : MonoBehaviour
{
    bool fullyRepaired;
    bool keyInserted;

    [SerializeField] spawner bossSpawner;
    [SerializeField] int bossSpawnAmount = 1;

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
      if (part.partType == CarPartsType.CarKey)
        {
            if(!fullyRepaired)
            {
                Debug.Log("Car is not repaired yet!");
                return false;
            }
            if (keyInserted)
            {
                Debug.Log("Key already inserted");
                return false;
            }

            keyInserted = true;
            Debug.Log("Car key insterted!");
            gameManager.instance.WinGame();
            return true;
        }


        RequiredPart req = requiredParts.Find(r => r.type == part.partType);
        if (req == null)
            return false;

        int installed = installedParts.TryGetValue(part.partType, out int count)
            ? count
            : 0;

        if (installed >= req.attachPoints.Length)
            return false;

        Transform point = req.attachPoints[installed];

        if (part.placedModel != null && point != null)
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

        fullyRepaired = true;
        Debug.Log("Car fully repaired!");

        if(bossSpawner != null)
        {
            bossSpawner.StartLevel(bossSpawnAmount);
        }
    }


}
