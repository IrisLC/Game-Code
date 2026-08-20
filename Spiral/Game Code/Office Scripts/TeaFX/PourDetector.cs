using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform origin = null;
    public GameObject streamPrefab = null;

    bool isPouring = false;
    Stream currentStream = null;

    void Update()
    {
        bool pourCheck = CalculatePourAngle() < pourThreshold;

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;
            if (isPouring)
            {
                StartPour();
            }
            else
            {
                EndPour();
            }
        }
    }

    void StartPour()
    {
        currentStream = CreateStream();
        currentStream.Begin();
    }

    void EndPour()
    {
        currentStream.End();
        currentStream = null;
    }

    float CalculatePourAngle()
    {
        return transform.up.y * Mathf.Rad2Deg;
    }

    Stream CreateStream()
    {
        GameObject streamObject = Instantiate(streamPrefab, origin.position, Quaternion.identity, transform);
        return streamObject.GetComponent<Stream>();
    }
}
