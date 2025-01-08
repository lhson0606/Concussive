using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DataPersistentManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool debugMode = false;
    [Header("Data File Path")]
    [SerializeField] private string fileName = "";
    private GameData gameData;
    private List<IDataPersistent> dataPersistentObjects;
    private FileDataHandler fileDataHandler;
    public static DataPersistentManager instance { get; private set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Another instance of DataPersistentManager already exists. Destroying this instance.");
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, this.fileName);
    }
    
    public void NewGame()
    {
        Debug.Log("Creating new game data.");

        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        Debug.Log("Loading game data.");

        this.gameData = this.fileDataHandler.Load();

        if(this.gameData == null && debugMode)
        {
            Debug.LogWarning("No game data found. Creating new game data.");
            NewGame();
        }
        if (this.gameData == null)
        {
            Debug.LogWarning("No game data to load.");
            return;
        }

        foreach (IDataPersistent dataPersistentObject in this.dataPersistentObjects)
        {
            dataPersistentObject.LoadData(this.gameData);
        }
    }

    public void SaveGame()
    {
        Debug.Log("Saving game data.");
        if (this.gameData == null)
        {
            Debug.LogWarning("No game data to save.");
            return;
        }
        foreach (IDataPersistent dataPersistentObject in this.dataPersistentObjects)
        {
            dataPersistentObject.SaveData(this.gameData);
        }   

        this.fileDataHandler.Save(this.gameData);
    }

    private void Start()
    {

    }

    // private void OnApplicationQuit()
    // {
    //     SaveGame();
    // }

    private List<IDataPersistent> FindAllDataPersistentObjects()
    {
        IEnumerable<IDataPersistent> dataPersistentObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistent>();
        
        
        return new List<IDataPersistent>(dataPersistentObjects);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistentObjects = FindAllDataPersistentObjects();
        LoadGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public bool hasGameData()
    {
        return this.gameData != null;
    }

    public GameData getGameData()
    {
        return this.gameData;
    }
}
