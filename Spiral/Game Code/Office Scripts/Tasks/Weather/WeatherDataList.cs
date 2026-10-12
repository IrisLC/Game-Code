using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Class that sits on the weather window object, handles the list of weather data images relating to different locations
/// </summary>
public class WeatherDataList : MonoBehaviour
{
    /// <summary>
    /// static reference to the script in the current scene
    /// </summary>
    public static WeatherDataList CurrentWindow { get; private set; } = null;
    [SerializeField] Image ImageElement;
    /// <summary>
    /// public getter property for the image currently being shown on the weather app
    /// </summary>
    public Sprite activeImage { get => ImageElement.sprite; private set => ImageElement.sprite = value; }
    [SerializeField] TextMeshProUGUI Text;
    /// <summary>
    /// public agetter property for the place text currently being shown on the weather app
    /// </summary>
    public string activeText { get => Text.text; private set => Text.SetText(value); }

    [Serializable]
    class WeatherEntries
    {
        // The "Correct" image, displayed in the window and on correct weather tasks
        public Sprite pairA;
        // The outdated scan, displayed on the weather task if the scan is set to be incorrect
        public Sprite pairB;
        // The location of the scans
        public string placeText;
    }

    [SerializeField] List<WeatherEntries> weatherEntries = new();
    int index = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Singleton code
        if (CurrentWindow == null)
        {
            CurrentWindow = this;
        }
        else
        {
            Destroy(gameObject);
        }


        if (weatherEntries.Count != 0)
        {
            activeImage = weatherEntries[0].pairA;
            activeText = weatherEntries[0].placeText;
        }
        else
        {
            Debug.LogError("No Weather Entries!");
        }
    }

    public void NextEntry()
    {
        Assert.IsTrue(weatherEntries.Count != 0);
        ++index;

        if (index == weatherEntries.Count)
        {
            index = 0;
        }

        UpdateWeather();
    }

    public void LastEntry()
    {
        Assert.IsTrue(weatherEntries.Count != 0);

        --index;

        if (index == -1)
        {
            index = weatherEntries.Count - 1;
        }

        UpdateWeather();
    }

    void UpdateWeather()
    {
        activeImage = weatherEntries[index].pairA;
        activeText = weatherEntries[index].placeText;
    }

    public static Sprite GetImageForTask(string searchText, bool isWeatherCorrect)
    {
        WeatherEntries foundEntry = null;
        foreach (WeatherEntries entry in CurrentWindow.weatherEntries)
        {
            if (entry.placeText == searchText)
            {
                foundEntry = entry;
                break;
            }
        }

        if (foundEntry == null)
        {
            Debug.LogError("No entry found");
            return null;
        }

        return isWeatherCorrect ? foundEntry.pairA : foundEntry.pairB;
    }

    void OnDestroy()
    {
        if (CurrentWindow == this)
        {
            CurrentWindow = null;
        }

    }
}
