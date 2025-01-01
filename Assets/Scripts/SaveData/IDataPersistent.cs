using UnityEngine;

public interface IDataPersistent
{
    void LoadData(GameData gameData);
    void SaveData(ref GameData gameData);
}
