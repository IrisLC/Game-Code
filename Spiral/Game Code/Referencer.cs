using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("EditorTests")]

public class Referencer : MonoBehaviour
{
    //These Assign themselves
    /// <summary>
    /// The game manager overseeing the game
    /// </summary>
    public static GameManager gameManager;

    /// <summary>
    /// The references for the office scene
    /// </summary>
    [SerializeField] OfficeReferences officeReferences;
    /// <summary>
    /// The references for the office scene
    /// </summary>
    [System.Serializable]
    public class OfficeReferences
    {
        /// <summary>
        /// The office manager handling all details in the office
        /// </summary>
        public static OfficeManager officeManager;
        /// <summary>
        /// The screen brought up when pausing the game.
        /// </summary>
        [Header("General")]
        [SerializeField] GameObject _pauseScreen;
        /// <summary>
        /// The screen brought up when pausing the game.
        /// </summary>
        public static GameObject pauseScreen { get; internal set; }

        /// <summary>
        /// The UI elements in the office
        /// </summary>
        [Header("Office")]
        [SerializeField] OfficeUIObjects officeUIObjects;
        /// <summary>
        /// The UI elements in the office
        /// </summary>
        [System.Serializable]
        class OfficeUIObjects
        {
            /// <summary>
            /// The Canvas holding the computerUI
            /// </summary>
            [SerializeField] internal Canvas _ComputerUI;
            /// <summary>
            /// The gauge representing the player's anxiety
            /// </summary>
            [Header("Anxiety Bar Elements")]
            [SerializeField] internal GameObject _AnxietyBar;
            /// <summary>
            /// The dial showing how far along the AnxietyBar the player's anxiety level is
            /// </summary>
            [SerializeField] internal Image _AnxietyDial;
            /// <summary>
            /// An array of gameObjects that should be hidden when the Computer is opened
            /// </summary>
            [Space]
            [SerializeField] internal GameObject[] _DisappearOnComputerOpen;
            /// <summary>
            /// The introductory message played for the player at the start of the game
            /// </summary>
            [SerializeField] internal GameObject _OpeningMessage;
            /// <summary>
            /// The GameObject that holds all email tasks
            /// </summary>
            [Header("Task Holders")]
            [SerializeField] internal GameObject _EmailHolder;
            /// <summary>
            /// The GameObject that holds all Incident Report tasks
            /// </summary>
            [SerializeField] internal GameObject _IRHolder;
            /// <summary>
            /// The GameObject that holds all printing tasks
            /// </summary>
            [SerializeField] internal GameObject _PrinterHolder;
            /// <summary>
            /// The GameObject holding all the active task buttons
            /// </summary>
            [SerializeField] internal GameObject _TodoList;
            /// <summary>
            /// The prefab around which all Todo buttons will be created from
            /// </summary>
            [Space]
            [SerializeField] internal GameObject _ButtonPrefab;
        }
        /// <summary>
        /// The Canvas holding the computerUI
        /// </summary>
        public static Canvas ComputerUI { get; internal set; }
        /// <summary>
        /// The gauge representing the player's anxiety
        /// </summary>
        public static GameObject AnxietyBar { get; internal set; }
        /// <summary>
        /// The dial showing how far along the AnxietyBar the player's anxiety level is
        /// </summary>
        public static Image AnxietyDial { get; internal set; }
        /// <summary>
        /// An array of gameObjects that should be hidden when the Computer is opened
        /// </summary>
        public static GameObject[] DisappearOnComputerOpen;
        /// <summary>
        /// The introductory message played for the player at the start of the game
        /// </summary>
        public static GameObject OpeningMessage { get; internal set; }
        /// <summary>
        /// The GameObject that holds all email tasks
        /// </summary>
        public static GameObject EmailHolder { get; internal set; }
        /// <summary>
        /// The GameObject that holds all Incident Report tasks
        /// </summary>
        public static GameObject IRHolder { get; internal set; }
        /// <summary>
        /// The GameObject that holds all printing tasks
        /// </summary>
        public static GameObject PrinterHolder { get; internal set; }
        /// <summary>
        /// The GameObject holding all the active task buttons
        /// </summary>
        public static GameObject TodoList { get; internal set; }
        /// <summary>
        /// The prefab around which all Todo buttons will be created from
        /// </summary>
        public static GameObject ButtonPrefab { get; internal set; }

        /// <summary>
        /// The non-ui objects in the office that have important functionality
        /// </summary>
        [SerializeField] OfficeFunctionalityObjects officeFunctionalityObjects;
        /// <summary>
        /// The non-ui objects in the office that have important functionality
        /// </summary>
        [System.Serializable]
        class OfficeFunctionalityObjects
        {
            /// <summary>
            /// The camera the player sees through in the office
            /// </summary>
            [SerializeField] internal OfficeCamera _officeCamera;
            /// <summary>
            /// The ComputerScript attatched to the computer in the office
            /// </summary>
            [SerializeField] internal ComputerScript _Computer;
            /// <summary>
            /// The script containing the modifiers for the current office scene
            /// </summary>
            [SerializeField] internal DailyModifiers _dailyModifiers;
        }

        /// <summary>
        /// The camera the player sees through in the office
        /// </summary>
        public static OfficeCamera officeCamera { get; internal set; }
        /// <summary>
        /// The ComputerScript attatched to the computer in the office
        /// </summary>
        public static ComputerScript Computer { get; internal set; }
        /// <summary>
        /// The script containing the modifiers for the current office scene
        /// </summary>
        public static DailyModifiers dailyModifiers { get; internal set; }

        /// <summary>
        /// The Animations that play in the office
        /// </summary>
        [SerializeField] OfficeEffects officeEffects;
        /// <summary>
        /// The Animations that play in the office
        /// </summary>
        [System.Serializable]
        class OfficeEffects
        {
            /// <summary>
            /// The camera blur that occues when entering an anxiety scene
            /// </summary>
            [SerializeField] internal Animator _Blur;
            /// <summary>
            /// The red screen flash when failing a task
            /// </summary>
            [SerializeField] internal Animator _ScreenFlash;
        }

        /// <summary>
        /// The camera blur that occues when entering an anxiety scene
        /// </summary>
        public static Animator Blur { get; internal set; }
        /// <summary>
        /// The red screen flash when failing a task
        /// </summary>
        public static Animator ScreenFlash { get; internal set; }
        /// <summary>
        /// The elements having to do with the operation of the phone
        /// </summary>
        [SerializeField] PhoneElements phoneElements;
        /// <summary>
        /// The elements having to do with the operation of the phone
        /// </summary>
        [System.Serializable]
        class PhoneElements
        {
            /// <summary>
            /// The Canvas holding all the UI elements of the phone
            /// </summary>
            [SerializeField] internal Canvas _PhoneUI;
            /// <summary>
            /// The GameObject that serves as the photos app
            /// </summary>
            [SerializeField] internal GameObject _Photos;
            /// <summary>
            /// The Game Objects that serves as the home screen of the phone
            /// </summary>
            [SerializeField] internal GameObject _Main;
            /// <summary>
            /// The Game Object that serves as the list of contacts, before going to a conversation
            /// </summary>
            [SerializeField] internal GameObject _Contacts;
            /// <summary>
            /// The GameObject that holds all ongoing text conversations
            /// </summary>
            [SerializeField] internal GameObject _Texts;
            /// <summary>
            /// The GameObject holding the texts with Mom
            /// </summary>
            [SerializeField] internal GameObject _Mom;
            /// <summary>
            /// The GameObject holding the texts with Hope
            /// </summary>
            [SerializeField] internal GameObject _Friend;
            /// <summary>
            /// The Button to exit a message
            /// </summary>
            [SerializeField] internal GameObject _exitMessages;
            /// <summary>
            /// The button to close the phone
            /// </summary>
            [SerializeField] internal GameObject _exitPhone;
            /// <summary>
            /// The Instance of the dialogueManager in the scene
            /// </summary>
            [SerializeField] internal DialogueManager _dialogueManager;
            /// <summary>
            /// The script that handles when the player gets a new message on their phone
            /// </summary>
            [SerializeField] internal messageNotification _messageNotification;
        }
        /// <summary>
        /// The Canvas holding all the UI elements of the phone
        /// </summary>
        public static Canvas PhoneUI { get; internal set; }
        /// <summary>
        /// The GameObject that serves as the photos app
        /// </summary>
        public static GameObject Photos { get; internal set; }
        /// <summary>
        /// The Game Objects that serves as the home screen of the phone
        /// </summary>
        public static GameObject Main { get; internal set; }
        /// <summary>
        /// The Game Object that serves as the list of contacts, before going to a conversation
        /// </summary>
        public static GameObject Contacts { get; internal set; }
        /// <summary>
        /// The GameObject that holds all ongoing text conversations
        /// </summary>
        public static GameObject Texts { get; internal set; }
        /// <summary>
        /// The GameObject holding the texts with Mom
        /// </summary>
        public static GameObject Mom { get; internal set; }
        /// <summary>
        /// The GameObject holding the texts with Hope
        /// </summary>
        public static GameObject Friend { get; internal set; }
        /// <summary>
        /// The Button to exit a message
        /// </summary>
        public static GameObject exitMessages { get; internal set; }
        /// <summary>
        /// The button to close the phone
        /// </summary>
        public static GameObject exitPhone { get; internal set; }
        /// <summary>
        /// The Instance of the dialogueManager in the scene
        /// </summary>
        public static DialogueManager dialogueManager { get; internal set; }
        /// <summary>
        /// The script that handles when the player gets a new message on their phone
        /// </summary>
        public static messageNotification messageNotification { get; internal set; }

        internal void SetUp()
        {
            pauseScreen = _pauseScreen;

            officeCamera = officeFunctionalityObjects._officeCamera;
            Computer = officeFunctionalityObjects._Computer;
            dailyModifiers = officeFunctionalityObjects._dailyModifiers;

            ComputerUI = officeUIObjects._ComputerUI;
            AnxietyBar = officeUIObjects._AnxietyBar;
            AnxietyDial = officeUIObjects._AnxietyDial;
            OpeningMessage = officeUIObjects._OpeningMessage;
            EmailHolder = officeUIObjects._EmailHolder;
            IRHolder = officeUIObjects._IRHolder;
            PrinterHolder = officeUIObjects._PrinterHolder;
            ButtonPrefab = officeUIObjects._ButtonPrefab;
            TodoList = officeUIObjects._TodoList;

            Blur = officeEffects._Blur;
            ScreenFlash = officeEffects._ScreenFlash;

            PhoneUI = phoneElements._PhoneUI;
            Photos = phoneElements._Photos;
            Main = phoneElements._Main;
            Texts = phoneElements._Texts;
            Contacts = phoneElements._Contacts;
            Mom = phoneElements._Mom;
            Friend = phoneElements._Friend;
            exitMessages = phoneElements._exitMessages;
            exitPhone = phoneElements._exitPhone;
            dialogueManager = phoneElements._dialogueManager;
            messageNotification = phoneElements._messageNotification;

            // If we have not been provided a list, instead do a manual search for the objects
            DisappearOnComputerOpen = officeUIObjects._DisappearOnComputerOpen.Length == 0 ?
                officeUIObjects._DisappearOnComputerOpen : GameObject.FindGameObjectsWithTag("DisappearOnComputerOpen");
        }
    }

    /// <summary>
    /// The references for objects in the anxietyScenes
    /// </summary>
    [SerializeField] AnxietyReferences anxietyReferences;
    /// <summary>
    /// The references for objects in the anxietyScenes
    /// </summary>
    [System.Serializable]
    public class AnxietyReferences
    {
        /// <summary>
        /// The elements having to do with the player
        /// </summary>
        [SerializeField] PlayerElements playerElements;
        /// <summary>
        /// The elements having to do with the player
        /// </summary>
        [System.Serializable]
        class PlayerElements
        {
            /// <summary>
            /// The GameObject for the player
            /// </summary>
            [SerializeField] internal GameObject _PlayerGameObject;
        }
        /// <summary>
        /// The GameObject for the player
        /// </summary>
        public static GameObject PlayerGameObject { get; internal set; }
        /// <summary>
        /// The PlayerMain component on the player
        /// </summary>
        public static PlayerMain PlayerScript { get; internal set; }

        /// <summary>
        /// The elements having to do with the enemies
        /// </summary>
        [SerializeField] EnemyElements enemyElements;
        /// <summary>
        /// The elements having to do with the enemies
        /// </summary>
        [System.Serializable]
        class EnemyElements
        {
            /// <summary>
            /// A list of all the enemies in the maze
            /// </summary>
            [SerializeField] internal GameObject[] _Enemies;
        }

        /// <summary>
        /// A list of all the enemies in the maze
        /// </summary>
        public static GameObject[] Enemies { get; internal set; }

        internal void SetUp()
        {

            PlayerGameObject = playerElements._PlayerGameObject;
            if (PlayerGameObject == null) return;
            PlayerScript = PlayerGameObject?.GetComponent<PlayerMain>();

            // If we have not been provided a list, go and search for it
            if (enemyElements._Enemies.Length != 0)
            {
                Enemies = enemyElements._Enemies;
            }
            else
            {
                Enemies = GameObject.FindGameObjectsWithTag("Enemy");
            }
        }
    }

    void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == GameManager.CurrentOfficeIndex)
        {
            officeReferences.SetUp();
        }
        else if (SceneManager.GetActiveScene().buildIndex == GameManager.CurrentMazeIndex)
        {
            anxietyReferences.SetUp();
        }

    }


}
