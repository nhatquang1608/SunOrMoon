using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public static GameSetting Instance;

    public enum PlayerType
    {
        Human,
        Computer
    }

    public enum PlayerValue
    {
        None,
        Sun,
        Moon
    }

    public PlayerType playerType;
    public PlayerValue playerValue;

    public void Awake()
    {
        if (Instance != null) 
        {
            DestroyImmediate(gameObject);
        } 
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
