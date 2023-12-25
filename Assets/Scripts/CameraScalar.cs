using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScalar : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float padding = 2;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    // dynamic camera scaling by board's width and height
    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new(x / 2, y / 2, cameraOffset);
        transform.position = tempPosition;

        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = board.width / 2 + padding;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
