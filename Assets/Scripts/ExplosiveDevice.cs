using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ExplosiveDevice : XRGrabInteractable
{
    public UnityEvent OnDetonate;
    private bool isActivated;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(args.interactableObject.transform.GetComponent<XRSocketInteractor>() != null)
        {
            isActivated = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isActivated && collision.gameObject.GetComponent<WandProjectile>() != null)
        {
            OnDetonate?.Invoke();
        }
    }
}
