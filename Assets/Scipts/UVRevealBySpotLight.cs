using UnityEngine;

public class UVRevealBySpotLight : MonoBehaviour
{

    [Header("Assign the Player-Held spotLight here")]
    [SerializeField] private Light blackLight;



    [Header("Renderer to reveal UV on")]
    [SerializeField] private Renderer[] renderersToToggle;

    [Header("Tuning")]
    [SerializeField] private float extraRange = 0.5f; // How much extra range to add to the spotlight for revealing the UV
    [SerializeField] private float extraAngle = 5f; // How much extra angle to add to the spotlight for revealing the UV
    [SerializeField] private LayerMask occlusionMask = ~0;

     
    private void Awake()
    {
        if (renderersToToggle == null || renderersToToggle.Length == 0)
            renderersToToggle = GetComponents<Renderer>();

        SetVisible(false);

    }
    // Update is called once per frame
    void Update()
    {
        if (!FlashLightPickup.HasFlashlightt)
        {
            SetVisible(false);
            return;
        }

        if (blackLight == null || !blackLight.enabled || blackLight.type != LightType.Spot)
        {
            SetVisible(false);
            return;
        }

        Vector3 LightPos = blackLight.transform.position;
        Vector3 targetPos = GetTargetPoint();

        Vector3 toTarget = targetPos - LightPos;
        float dist = toTarget.magnitude;

        if (dist > blackLight.range + extraRange)
        {
            SetVisible(false);
            return;
        }

        float halfAngle = blackLight.spotAngle / 2f + extraAngle;
        float angle = Vector3.Angle(blackLight.transform.forward, toTarget);


        if (angle > halfAngle)
        {
            SetVisible(false);
            return;
        }

        if (Physics.Raycast(LightPos, toTarget.normalized,out RaycastHit hit, dist, occlusionMask))
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);
    }


    Vector3 GetTargetPoint()
    {
         if (renderersToToggle != null && renderersToToggle.Length > 0 && renderersToToggle[0] != null)
            return renderersToToggle[0].bounds.center;

         return transform.position;

    }

    void SetVisible(bool on)
    {
       for (int i = 0; i < renderersToToggle.Length; i++)
        {
            if (renderersToToggle[i] != null)
                renderersToToggle[i].enabled = on;
        }
    }

    public void SetBlackLight(Light l) => blackLight = l;
}
