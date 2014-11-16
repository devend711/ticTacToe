using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class AI
    {
        public enum CellSelection { N, O, X };
        private CellSelection[,] grid = new CellSelection[3, 3];
        private CellSelection playerSelection = CellSelection.X;
        private CellSelection computerSelection = CellSelection.O;

        public AI(){
            newGame();
        }
        
        public CellSelection[,] currentGrid()
        {
            return this.grid;
        }

        public Boolean gameOver()
        {
            if (didIWin(computerSelection) || didIWin(playerSelection) || numSpacesLeft()==0)
            {
                return true;
            }
            return false;
        }

        public void newGame() {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    grid[i, j] = CellSelection.N;
                }
            }
        }

        public Boolean spaceIsEmpty(int i, int j)
        {
            return (grid[i, j] == CellSelection.N);
        }

        public int numSpacesLeft()
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (grid[i,j] != playerSelection && grid[i,j] != computerSelection)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private bool computerMoveAt(int i, int j) // pass the grid by reference so we can change it
        {
            if (grid[i, j] == CellSelection.N)
            {
                grid[i, j] = computerSelection;
                return true;
            }
            return false;
        }

        public bool playerMoveAt(int i, int j) // pass the grid by reference so we can change it
        {
            if (grid[i, j] == CellSelection.N)
            {
                grid[i, j] =  playerSelection;
                return true;
            }
            return false;
        }

        private int minimax(CellSelection player) // recursivley find a score
        {
            if (didIWin(player)) {
                return 1;
            } else if (didIWin(opponent(player))) {
                return -1;
            }

            int bestRow = -1;
            int bestCol = -1;
            int score = -2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (spaceIsEmpty(i,j))
                    {
                        grid[i, j] = player; // try this move
                        int tempScore = -minimax(opponent(player)); // recursive call
                        if (tempScore > score) {
                            score = tempScore;
                            bestRow = i;
                            bestCol = j;
                        }
                        grid[i,j] = CellSelection.N; // undo the move
                    }
                }
            }
            if (bestRow == -1 && bestCol == -1) {
                return 0; // didnt find a good move
            }
            return score;
        }

        public void takeTurn() // recursivley find a best move
        {
            if (numSpacesLeft() == 0)
            {
                return;
            }
            int row = -1;
            int col = -1;
            int score = -2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (spaceIsEmpty(i,j))
                    {
                        grid[i, j] = computerSelection;
                        int tempScore = -minimax(playerSelection);
                        grid[i, j] = CellSelection.N;
                        if (tempScore > score)
                        {
                            score = tempScore;
                            row = i;
                            col = j;
                        }
                    }
                }
            }
            computerMoveAt(row, col);
        }

        private CellSelection opponent(CellSelection player)
        {
            return (player == playerSelection ? computerSelection : playerSelection);
        }

        public Boolean didIWin(CellSelection marker) // returns N if nobody won
        {
            int rowScore;
            int colScore;

            // check rows and columns
            for (int i = 0; i < 3; i++)
            {
                rowScore = 0;
                colScore = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (grid[i, j] == marker)
                    {
                        rowScore += 1;
                    }
                    if (grid[j, i] == marker)
                    {
                        colScore += 1;
                    }
                }
                if (rowScore == 3 || colScore == 3)
                {
                    return true;
                }
            }

            // check diagonals
            if (grid[1, 1] == marker)
            {
                if (grid[0,0] == marker && grid[2,2] == marker)
                {
                    return true;
                }
                if (grid[0, 2] == marker && grid[2, 0] == marker)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
