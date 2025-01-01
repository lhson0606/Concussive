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
        // Add code here to reset all data to default values
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
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
        if (this.gameData == null)
        {
            Debug.LogWarning("No game data to save.");
            return;
        }
        foreach (IDataPersistent dataPersistentObject in this.dataPersistentObjects)
        {
            dataPersistentObject.SaveData(ref this.gameData);
        }   

        this.fileDataHandler.Save(this.gameData);
    }

    private void Start()
    {

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

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

    private void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public bool hasGameData()
    {
        return this.gameData != null;
    }
}
