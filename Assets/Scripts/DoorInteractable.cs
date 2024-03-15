using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : SimpleHingeInteractable
{
    public UnityEvent onOpen;
    [SerializeField] CombinationLock comboLock;
    [SerializeField] Transform doorObject;
    [SerializeField] Vector3 rotationLimits;
    [SerializeField] Collider closedCollider;
    private Vector3 startRotation;
    private bool isClosed;
    [SerializeField] Collider openCollider;
    [SerializeField] private Vector3 endRotation;
    private bool isOpen;
    private float startAngleX;

    protected override void Start()
    {
        base.Start();
        startRotation = transform.localEulerAngles;
        startAngleX = GetAngle(startRotation.x);

        if(comboLock != null)
        {
            comboLock.UnlockAction += OnUnlocked;
            comboLock.LockAction += OnLocked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == closedCollider)
        {
            ReleaseHinge();
            isClosed = true;
        }
        else if(other == openCollider)
        {
            ReleaseHinge();
            isOpen = true;
        }
    }

    private void OnLocked()
    {
        LockHinge();
    }

    private void OnUnlocked()
    {
        UnlockHinge();
    }

    protected override void Update()
    {
        base.Update();
        if(doorObject != null)
        {
            doorObject.localEulerAngles = new Vector3(doorObject.localEulerAngles.x, transform.localEulerAngles.y, doorObject.localEulerAngles.z);
        }
        if(isSelected)
        {
            CheckLimits();
        }
    }

    private void CheckLimits()
    {
        isClosed = false;
        isOpen = false;
        float localAngleX = GetAngle(transform.localEulerAngles.x);

        if (localAngleX >= startAngleX + rotationLimits.x || localAngleX <= startAngleX - rotationLimits.x)
        {
            ReleaseHinge();
            ResetHinge();
        }
    }

    private float GetAngle(float angle)
    {
        if (angle >= 180)
        {
            angle -= 360;
        }
        return angle;
    }

    protected override void ResetHinge()
    {
        if (isClosed)
        {
            transform.localEulerAngles = startRotation;
        }
        else if (isOpen)
        {
            transform.localEulerAngles = endRotation;
            onOpen?.Invoke();
        }
        else
        {
            transform.localEulerAngles = new Vector3(startAngleX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
