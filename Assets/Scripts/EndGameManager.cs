using System;
using TMPro;
using UnityEngine;

public enum GameType
{
    Moves,
    Time
}
[Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{
    public EndGameRequirements requirements;
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject winPanel;
    public GameObject tryAgainPanel;
    public TextMeshProUGUI counter;
    private Board board;
    public int currentCounterValue;
    private float timerSeconds;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
    }
    void SetGameType()
    {
        if (board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }

        }
    }
    void SetupGame()
    {

        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterVal()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        winPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        //for fading out animation
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("You Lose!");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        //for fading out animation
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        //if in time game type
        if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            //decrease timer by how much time pass since [last frame]
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterVal();
                timerSeconds = 1; // reset timer
            }
        }
    }
}
