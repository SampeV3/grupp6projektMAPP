using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private bool _active = false;
    private bool localizationActivated = false;

    private void Start()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(ID);
    }

    private void FixedUpdate()
    {
        if (!localizationActivated)
        {
            localizationActivated = true;
            int ID = PlayerPrefs.GetInt("LocaleKey", 0);
            ChangeLocale(ID);
        }
    }

    public void ChangeLocale(int localeID)
    {
        if (_active)
        {
            return;
        }

        StartCoroutine(SetLocale(localeID));
    }

    private IEnumerator SetLocale(int _localeID)
    {
        _active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        _active = false;
    }
}