using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class LocalizationLocaleChanger : MonoBehaviour
{
    public void SetLocale(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetLocale(LocalizationSettings.AvailableLocales.Locales[0]);

            Debug.Log("Locale set to" + " " + LocalizationSettings.AvailableLocales.Locales[0]);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetLocale(LocalizationSettings.AvailableLocales.Locales[1]);

            Debug.Log("Locale set to" + " " + LocalizationSettings.AvailableLocales.Locales[1]);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetLocale(LocalizationSettings.AvailableLocales.Locales[2]);

            Debug.Log("Locale set to" + " " + LocalizationSettings.AvailableLocales.Locales[2]);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetLocale(LocalizationSettings.AvailableLocales.Locales[3]);

            Debug.Log("Locale set to" + " " + LocalizationSettings.AvailableLocales.Locales[3]);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetLocale(LocalizationSettings.AvailableLocales.Locales[4]);

            Debug.Log("Locale set to" + " " + LocalizationSettings.AvailableLocales.Locales[4]);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SetLocale(LocalizationSettings.AvailableLocales.Locales[5]);

            Debug.Log("Locale set to" + " " + LocalizationSettings.AvailableLocales.Locales[5]);
        }
    }
}
