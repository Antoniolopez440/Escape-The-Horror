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
       // public int requiredAmount;
        public GameObject[] partObjects;
    }

    public List<RequiredPart> requiredParts;

    public Dictionary<CarPartsType, int> installedParts = new();

    public void Awake()
    {
        foreach (var req in requiredParts)
        {
            installedParts[req.type] = 0;

            foreach (var obj in req.partObjects)
                obj.SetActive(false);
        }
    }

    public bool TryInstallPart(CarPart part)
    {
      if (part.partType == CarPartsType.CarKey)
        {
            if(!fullyRepaired || keyInserted)
              //  Debug.Log("Car is not repaired yet!");
                return false;
            

            keyInserted = true;
          //  Debug.Log("Car key insterted!");
            gameManager.instance.WinGame();
            return true;
        }


        RequiredPart req = requiredParts.Find(r => r.type == part.partType);
        if (req == null)
            return false;

        int installed = installedParts[part.partType];
        if (installed >= req.partObjects.Length)
            return false;


        req.partObjects[installed].SetActive(true);
        installedParts[part.partType]++;

      

        CheckIfComplete();
        return true;
    }

    void CheckIfComplete()
    {
        if (fullyRepaired)
            return;

        foreach (var req in requiredParts)
        {
            int installed = installedParts.ContainsKey(req.type)
                ? installedParts[req.type]
                : 0;

            if (installed < req.partObjects.Length)
                return;
        }

        fullyRepaired = true;
      //  Debug.Log("Car fully repaired!");

        if(bossSpawner != null)
        {
            bossSpawner.StartLevel(bossSpawnAmount);
        }
    }


}
