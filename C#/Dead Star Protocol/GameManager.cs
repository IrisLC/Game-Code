using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Animation deathCam;
    [SerializeField] private PlayerController pm;

    [SerializeField] private GameObject particles;

    [SerializeField] private GameObject subtitlesManager;
    [SerializeField] private GameObject reader;
    public AudioClip explosionSFX;
    bool soundPlaying = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartSceneDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameTimeManager.GetIsGamePaused())
        {
            return;
        }
    }

    public void timeUp()
    {
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        if (!soundPlaying)
        {
            soundPlaying = true;
            audioManager.PlaySFX(explosionSFX);
        }
        particles.SetActive(true);
        Invoke("afterDeath", 2f);
    }


    public void killPlayer()
    {
        pm.lockControls = true;
        deathCam.wrapMode = WrapMode.ClampForever;
        deathCam.Play();

        Invoke("afterDeath", 4f);
    }

    private void afterDeath()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator StartSceneDelay()
    {
        yield return new WaitForSeconds(1f);

        subtitlesManager.GetComponent<SubtitlesManager>().playSubtitleSequence(reader.GetComponent<Reader>().allSubtitlesList.StartScene);
    }
}
