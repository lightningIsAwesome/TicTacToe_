using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameDataMessage : MonoBehaviour
{
    [SerializeField] Text roundTime;
    [SerializeField] Text result;
    [SerializeField] Text enemy;

    public void Init(GameData data)
    {
        this.roundTime.text = data.roundTime.ToString(".00");
        if (data.win == GameData.GameResult.Win)
        {
            result.text = "Win";
            result.color = Color.green;
        }
        else if (data.win == GameData.GameResult.Lose)
        {
            result.text = "Lose";
            result.color = Color.red;
        }
        else
            result.text = "Tie";
        this.enemy.text = data.enemyName;
    }
}
