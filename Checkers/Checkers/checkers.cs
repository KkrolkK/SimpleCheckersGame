using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal class Checkers
    {
        public Checkers() {  }

        private TableLayoutPanel board;
        private Label label;

        private readonly string whiteIcon = "⚪";
        private readonly string blackIcon = "⚫";
        private readonly string whiteKingIcon = "⬜";
        private readonly string blackKingIcon = "⬛";
        private Color squaresColor = Color.Pink;

        private HashSet<string> blacks = new();
        private HashSet<string> whites = new();
        private HashSet<string> blackKings = new();
        private HashSet<string> whiteKings = new();
        private HashSet<string> buttonsGreen = new(); // set of buttons that are green

        private string[] possiblePosition = new string[2];
        private int BoardRows, BoardCols;
        private string oldPosition; //position the to remove the piece
        private bool turn = true; // true = black turn / false = white turn

        public TableLayoutPanel CreateBoard(int boardRows, int boardColumns)
        {
            BoardCols = boardColumns;
            BoardRows = boardRows;

            board = new TableLayoutPanel();
            board.Location = new Point(100, 100);
            board.RowCount = boardRows;
            board.ColumnCount = boardColumns;
            board.AutoSize = true;
            bool isBlack = false;

            for (int y = 0; y < boardRows; y++)
            {
                isBlack = !isBlack;
                for (int x = 0; x < boardColumns; x++)
                {
                    Button button = new();
                    button.Size = new Size(100, 100);
                    button.Font = new Font(FontFamily.GenericSansSerif, 30);
                    button.Click += Button_Click;
                    string position = $"[{y},{x}]";
                    button.Name = position;

                    if (y % 2 == 0 && x % 2 ==0)
                    {
                        AddPiecesAndColor(y, button, position);
                    }
                    if (y % 2 != 0 && x % 2 !=0)
                    {
                        AddPiecesAndColor(y, button, position);
                    }

                    int[] buttonIndex = { y, x };
                    button.Tag = buttonIndex;

                    board.Controls.Add(button);
                }
            }
            return board;
        }
        public Button CreateButton()
        {
            Button btn = new();
            btn.Text = "Restart";
            btn.Location = new Point(100, 20);
            btn.Font = new Font(FontFamily.GenericSansSerif, 20);
            btn.AutoSize = true;
            btn.BackColor = squaresColor;
            btn.Click += BtnRestart_Click;

            return btn;
        }
        public Label CreateLabel()
        {
            label = new Label();
            label.Text = $"Turn: Black    {blackIcon}: {blacks.Count}     {whiteIcon}: {whites.Count}       {blackKingIcon}: {blackKings.Count}       {whiteKingIcon}: {whiteKings.Count}";
            label.Location = new Point(290, 30);
            label.Font = new Font(FontFamily.GenericSansSerif, 15);
            label.AutoSize = true;
            return label;
        }
        private void BtnRestart_Click(object? sender, EventArgs e)
        {
            Restart();
        }
        private void Button_Click(object? sender, EventArgs e)
        {
            Button? btnClicked = (Button)sender;
            string btnName = btnClicked.Name;

            if ((turn && blacks.Contains(btnName)) || (turn && blackKings.Contains(btnName)) || (!turn && whites.Contains(btnName)) || (!turn && whiteKings.Contains(btnName)))
            {
                oldPosition = btnName;
                FirstClick(btnClicked);
            }
            else if (buttonsGreen.Contains(btnName))
            {
                SecondClick(btnClicked);
            }
        }
        private void FirstClick(Button btn)
        {
            string name = btn.Name;
            int y = Int32.Parse(name[1].ToString());
            int x = Int32.Parse(name[3].ToString());

            DeleteGreenButtons();

            if (turn && blacks.Contains(name))
            {
                possiblePosition[0] = $"[{y + 1},{x + 1}]";
                possiblePosition[1] = $"[{y + 1},{x - 1}]";
                GreenFoward(y, x, blacks, whites, blackKings, whiteKings);
            }
            else if (turn && blackKings.Contains(name))
            {
                possiblePosition[0] = $"[{y + 1},{x + 1}]";
                possiblePosition[1] = $"[{y + 1},{x - 1}]";
                GreenFoward(y, x, blacks, whites, blackKings, whiteKings);
                possiblePosition[0] = $"[{y - 1},{x + 1}]";
                possiblePosition[1] = $"[{y - 1},{x - 1}]";
                GreenBack(y, x, blacks, whites, blackKings, whiteKings);
            }
            else if (!turn && whiteKings.Contains(name))
            {
                possiblePosition[0] = $"[{y - 1},{x + 1}]";
                possiblePosition[1] = $"[{y - 1},{x - 1}]";
                GreenBack(y, x, whites, blacks, whiteKings, blackKings);
                possiblePosition[0] = $"[{y + 1},{x + 1}]";
                possiblePosition[1] = $"[{y + 1},{x - 1}]";
                GreenFoward(y, x, whites, blacks, whiteKings, blackKings);
            }
            else if (!turn && whites.Contains(name))
            {
                possiblePosition[0] = $"[{y - 1},{x + 1}]";
                possiblePosition[1] = $"[{y - 1},{x - 1}]";
                GreenBack(y, x, whites, blacks, whiteKings, blackKings);
            }
        }
        private void SecondClick(Button btn)
        {
            string btnName = btn.Name;
            int y = Int32.Parse(btnName[1].ToString());
            int x = Int32.Parse(btnName[3].ToString());
            int y1 = Int32.Parse(oldPosition[1].ToString());
            int x1 = Int32.Parse(oldPosition[3].ToString());
            int difY = y - y1, difX = x - x1;

            if (turn)
            {
                if (y == (BoardRows - 1) || blackKings.Contains(oldPosition))
                {
                    if (blacks.Contains(oldPosition))
                    {
                        blacks.Remove(oldPosition);
                    }
                    MovePiece(btnName, blackKings, blackKingIcon);
                }
                else
                {
                    MovePiece(btnName, blacks, blackIcon);
                }
                turn = !turn;
                if (difX > 1)
                {
                    FindAndRemove(y, difY, (x - 1), whites, whiteKings, whiteIcon);
                }
                else if (difX < -1)
                {
                    FindAndRemove(y, difY, (x + 1), whites, whiteKings, whiteIcon);
                }
                label.Text = $"Turn: White    {blackIcon}: {blacks.Count}     {whiteIcon}: {whites.Count}       {blackKingIcon}: {blackKings.Count}       {whiteKingIcon}: {whiteKings.Count}";
            }
            else
            {
                if (y == 0 || whiteKings.Contains(oldPosition))
                {
                    if (whites.Contains(oldPosition))
                    {
                        whites.Remove(oldPosition);
                    }
                    MovePiece(btnName, whiteKings, whiteKingIcon);
                }
                else
                {
                    MovePiece(btnName, whites, whiteIcon);
                }
                turn = !turn;
                if (difX > 1)
                {
                    FindAndRemove(y, difY, (x - 1), blacks, blackKings, blackIcon);
                }
                else if (difX < -1)
                {
                    FindAndRemove(y, difY, (x + 1), blacks, blackKings, blackIcon);
                }
                label.Text = $"Turn: Black    {blackIcon}: {blacks.Count}     {whiteIcon}: {whites.Count}       {blackKingIcon}: {blackKings.Count}       {whiteKingIcon}: {whiteKings.Count}";
            }
            DeleteGreenButtons();
            CheckWin();
        }

        private void DeleteGreenButtons()
        {
            if (buttonsGreen.Count > 0)
            {
                foreach (string button in buttonsGreen)
                {
                        board.Controls[button].BackColor = squaresColor;
                }
                buttonsGreen.Clear();
            }
        }

        private void FindAndRemove(int y, int difY, int x1, HashSet<string> pieces, HashSet<string> kingPieces, string icon)
        {
            string aux;
            if (difY > 1)
            {
                aux = $"[{y - 1},{x1}]";
                if (pieces.Contains(aux))
                    PieceRemove(aux, pieces);
                else
                    KingToPiece(aux, pieces, kingPieces, icon);
            }
            else if (difY < -1)
            {
                aux = $"[{y + 1},{x1}]";
                if (pieces.Contains(aux))
                    PieceRemove(aux, pieces);
                else
                    KingToPiece(aux, pieces, kingPieces, icon);
            }
        }

        private void MovePiece(string btnName, HashSet<string> pieces, string icon)
        {
            pieces.Remove(oldPosition);
            board.Controls[oldPosition].Text = string.Empty;
            board.Controls[btnName].Text = icon;
            pieces.Add(btnName);
        }

        private void PieceRemove(string aux, HashSet<string> pieces)
        {
            pieces.Remove(aux);
            board.Controls[aux].Text = string.Empty;
        }

        private void KingToPiece(string aux, HashSet<string> pieces, HashSet<string> kingPieces, string icon)
        {
            pieces.Add(aux);
            kingPieces.Remove(aux);
            board.Controls[aux].Text = icon;
        }

        private void GreenFoward(int y, int x, HashSet<string> piecesTurn, HashSet<string> otherPieces, HashSet<string> kingTurn, HashSet<string> otherKing)
        {
            string possiblePosition2;
            for (int i = 0; i < 2; i++)
            {
                int y1 = y + 1, x1 = x + 1 + (i * -2);
                if (otherPieces.Contains(possiblePosition[i]) || otherKing.Contains(possiblePosition[i]))
                {
                    y1 = y + 2;
                    x1 = x + 2 + (i * -4);
                    possiblePosition2 = $"[{y1},{x1}]";
                    if (!blacks.Contains(possiblePosition2) && !whites.Contains(possiblePosition2) && !blackKings.Contains(possiblePosition2) && !whiteKings.Contains(possiblePosition2) && y1 >= 0 && y1 < BoardRows && x1 < BoardCols && x1 > -1)
                    {
                        MakeSquareGreen(possiblePosition2);
                    }
                }
                else if (y1 < BoardRows && y1 > -1 && x1 < (BoardCols) && x1 > -1 && !piecesTurn.Contains(possiblePosition[i]) && !kingTurn.Contains(possiblePosition[i]))
                {
                    MakeSquareGreen(possiblePosition[i]);
                }
            }
        }
        private void GreenBack(int y, int x, HashSet<string> piecesTurn, HashSet<string> otherPieces, HashSet<string> kingTurn, HashSet<string> otherKing)
        {
            string possiblePosition2;
            for (int i = 0; i < 2; i++)
            {
                int y1 = y - 1, x1 = x + 1 + (i * -2);
                if (otherPieces.Contains(possiblePosition[i]) || otherKing.Contains(possiblePosition[i]))
                {
                    y1 = y - 2;
                    x1 = x + 2 + (i * -4);
                    possiblePosition2 = $"[{y1},{x1}]";
                    if (!blacks.Contains(possiblePosition2) && !whites.Contains(possiblePosition2) && !blackKings.Contains(possiblePosition2) && !whiteKings.Contains(possiblePosition2) && y1 >= 0 && y1 < BoardRows && x1 < BoardCols && x1 > -1)
                    {
                        MakeSquareGreen(possiblePosition2);
                    }
                }
                else if (y1 < BoardRows && y1 > -1 && x1 < (BoardCols) && x1 > -1 && !piecesTurn.Contains(possiblePosition[i]) && !kingTurn.Contains(possiblePosition[i]))
                {
                    MakeSquareGreen(possiblePosition[i]);
                }
            }
        }
        private void MakeSquareGreen(string green)
        {
            buttonsGreen.Add(green);
            board.Controls[green].BackColor = Color.PaleGreen;
        }
        private void AddPiecesAndColor(int y, Button button, string position)
        {
            button.BackColor = squaresColor;
            if (blacks.Count <= 12 && y < 3)
            {
                button.Text = blackIcon;
                blacks.Add(position);
            }
            else if (whites.Count <= 12 && y > 4)
            {
                button.Text = whiteIcon;
                whites.Add(position);
            }
        }
        private void CheckWin()
        {
            if(blacks.Count == 0 && blackKings.Count == 0)
            {
                MessageBox.Show($"WHITE WON!");
                Restart();
            }
            else if (whites.Count == 0 && whiteKings.Count == 0)
            {
                MessageBox.Show($"BLACK WON!");
                Restart();
            }
        }
        private void Restart()
        {
            string position;
            int x, y;
            turn = true;
            whites.Clear();
            blacks.Clear();
            blackKings.Clear();
            whiteKings.Clear();
            buttonsGreen.Clear();

            foreach (Button btn in board.Controls)
            {
                btn.Text = string.Empty;
                position = btn.Name;
                y = Int32.Parse(position[1].ToString());
                x = Int32.Parse(position[3].ToString());
                if (y % 2 == 0 && x % 2 == 0)
                {
                    AddPiecesAndColor(y, btn, position);
                }
                if (y % 2 != 0 && x % 2 != 0)
                {
                    AddPiecesAndColor(y, btn, position);
                }
            }
            label.Text = $"Turn: Black    {blackIcon}: {blacks.Count}     {whiteIcon}: {whites.Count}       {blackKingIcon}: {blackKings.Count}       {whiteKingIcon}: {whiteKings.Count}";
        }
    }

}
