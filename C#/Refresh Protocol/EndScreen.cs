using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] GameObject[] stars = new GameObject[3];
    [SerializeField] int par;
    [SerializeField] TextMeshProUGUI parText;
    private InputReader ir;

    void Awake()
    {
        ir = GameObject.FindGameObjectWithTag("InputReader").GetComponent<InputReader>();
        scoring();
    }

    private void scoring()
    {
        stars[0].SetActive(true);
        int count = 0;
        foreach (GameManager.Inputs input in ir.inputs)
        {
            if (input != GameManager.Inputs.None) count++;
        }
        if (count <= par) stars[1].SetActive(true);
        if (count < par) stars[2].SetActive(true);
        parText.text = "Par: " + par;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
