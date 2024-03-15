using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent (typeof(Collider))]
[RequireComponent (typeof(Rigidbody))]
public abstract class SimpleHingeInteractable : XRSimpleInteractable
{
    [SerializeField] Vector3 positionLimits;
    [SerializeField] Transform grabHand;
    private Collider hingeCollider;
    private Vector3 hingePositions;
    [SerializeField] bool isLocked;
    private const string defaultLayer = "Default";
    private const string grabLayer = "Grab";

    protected virtual void Start()
    {
        hingeCollider = GetComponent<Collider>();
    }

    public void UnlockHinge()
    {
        isLocked = false;
    }

    public void LockHinge()
    {
        isLocked = true;
    }

    protected virtual void Update()
    {
        if(grabHand != null)
        {
            TrackHand();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if(!isLocked)
        {
            base.OnSelectEntered(args);
            grabHand = args.interactableObject.transform;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        grabHand = null;
        ChangeLayerMask(grabLayer);
        ResetHinge();
    }

    private void TrackHand()
    {
        transform.LookAt(grabHand, transform.forward);
        hingePositions = hingeCollider.bounds.center;
        if(grabHand.position.x >= hingePositions.x + positionLimits.x ||
           grabHand.position.x <= hingePositions.x - positionLimits.x)
        {
            ReleaseHinge();
        }
        if (grabHand.position.y >= hingePositions.y + positionLimits.y ||
            grabHand.position.y <= hingePositions.y - positionLimits.y)
        {
            ReleaseHinge();
        }
        if (grabHand.position.z >= hingePositions.z + positionLimits.z ||
            grabHand.position.z <= hingePositions.z - positionLimits.z)
        {
            ReleaseHinge();
        }
    }

    public void ReleaseHinge()
    {
        ChangeLayerMask(defaultLayer);
    }

    protected abstract void ResetHinge();

    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
