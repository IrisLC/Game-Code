using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The active gameManager instance, exists for singleton
    /// </summary>
    static GameManager Instance;
    /// <summary>
    /// The OfficeManager instance the game is working with
    /// </summary>
    static OfficeManager officeManager;

    /// <summary>
    /// The possible scenes that could be active
    /// </summary>
    public enum SceneTypes { Menu, Office, Anxiety, Home }
    /// <summary>
    /// The current type of scene
    /// </summary>
    public static SceneTypes CurrentSceneType { get; private set; }

    /// <summary>
    /// The scene indexes for all Office scenes in the game, must be in order
    /// </summary>
    [SerializeField] int[] OfficeSceneIndexes;
    /// <summary>
    /// The scene indexes for all Home scenes in the game, must be in order
    /// </summary>
    [SerializeField] int[] HomeSceneIndexes;
    /// <summary>
    /// The scene indexes for all Maze scenes in the game, must be in order
    /// </summary>
    [SerializeField] int[] MazeSceneIndexes;

    /// <summary>
    /// The index of the scene we are coming from
    /// </summary>
    public static int PreviousLevelIndex { get; private set; }
    /// <summary>
    /// The index of the scene we are in
    /// </summary>
    public static int CurrentLevelIndex { get; private set; }
    /// <summary>
    /// The scene index that we should go to when going to an office scene, should change at ends of Office days.
    /// </summary>
    public static int CurrentOfficeIndex { get; private set; } = 1;
    /// <summary>
    /// The scene index that we should go to when going to a maze scene.
    /// </summary>
    public static int CurrentMazeIndex { get; private set; } = 2;
    /// <summary>
    /// The scene index that we should go to when going to a home scene.
    /// </summary>
    public static int CurrentHomeIndex { get; private set; } = 3;

    /// <summary>
    /// Whether or not the player made it to the goal of the maze they were in
    /// </summary>
    public bool SucceededMaze;

    /// <summary>
    /// If the playerInput variable has been successfully created
    /// </summary>
    public static bool PlayerInputInitialized { get; private set; }
    /// <summary>
    /// The PlayerInput object for the game
    /// </summary>
    static PlayerInput playerInput;
    /// <summary>
    /// The OfficePlayerActions found in playerInput 
    /// </summary>
    public static PlayerInput.OfficePlayerActions OfficeInputs { get; private set; }
    /// <summary>
    /// The AnxietyPLayerActions found in playerInput 
    /// </summary>
    public static PlayerInput.AnxietyPlayerActions AnxietyInputs { get; private set; }

#if UNITY_EDITOR
    /// <summary>
    /// If Debug.Log commands should be called, modified with DebugCommands
    /// </summary>
    internal static bool isDebugging;
#endif

    void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Set up references and connect to Scene change event
        Referencer.gameManager = this;
        SceneManager.sceneLoaded += OnSceneLoad;

        SetupInputs();
    }

    public static void SetupInputs()
    {
        if (!PlayerInputInitialized)
        {
            // Set up PlayerInputs
            playerInput = new PlayerInput();
            OfficeInputs = playerInput.OfficePlayer;
            AnxietyInputs = playerInput.AnxietyPlayer;
            PlayerInputInitialized = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentOfficeIndex = OfficeSceneIndexes[0];
        CurrentMazeIndex = MazeSceneIndexes[0];
    }

    void OnEnable()
    {
        OfficeInputs.Enable();
        AnxietyInputs.Enable();
    }

    /// <summary>
    /// After short time, enters an anxiety attack section
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnterAnxietyAttack()
    {
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene(CurrentMazeIndex);
        officeManager.enabled = false;

        CurrentSceneType = SceneTypes.Anxiety;
    }

    /// <summary>
    /// Loads the current office scene
    /// </summary>
    public static void LoadOffice()
    {
        if (CurrentLevelIndex == CurrentOfficeIndex) return;
        SceneManager.LoadScene(CurrentOfficeIndex);

        CurrentSceneType = SceneTypes.Office;
    }

    /// <summary>
    /// Called when a new scene is loaded, handles transitions and setting up relevant method calls
    /// </summary>
    /// <param name="scene">The scene that was just loaded</param>
    /// <param name="mode">Whether the load was additive or a solo scene</param>
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        PreviousLevelIndex = CurrentLevelIndex;
        CurrentLevelIndex = scene.buildIndex;

        //If we are entering into the office
        if (CurrentLevelIndex == CurrentOfficeIndex)
        {
            OnEnteringOffice();
        }
    }

    /// <summary>
    /// Handles how to enter an office scene depending on if we came from a maze scene, are just starting the game, or neither.
    /// </summary>
    void OnEnteringOffice()
    {
        // If we came from the maze, resume the office day, otherwise start a new day
        if (PreviousLevelIndex == CurrentMazeIndex)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            if (officeManager)
            {
                officeManager.enabled = true;
                officeManager.ResumeDay();
            }

        }
        else
        {
            if (officeManager == null)
            {
                officeManager = OfficeManager.GetOfficeManager();
            }
            else
            {
                officeManager?.StartDay();
            }
        }
    }

    void OnDestroy()
    {
        // Resets singleton variables
        if (Instance == this)
        {
            Instance = null;
            OfficeInputs.Disable();
            AnxietyInputs.Disable();
        }
    }

    /// <summary>
    /// Starts a new game
    /// </summary>
    public static void StartGame()
    {
#if UNITY_EDITOR
        if (isDebugging) Debug.Log("Start Game");
#endif
        LoadOffice();
    }

    /// <summary>
    /// Destroys all DontDestroyOnLoad objects and loads the Main menu
    /// </summary>
    public static void ResetGame()
    {
        foreach (GameObject go in Instance.gameObject.scene.GetRootGameObjects().Reverse())
        {
            Destroy(go);
        }

        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Exits the game
    /// </summary>
    public static void QuitGame()
    {
#if UNITY_EDITOR
        if (isDebugging) Debug.Log("Exit");
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
