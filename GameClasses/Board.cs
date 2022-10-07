using System;

namespace GameClasses;

public class Board
{
    /// <summary>
    /// Class of a visible board. Can add and delete ship and check if we can add ship on particular coords
    /// bool CheckShipPosition(int startX, int startY, int endX, int endY):
    /// returns true if we can add ship when up-left coord is (startX, startY) and down-right is (endX, endY)
    /// bool AddShip(int startX, int startY, int endX, int endY)
    /// tries add ship with up-left coord is (startX, startY) and down-right is (endX, endY)
    /// If it's possible, it do it and returns true. Else do nothing and returns false
    /// int DeleteShip(int x, int y) - tries to delete ship, one of the coords of that is (x, y)
    /// returns 0 if it is not possible, else returns length of deleted ship
    /// </summary>
    public bool[,] board; // board[i, j] == true if there is a part of a ship in (i, j) tile

    public Board()
    {
        board = new bool[10, 10];
    }

    public bool CheckShipPosition(int startX, int startY, int endX, int endY)
    {
        Tuple<int, int>[] moves =
        {
            new Tuple<int, int>(1, 0), new Tuple<int, int>(-1, 0), new Tuple<int, int>(0, -1),
            new Tuple<int, int>(0, 1), new Tuple<int, int>(-1, 1), new Tuple<int, int>(-1, -1),
            new Tuple<int, int>(1, -1), new Tuple<int, int>(1, 1), new Tuple<int, int>(0, 0)
        }; // Neighbours of the particular tile
        for (int i = startX; i <= endX; ++i)
        {
            for (int j = startY; j <= endY; ++j)
            {
                // Runs for every ship's coord
                for (int k = 0; k < 9; ++k)
                {
                    int newX = i + moves[k].Item1;
                    int newY = j + moves[k].Item2; // Neighbour coords
                    if (newX >= 0 && newY >= 0 && newX < 10 && newY < 10 && board[newX, newY])
                    {
                        return false; // There are other ship
                    }

                    if (i < 0 || j < 0 || i >= 10 || j >= 10)
                    {
                        return false; // The ship's coord is out of the board
                    }
                }
            }
        }

        return true;
    }

    public bool AddShip(int startX, int startY, int endX, int endY)
    {
        if (!CheckShipPosition(startX, startY, endX, endY))
        {
            return false; // Ship cannot be here
        }

        for (int i = startX; i <= endX; ++i)
        {
            for (int j = startY; j <= endY; ++j)
            {
                board[i, j] = true; // Add a ship on the board
            }
        }

        return true;
    }

    public int DeleteShip(int x, int y)
    {
        int result = 0;
        if (!board[x, y])
        {
            return 0; // There's no ship in this tile
        }
        int[] movesX = { 1, -1, 0, 0 };
        int[] movesY = { 0, 0, 1, -1 }; // Arrays set directions
        for (int i = 0; i < 4; ++i)
        {
            int currentX = x;
            int currentY = y; // coords, that will be run for particular direction
            while (currentX >= 0 && currentX < 10 && currentY >= 0 && currentY < 10 &&
                   (board[currentX, currentY] || (currentX == x && currentY == y))) 
            {
                // Coords are valid and there is ship (or we already delete this part of ship)
                if (board[currentX, currentY])
                {
                    result++; // If we delete new tile, increase length of the ship
                }
                board[currentX, currentY] = false; // Delete ship's part
                currentX += movesX[i];
                currentY += movesY[i];
            }
        }

        return result;
    }

    public static void Main(string[] args)
    {
    }
}