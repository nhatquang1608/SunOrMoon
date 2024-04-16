using System.Collections;
using UnityEngine;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action OnComputerTurn;
    public enum Turn
    {
        Sun,
        Moon
    }

    public Turn turn;

    public bool isGameOver;
    [SerializeField] private TileBoard board;
    [SerializeField] private CanvasGroup gameOver;

    [SerializeField] private GameObject sunWin;
    [SerializeField] private GameObject moonWin;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject sunGoFirst;
    [SerializeField] private GameObject moonGoFirst;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        isGameOver = false;

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        sunWin.SetActive(false);
        moonWin.SetActive(false);

        board.ClearBoard();
        board.CreateTiles();
        board.enabled = true;

        System.Random rnd = new System.Random();
        int randomIndex = rnd.Next(2);
        turn = (Turn)randomIndex;

        if(turn == Turn.Sun)
        {
            if(GameSetting.Instance.playerValue == GameSetting.PlayerValue.Sun) GameSetting.Instance.playerType = GameSetting.PlayerType.Human;
            else GameSetting.Instance.playerType = GameSetting.PlayerType.Computer;

            StartCoroutine(ShowInitPopup(sunGoFirst));
        }
        else if(turn == Turn.Moon)
        {
            if(GameSetting.Instance.playerValue == GameSetting.PlayerValue.Sun) GameSetting.Instance.playerType = GameSetting.PlayerType.Computer;
            else GameSetting.Instance.playerType = GameSetting.PlayerType.Human;

            StartCoroutine(ShowInitPopup(moonGoFirst));
        }
    }

    private IEnumerator ShowInitPopup(GameObject goFirst)
    {
        popup.SetActive(true);
        goFirst.SetActive(true);

        yield return new WaitForSeconds(3);

        popup.SetActive(false);
        goFirst.SetActive(false);
    }

    public void GameOver(bool sun)
    {
        board.enabled = false;
        gameOver.interactable = true;

        if(sun) sunWin.SetActive(true);
        else moonWin.SetActive(true);

        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void SwapTurn()
    {
        if(turn == Turn.Sun) turn = Turn.Moon;
        else turn = Turn.Sun;

        if(GameSetting.Instance.playerType == GameSetting.PlayerType.Human) 
        {
            GameSetting.Instance.playerType = GameSetting.PlayerType.Computer;
            OnComputerTurn?.Invoke();
        }
        else GameSetting.Instance.playerType = GameSetting.PlayerType.Human;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
