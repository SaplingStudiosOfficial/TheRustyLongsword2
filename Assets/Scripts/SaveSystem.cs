using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem 
{
    
    public static void SavePlayer(PlayerController player)
    {

        
        SaveManagerScript data = new SaveManagerScript(player);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/newerSaveData.json", json);
        

        
        

        
    }


    public static SaveManagerScript LoadData()
    {
        

        string path = Application.persistentDataPath + "/newerSaveData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/newerSaveData.json");
            SaveManagerScript data = JsonUtility.FromJson<SaveManagerScript>(json);

            return data;
        } 
        else
        {
            return null;
        }
    }
}
