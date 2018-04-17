using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameData
{
    [Serializable]
    public enum GameResult
    {
        Tie = 0,
        Win = 1,
        Lose = -1
    } 
    public GameResult win;
    public string enemyName;
    public float roundTime;
    public static string saveLocation = "/Saves";
    public GameData()
    { }

    public GameData(GameResult win, string enemyName, float roundTime)
    {
        this.win = win;
        this.enemyName = enemyName;
        this.roundTime = roundTime;
    }

    public void Save()
    {
        XmlSerializer formatter = new XmlSerializer(typeof(GameData));
        var curDir = Directory.GetCurrentDirectory();
        var saveDir = curDir + saveLocation;
        if (!Directory.Exists(saveDir))
            Directory.CreateDirectory(saveDir);
        var name = String.Format(saveDir + "/gameData{0}.xml", DateTime.Now.Millisecond.ToString());
        using (FileStream fs = new FileStream(name, FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, this);
        }
    }

    public static List<GameData> GetSaves()
    {
        var files = new DirectoryInfo(Directory.GetCurrentDirectory() + "/Saves").GetFiles("*.xml").OrderByDescending((f) => f.LastWriteTime);
        XmlSerializer formatter = new XmlSerializer(typeof(GameData));
        List<GameData> gDataList = new List<GameData>();
        GameData gData;
        foreach (var file in files)
        {
            using (FileStream fs = new FileStream(file.FullName ,FileMode.Open))
            {
                try
                {
                    gData = (GameData)formatter.Deserialize(fs);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    continue;
                }
                gDataList.Add(gData);
            }
        }
        return gDataList;
    }
}
