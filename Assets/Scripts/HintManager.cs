using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds <= 0 && currentHint == null)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;
        }
    }
    //find all possible matches on the board
    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int ii = 0; ii < board.height; ii++)
            {
                if (board.allDots[i, ii] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, ii, Vector2Int.right))
                        {
                            possibleMoves.Add(board.allDots[i, ii]);
                        }
                    }
                    if (ii < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, ii, Vector2Int.up))
                        {
                            possibleMoves.Add(board.allDots[i, ii]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }
    //pick one of the matches randomly
    GameObject RandomPickOne()
    {
        List<GameObject> possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }
    //create the hint of the chosen match
    private void MarkHint()
    {
        GameObject move = RandomPickOne();
        if (move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }
    //destroy hint
    public void DestroyHint()
    {
        if (currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
