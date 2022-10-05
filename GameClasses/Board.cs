using System;

namespace GameClasses;

public class Board
{
    public bool[,] board;

    public Board()
    {
        board = new bool[10, 10];
    }

    public bool CheckShipPosition(int startX, int startY, int endX, int endY)
    {
        Tuple<int, int>[] moves =
        {
            new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, -1), new Tuple<int, int>(0, 1)
        };
        for (int i = startX; i <= endX; ++i)
        {
            for (int j = startY; i <= endY; ++j)
            {
                for (int k = 0; k < 4; ++k)
                {
                    int newX = i + moves[k].Item1;
                    int newY = j + moves[k].Item2;
                    if (newX >= 0 && newY >= 0 && newX < 10 && newY < 10 && board[newX, newY])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void AddShip(int startX, int startY, int endX, int endY)
    {
        if (!CheckShipPosition(startX, startY, endX, endY))
        {
            return;
        }

        for (int i = startX; i <= endX; ++i)
        {
            for (int j = startY; i <= endY; ++j)
            {
                board[i, j] = true;
            }
        }
    }

    public void DeleteShip(int x, int y)
    {
        int[] movesX = { 1, -1, 0, 0 };
        int[] movesY = { 0, 0, 1, -1 };
        for (int i = 0; i < 4; ++i)
        {
            int currentX = x;
            int currentY = y;
            while (currentX >= 0 && currentX < 10 && currentY >= 0 && currentY < 10 && board[currentX, currentY])
            {
                board[currentX, currentY] = false;
                currentX += movesX[i];
                currentY += movesY[i];
            }
        }
    }

    public static void Main(string[] args) {}
}