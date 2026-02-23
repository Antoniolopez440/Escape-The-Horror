using UnityEngine;
using System.Collections;
using Unity.Multiplayer.Center.Common;

public class LightSwitch : MonoBehaviour
{
    [Header("Intereaction")]
    [SerializeField] string playerTag = "Player";

    [Header("Lights To Control")]
    [SerializeField] GameObject lights;

    [Header("Lever")]
    [SerializeField] Transform leverPivot;
    [SerializeField] Transform leverPoseOff;
    [SerializeField] Transform leverPoseOn;
    [SerializeField] float leverMoveTime = 0.15f;

    bool PlayerInRange;
    bool isOn;
    bool isMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (lights != null) lights.SetActive(false);

        if (leverPivot != null && leverPoseOff != null )
        {
            leverPivot.localRotation = leverPoseOff.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerInRange || isMoving) return;

        if (Input.GetButtonDown("Light Switch"))
        {
          //  Debug.Log("Interact Pressed");
            Toggle();
        }
    }

    void Toggle()
    {
        isOn = !isOn;

        if (lights != null)
        {
            lights.SetActive(isOn);
        }

        if (leverPivot != null && leverPoseOff != null && leverPoseOn != null)
        {
            StartCoroutine(RotateLever(isOn ? leverPoseOn.localRotation : leverPoseOff.localRotation));
        }
    }

    IEnumerator RotateLever(Quaternion targetRot)
    {
        isMoving = true;

        Quaternion startRot = leverPivot.localRotation;
        float t = 0F;

        while (t < 1f)
        {
            t += Time.deltaTime / leverMoveTime;
            leverPivot.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        leverPivot.localRotation = targetRot;
        isMoving = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag)) PlayerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag)) PlayerInRange = false;
    }
}
