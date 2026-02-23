using UnityEngine;

public enum CarPartsType
{
   Wheel,
   SteeringWheel,
   StickShift,
   CarKey
}

[CreateAssetMenu(menuName = "Items/Car Part")]
public class CarPart : ScriptableObject
{
    public CarPartsType partType;
    public Sprite uIIcon;
    public GameObject placedModel; 
}
