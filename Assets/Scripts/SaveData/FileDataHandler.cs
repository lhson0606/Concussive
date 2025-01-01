using UnityEngine;
using System.IO;
public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = ""; 

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string dataPath = Path.Combine(this.dataDirPath, this.dataFileName);
        GameData gameData = new GameData();
        if (File.Exists(dataPath))
        {
            try
            {
                string dataAsJson = "";
                
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading data from " + dataPath + ": " + e.Message);
            }
        }

        return new GameData();
    }

    public void Save(GameData gameData)
    {
        string dataPath = Path.Combine(this.dataDirPath, this.dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));

            string dataToStore = JsonUtility.ToJson(gameData, true);

            using (FileStream fs = new FileStream(dataPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving data to " + dataPath + ": " + e.Message);
        }

    }
}
