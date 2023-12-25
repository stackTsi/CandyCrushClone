using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private Backgroundtile[,] breakableTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    private ScoreManager scoreManager;
    public int basePieceVal = 10;
    private int streakValue = 1;
    public float refillDelay = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTiles = new Backgroundtile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    public void GenerateBlankSpaces()
    {   //checking for all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {   //if the tile is blank
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                //set blank space flag at that position to true
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        //checking for all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if the tile is blank
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                //create a new breakable tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y); // take the current position
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity); // set the tile that position as breakable 
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<Backgroundtile>();
            }
        }
    }
    private void SetUp()
    {
        GenerateBreakableTiles();
        GenerateBlankSpaces();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);

                    GameObject backgroundtile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundtile.transform.parent = this.transform;
                    backgroundtile.name = "(" + i + "," + j + ")";
                    int dotToUse = UnityEngine.Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = UnityEngine.Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;

                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null & allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null & allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null & allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row] != null & allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if (dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return numberHorizontal == 5 || numberVertical == 5;

    }
    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //Make a color bomb
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Make an adjacent bomb
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //How many elements are in the matched pieces list from Findmatches?
            if (findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            //Does a tile be to break?
            if (breakableTiles[column, row] != null)
            {
                //take 1 damage if does
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceVal * streakValue);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }

    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int ii = 0; ii < height; ii++)
            {
                //if the current spot isn't blank and is empty
                if (!blankSpaces[i, ii] && allDots[i, ii] == null)
                {
                    //loop from the space above to the top of the column
                    for (int iii = ii + 1; iii < height; iii++)
                    {
                        if (allDots[i, iii] != null)
                        {
                            //move the founded dot to this empty space
                            allDots[i, iii].GetComponent<Dot>().row = ii;
                            //set that spot to be null
                            allDots[i, iii] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }
    // private IEnumerator DecreaseRowCo()
    // {
    //     int nullCount = 0;
    //     for (int i = 0; i < width; i++)
    //     {
    //         for (int j = 0; j < height; j++)
    //         {
    //             if (allDots[i, j] == null)
    //             {
    //                 nullCount++;
    //             }
    //             else if (nullCount > 0)
    //             {
    //                 allDots[i, j].GetComponent<Dot>().row -= nullCount;
    //                 allDots[i, j] = null;
    //             }
    //         }
    //         nullCount = 0;
    //     }
    //     yield return new WaitForSeconds(refillDelay * 0.5f);
    //     StartCoroutine(FillBoardCo());
    // }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = UnityEngine.Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = UnityEngine.Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        currentState = GameState.wait;
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.6f * refillDelay);
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
            Debug.Log("Deadlocked detected!");
        }
        currentState = GameState.move;
        streakValue = 1;
    }

    //Deadlock checker
    private void SwitchPieces(int column, int row, Vector2Int direction)
    {
        //Take the first piece and save it in a holder
        GameObject holder = allDots[column + direction.x, row + direction.y];
        //switching the first dot to be the second position
        allDots[column + direction.x, row + direction.y] = allDots[column, row];
        //Set the first dot to be the second dot
        allDots[column, row] = holder;
    }
    //Deadlock checker for width and height matches
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int ii = 0; ii < height; ii++)
            {
                if (allDots[i, ii] != null && !blankSpaces[i, ii])
                {   //make sure the one and two to the right is in the board
                    if (i < width - 2)
                    {
                        //check if the dots to the right and the dot next to it on the right exist
                        if (allDots[i + 1, ii] != null && allDots[i + 2, ii] != null)
                        {
                            if (allDots[i + 1, ii].tag == allDots[i, ii].tag
                            && allDots[i + 2, ii].tag == allDots[i, ii].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (ii < height - 2)
                    {
                        //check if the dots above(or below) exist
                        if (allDots[i, ii + 1] != null && allDots[i, ii + 2] != null)
                        {
                            if (allDots[i, ii + 1].tag == allDots[i, ii].tag
                            && allDots[i, ii + 2].tag == allDots[i, ii].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }
    //Deadlock checker
    public bool SwitchAndCheck(int column, int row, Vector2Int direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    //Deadlock checker
    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int ii = 0; ii < height; ii++)
            {
                if (allDots[i, ii] != null)
                {
                    // row check
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, ii, Vector2Int.right))
                        {
                            return false;
                        }
                    }
                    // column check
                    if (ii < height - 1)
                    {
                        if (SwitchAndCheck(i, ii, Vector2Int.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(.5f);
        //Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();

        //Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for (int ii = 0; ii < height; ii++)
            {
                if (allDots[i, ii] != null)
                {
                    newBoard.Add(allDots[i, ii]);
                }
            }
        }
        yield return new WaitForSeconds(.5f);
        //for every spot on the board
        for (int i = 0; i < width; i++)
        {
            for (int ii = 0; ii < height; ii++)
            {
                //if this spot isn't blank
                if (!blankSpaces[i, ii])
                {
                    //Pick a random number
                    int pieceToUse = UnityEngine.Random.Range(0, newBoard.Count);

                    int maxIterations = 0;
                    while (MatchesAt(i, ii, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = UnityEngine.Random.Range(0, newBoard.Count - 1);
                        maxIterations++;
                    }
                    //make a container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    //assign the column and row position to the piece
                    piece.column = i;
                    piece.row = ii;
                    //fill in the dots array with new pieces using the assigned placement above
                    allDots[piece.column, piece.row] = newBoard[pieceToUse];
                    //and remove from the piece
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }

        //check for deadlock from the new shuffled board
        if (IsDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
    }
}
