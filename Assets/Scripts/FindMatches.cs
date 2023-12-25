using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }
        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }
        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }
        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }
        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new();
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }
        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }
        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }
        return currentDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }
    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));
                                currentMatches.Union(IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));
                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));
                                currentMatches.Union(IsAdjacentBomb(upDotDot, currentDotDot, downDotDot));
                                GetNearbyPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }

            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int ii = 0; ii < board.height; ii++)
            {
                //check if that piece exists
                if (board.allDots[i, ii] != null)
                {
                    //check the tag on that dot
                    if (board.allDots[i, ii].tag == color)
                    {
                        //set that dot to be matched
                        board.allDots[i, ii].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }
    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int ii = row - 1; ii <= row + 1; ii++)
            {
                //Check if piece is inside the board
                if (i >= 0 && i < board.width && ii >= 0 && ii < board.height)
                {
                    if (board.allDots[i, ii] != null)
                    {
                        Dot dot = board.allDots[i, ii].GetComponent<Dot>();
                        //check for other type of bomb for bomb chaining
                        if (dot.isColumnBomb)
                        {
                            dot.isColumnBomb = false;
                            dots.Union(GetColumnPieces(i)).ToList();
                        }
                        else if (dot.isRowBomb)
                        {
                            dot.isRowBomb = false;
                            dots.Union(GetRowPieces(ii)).ToList();
                        }
                        else if (dot.isAdjacentBomb)
                        {
                            dot.isAdjacentBomb = false;
                            dots.Union(GetAdjacentPieces(i, ii)).ToList();
                        }
                        {
                            dots.Add(board.allDots[i, ii]);
                            dot.isMatched = true;
                        }
                    }

                }
            }
        }
        return dots;
    }
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                //check for other type of bomb for bomb chaining
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot != null)
                {
                    if (dot.isRowBomb)
                    {
                        dot.isRowBomb = false;
                        dots.Union(GetRowPieces(i)).ToList();
                    }
                    else if (dot.isAdjacentBomb)
                    {
                        dot.isAdjacentBomb = false;
                        dots.Union(GetAdjacentPieces(column, i)).ToList();
                    }
                    else
                    {
                        dots.Add(board.allDots[column, i]);
                        dot.isMatched = true;
                    }
                }

            }
        }
        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot != null)
                {
                    //check for other type of bomb for bomb chaining
                    if (dot.isColumnBomb)
                    {
                        dot.isColumnBomb = false;
                        dots.Union(GetColumnPieces(i)).ToList();
                    }
                    else if (dot.isAdjacentBomb)
                    {
                        dot.isAdjacentBomb = false;
                        dots.Union(GetAdjacentPieces(i, row)).ToList();
                    }
                    else
                    {
                        dots.Add(board.allDots[i, row]);
                        dot.isMatched = true;
                    }
                }
            }
        }
        return dots;
    }

    public void CheckBombs()
    {
        //Did the player move something ?
        if (board.currentDot != null)
        {
            //is the piece they move matched ?(currentDot)
            if (board.currentDot.isMatched)
            {
                //make it unmatched
                board.currentDot.isMatched = false;
                //Decide what kind of bomb to make
                /*
                /*
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    //make a row bomb
                    board.currentDot.MakeRowBomb();
                }
                else if (typeOfBomb >= 50)
                {
                    //make a column bomb
                    board.currentDot.MakeColumnBomb();
                }
                */
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                || board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)
                {
                    board.currentDot.MakeColumnBomb();
                }
                else
                {
                    board.currentDot.MakeRowBomb();
                }
            }
            //is the other piece matched? (otherDot)
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                //is the other dot matched?
                if (otherDot.isMatched)
                {
                    //Make it unmatched
                    otherDot.isMatched = false;
                    /*
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {   
                        //make a column bomb
                        otherDot.MakeColumnBomb();
                    }*/
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                        || board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)
                    {
                        otherDot.MakeColumnBomb();
                    }
                    else
                    {
                        otherDot.MakeRowBomb();
                    }
                }
            }
        }
    }

}
