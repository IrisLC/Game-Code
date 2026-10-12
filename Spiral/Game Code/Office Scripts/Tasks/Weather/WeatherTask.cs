using System.Collections;
using UnityEngine;

public class WeatherTask : ATask
{
    public override TaskType Type { get => TaskType.Weather; }
    [SerializeField] bool isWeatherAccurate;
    [SerializeField] string placeName;
    public Sprite image { get; private set; }
    public bool playerInput;

    void Start()
    {
        image = WeatherDataList.GetImageForTask(placeName, isWeatherAccurate);
    }

    void OnEnable()
    {
        StartCoroutine(SetChanges());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Referencer.OfficeReferences.WeatherImage.sprite = null;
        Referencer.OfficeReferences.WeatherText.SetText("");
    }

    /// <summary>
    /// Was running into issues with the image being set to null because it 
    /// was disabled the frame we were assigning the value, so this delays the 
    /// change by one frame, so we ensure the image is enabled before setting the value
    /// </summary>
    IEnumerator SetChanges()
    {
        yield return null;
        Referencer.OfficeReferences.WeatherImage.sprite = image;
        Referencer.OfficeReferences.WeatherText.SetText(placeName);
        Canvas.ForceUpdateCanvases();
    }

    protected override bool EvaluateTask()
    {
        return playerInput == isWeatherAccurate;
    }

    public override string GetResponse()
    {
        return CorrectionEmail.WeatherCorrection1 + (playerInput ? "" : "not ") + CorrectionEmail.WeatherCorrection2;
    }

}
