using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVibrationHandler : MonoBehaviour
{
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private Button SettingsButton;
    [SerializeField]
    private Button QuitButton;

    private void OnEnable()
    {
        StartButton.onClick.AddListener(DefaultVibration);
        SettingsButton.onClick.AddListener(DefaultVibration);
        QuitButton.onClick.AddListener(DefaultVibration);

    }

    private void OnDisable()
    {
        StartButton.onClick.RemoveListener(DefaultVibration);
        SettingsButton.onClick.RemoveListener(DefaultVibration);
        QuitButton.onClick.RemoveListener(DefaultVibration);
    }

    private void DefaultVibration()
    {
        Debug.Log("Default Vibration performed!");
        Handheld.Vibrate();

    }
}
