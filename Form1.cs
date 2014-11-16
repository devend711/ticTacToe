using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D; // For CoordinateSpace
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        private const float clientSize = 100;
        private const float lineLength = 80;
        private const float block = lineLength / 3;
        private const float offset = 10;
        private const float delta = 5;
        private float scale;            // Current scale factor

        private AI ai = new AI();
        private AI.CellSelection player = AI.CellSelection.X;
        private AI.CellSelection computer = AI.CellSelection.O;


        public Form1()
        {
            InitializeComponent();
            ResizeRedraw = true;
        }

        private void newGame()
        {
            ai.newGame();
            this.Invalidate();
        }

        private void alertWinner() {
            if (ai.didIWin(computer))
            {
                MessageBox.Show("You lost!");
                this.computerStartsToolStripMenuItem.Enabled = true;
            }
            else if (ai.didIWin(player))
            {
                MessageBox.Show("You won!");
                this.computerStartsToolStripMenuItem.Enabled = true;
            }
            else if (ai.numSpacesLeft()==0)
            {
                MessageBox.Show("It's a draw!");
                this.computerStartsToolStripMenuItem.Enabled = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            ApplyTransform(g);

            if (ai.numSpacesLeft() < 9)
            {
                this.computerStartsToolStripMenuItem.Enabled = false;
            }

            // Draw board
            g.DrawLine(Pens.Black, block, 0, block, lineLength);
            g.DrawLine(Pens.Black, 2 * block, 0, 2 * block, lineLength);
            g.DrawLine(Pens.Black, 0, block, lineLength, block);
            g.DrawLine(Pens.Black, 0, 2 * block, lineLength, 2 * block);
            AI.CellSelection[,] grid = ai.currentGrid();

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (grid[i, j] == AI.CellSelection.O)
                    {
                        DrawO(i, j, g);

                    }
                    else if (grid[i, j] == AI.CellSelection.X)
                    {
                        DrawX(i, j, g);
                    }
                }
            }
        }

        private void ApplyTransform(Graphics g)
        {
            scale = Math.Min(ClientRectangle.Width / clientSize, ClientRectangle.Height / clientSize);

            if (scale == 0f) return;
            g.ScaleTransform(scale, scale);
            g.TranslateTransform(offset, offset);
        }

        private void DrawX(int i, int j, Graphics g)
        {
            g.DrawLine(Pens.Black, (i * block) + delta, (j * block) + delta, (i * block) + block - delta, (j * block) + block - delta);
            g.DrawLine(Pens.Black, (i * block) + block - delta, (j * block) + delta, (i * block) + delta, (j * block) + block - delta);
        }

        private void DrawO(int i, int j, Graphics g)
        {
            g.DrawEllipse(Pens.Black, (i * block) + delta, (j * block) + delta, block - (2 * delta), block - (2 * delta));
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = CreateGraphics();
            ApplyTransform(g);
            PointF[] p = { new Point(e.X, e.Y) };
            g.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, p);

            if (ai.numSpacesLeft()>0 && !ai.gameOver())
            {
                if (p[0].X < 0 || p[0].Y < 0) return;
                int i = (int)(p[0].X / block);
                int j = (int)(p[0].Y / block);
                if (i > 2 || j > 2) return;

                // Only allow setting empty cells
                if (ai.spaceIsEmpty(i, j))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        ai.playerMoveAt(i, j); // pass cell coordinates to AI
                        this.Invalidate();
                        alertWinner();
                        if (!ai.gameOver())
                        {
                            ai.takeTurn();
                            this.Invalidate();
                            alertWinner();
                        }
                    }
                }
                else // space is taken
                {
                    MessageBox.Show("Bad move!");
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.newGame();
        }

        private void computerStartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.newGame();
            ai.takeTurn();
        }
    }
}