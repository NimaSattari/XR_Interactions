using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class CombinationLock : MonoBehaviour
{
    public UnityAction UnlockAction;
    private void OnUnlocked() => UnlockAction?.Invoke();
    public UnityAction LockAction;
    private void OnLocked() => LockAction?.Invoke();
    [SerializeField] XRButtonInteractable[] comboButtons;
    [SerializeField] TextMeshProUGUI userInputText;
    [SerializeField] Image lockedPanel;
    [SerializeField] Color unlockedColor, lockedColor;
    [SerializeField] TextMeshProUGUI lockedText;
    [SerializeField] bool isLocked;
    [SerializeField] int[] comboValues = new int[3];
    [SerializeField] int[] inputValues;
    private int maxButtonPresses;
    private int buttonPresses;

    void Start()
    {
        maxButtonPresses = comboValues.Length;
        ResetUserValues();
        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }
    }

    private void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {
        if(buttonPresses >= maxButtonPresses)
        {

        }
        else
        {
            for (int i = 0; i < comboButtons.Length; i++)
            {
                if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
                {
                    userInputText.text += i.ToString();
                    inputValues[buttonPresses] = i;
                }
                else
                {
                    comboButtons[i].ResetColor();
                }
            }
            buttonPresses++;
            if(buttonPresses == maxButtonPresses)
            {
                CheckCombo();
            }
        }
    }

    private void CheckCombo()
    {
        int matches = 0;
        for (int i = 0; i < maxButtonPresses; i++)
        {
            if (inputValues[i] == comboValues[i])
            {
                matches++;
            }
        }
        if (matches == maxButtonPresses)
        {
            isLocked = false;
            lockedPanel.color = unlockedColor;
            lockedText.text = "Unlocked";
        }
        else
        {
            ResetUserValues();
        }
    }

    private void UnlockCombo()
    {
        isLocked = false;
        lockedPanel.color = unlockedColor;
        OnUnlocked();
    }

    private void LockCombo()
    {
        isLocked = true;
        lockedPanel.color = lockedColor;
        userInputText.text = string.Empty;
        for (int i = 0; i < maxButtonPresses; i++)
        {
            comboValues[i] = inputValues[i];
        }
        OnLocked();
    }

    private void ResetUserValues()
    {
        inputValues = new int[maxButtonPresses];
        userInputText.text = "";
        buttonPresses = 0;
    }
}
