using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSceneManager : MonoBehaviour
{
    private void Awake()
    {
        GameSetting.Instance.playerValue = GameSetting.PlayerValue.None;
    }

    public void SetPlayerValueToSun()
    {
        GameSetting.Instance.playerValue = GameSetting.PlayerValue.Sun;
    }

    public void SetPlayerValueToMoon()
    {
        GameSetting.Instance.playerValue = GameSetting.PlayerValue.Moon;
    }

    public void PlayGame()
    {
        if(GameSetting.Instance.playerValue != GameSetting.PlayerValue.None) SceneManager.LoadScene("GameScene");
    }

    public void BackToHome()
    {
        SceneManager.LoadScene("TopScene");
    }
}
