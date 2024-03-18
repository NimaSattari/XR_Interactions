using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : XRGrabInteractable
{
    [SerializeField] Transform drawerTransform;
    [SerializeField] XRSocketInteractor keySocket;
    public XRSocketInteractor GetKeySocket => keySocket;
    [SerializeField] bool isLocked;
    [SerializeField] GameObject myLight;
    [SerializeField] AudioClip drawerMoveClip;
    public AudioClip GetDrawerMoveClip => drawerMoveClip;
    [SerializeField] AudioClip socketedClip;
    public AudioClip GetSocketedClip => socketedClip;

    private Transform parentTransform;
    private const string defaultLayer = "Default";
    private const string grabLayer = "Grab";
    private bool isGrab;
    private Vector3 limitPos;
    [SerializeField] float drawerLimitZ = 0.9f;
    [SerializeField] private Vector3 limitDistances = new Vector3(0.2f, 0.2f, 0);

    void Start()
    {
        if(keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }
        parentTransform = transform.parent.transform;
        limitPos = drawerTransform.localPosition;
    }

    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        print("*** Drawer Locked");
    }

    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        if(myLight != null)
        {
            myLight.gameObject.SetActive(false);
        }
        print("*** Drawer Unlocked");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(!isLocked)
        {
            transform.SetParent(parentTransform);
            isGrab = true;
        }
        else
        {
            ChangeLayerMask(defaultLayer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        ChangeLayerMask(grabLayer);
        isGrab = false;
        transform.localPosition = drawerTransform.localPosition;
    }

    void Update()
    {
        if(isGrab && drawerTransform != null)
        {
            drawerTransform.localPosition = new Vector3(drawerTransform.localPosition.x, drawerTransform.localPosition.y, transform.localPosition.z);
            CheckLimits();
        }
    }

    private void CheckLimits()
    {
        if (transform.localPosition.x >= limitPos.x + limitDistances.x || transform.localPosition.x <= limitPos.x - limitDistances.x)
        {
            ChangeLayerMask(defaultLayer);
        }
        else if (transform.localPosition.y >= limitPos.y + limitDistances.y || transform.localPosition.y <= limitPos.y - limitDistances.y)
        {
            ChangeLayerMask(defaultLayer);
        }
        else if(drawerTransform.localPosition.z <= limitPos.z - limitDistances.z)
        {
            isGrab = false;
            drawerTransform.localPosition = limitPos;
            ChangeLayerMask(defaultLayer);
        }
        else if (drawerTransform.localPosition.z >= limitPos.z + limitDistances.z)
        {
            isGrab = false;
            drawerTransform.localPosition = new Vector3(drawerTransform.localPosition.x, drawerTransform.localPosition.y, drawerLimitZ);
            ChangeLayerMask(defaultLayer);
        }
    }

    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
