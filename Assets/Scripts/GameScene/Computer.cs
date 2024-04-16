using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.OnComputerTurn += OnComputerTurn;
    }

    private void OnComputerTurn()
    {
        Debug.Log("OnComputerTurn");
    }

    private void OnDisable()
    {
        GameManager.OnComputerTurn -= OnComputerTurn;
    }
}
