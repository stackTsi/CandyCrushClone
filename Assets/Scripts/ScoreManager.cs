using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public TextMeshProUGUI scoreText;
    public int score;
    public Image scoreBar;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        LoadBar();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }
    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
       //Debug.Log(scoreBar);
        LoadBar();
    }
    private void LoadBar()
    {
        if (board != null && scoreBar != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];

        }
    }
}
