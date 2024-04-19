namespace Checkers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int BoardRows { get; set; } = 8;
        public static int BoardCols { get; set; } = 8;

        private void Form1_Load(object sender, EventArgs e)
        {
            Checkers game = new();
            var board = game.CreateBoard(BoardRows, BoardCols);
            var btn = game.CreateButton();
            var label = game.CreateLabel();

            this.Controls.Add(label);
            this.Controls.Add(btn);
            this.Controls.Add(board);
        }
    }
}