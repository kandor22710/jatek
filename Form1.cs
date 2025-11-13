using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private List<Point> snake = new List<Point>();
        private Point food;
        private int cellSize = 20;
        private int cols, rows;
        private string direction = "RIGHT";
        private bool gameOver = false;
        private int score = 0;
        private Random rand = new Random();
        private bool speedBoost = false;
        private string theme;
        private List<Point> obstacles = new List<Point>();

        public Form1(string selectedTheme)
        {
            InitializeComponent();
            this.Text = "🐍 Snake Game";
            this.DoubleBuffered = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.ClientSize = new Size(600, 400);

            theme = selectedTheme;
            ApplyTheme(theme);

            cols = this.ClientSize.Width / cellSize;
            rows = this.ClientSize.Height / cellSize;

            StartGame();
        }

        private void ApplyTheme(string theme)
        {
            switch (theme)
            {
                case "Classic":
                    this.BackColor = Color.Black;
                    break;
                case "Neon":
                    this.BackColor = Color.FromArgb(5, 5, 20);
                    break;
                case "Retro":
                    this.BackColor = Color.FromArgb(0, 40, 0);
                    break;
            }
        }

        private void StartGame()
        {
            score = 0;
            direction = "RIGHT";
            gameOver = false;
            snake.Clear();
            obstacles.Clear();

            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));

            GenerateFood();
            GenerateObstacles();

            timer = new Timer();
            timer.Interval = 120;
            timer.Tick += Update;
            timer.Start();
        }

        private void GenerateFood()
        {
            food = new Point(rand.Next(0, cols), rand.Next(0, rows));
        }

        private void GenerateObstacles()
        {
            for (int i = 0; i < 5; i++)
            {
                obstacles.Add(new Point(rand.Next(3, cols - 3), rand.Next(3, rows - 3)));
            }
        }

        private void Update(object sender, EventArgs e)
        {
            if (gameOver)
            {
                timer.Stop();
                return;
            }

            MoveSnake();
            Invalidate();
        }

        private void MoveSnake()
        {
            Point head = snake[0];
            Point newHead = head;

            switch (direction)
            {
                case "UP": newHead = new Point(head.X, head.Y - 1); break;
                case "DOWN": newHead = new Point(head.X, head.Y + 1); break;
                case "LEFT": newHead = new Point(head.X - 1, head.Y); break;
                case "RIGHT": newHead = new Point(head.X + 1, head.Y); break;
            }

            if (newHead.X < 0 || newHead.Y < 0 || newHead.X >= cols || newHead.Y >= rows)
            {
                EndGame();
                return;
            }

            foreach (var part in snake)
            {
                if (part == newHead)
                {
                    EndGame();
                    return;
                }
            }

            foreach (var obs in obstacles)
            {
                if (obs == newHead)
                {
                    EndGame();
                    return;
                }
            }

            snake.Insert(0, newHead);

            if (newHead == food)
            {
                SystemSounds.Beep.Play();
                score += 10;
                GenerateFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            timer.Interval = speedBoost ? 60 : 120;
        }

        private void EndGame()
        {
            SystemSounds.Hand.Play();
            gameOver = true;
            Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameOver && e.KeyCode == Keys.Enter)
            {
                StartGame();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != "DOWN") direction = "UP";
                    break;
                case Keys.Down:
                    if (direction != "UP") direction = "DOWN";
                    break;
                case Keys.Left:
                    if (direction != "RIGHT") direction = "LEFT";
                    break;
                case Keys.Right:
                    if (direction != "LEFT") direction = "RIGHT";
                    break;
                case Keys.ShiftKey:
                    speedBoost = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                speedBoost = false;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

        private void Form1_Load(object sender, EventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (gameOver)
            {
                string text = "💀 GAME OVER 💀\nPress ENTER to restart";
                SizeF textSize = g.MeasureString(text, new Font("Consolas", 20));
                g.DrawString(text, new Font("Consolas", 20, FontStyle.Bold),
                    Brushes.White,
                    (ClientSize.Width - textSize.Width) / 2,
                    (ClientSize.Height - textSize.Height) / 2);
                return;
            }

            // Étel
            g.FillEllipse(Brushes.Red, food.X * cellSize, food.Y * cellSize, cellSize, cellSize);

            // Akadályok
            foreach (var obs in obstacles)
            {
                if (theme == "Neon")
                    DrawGlow(g, obs, Color.DeepPink, 8);
                else
                {
                    Brush brush = theme == "Retro" ? Brushes.DarkOliveGreen : Brushes.Gray;
                    g.FillRectangle(brush, obs.X * cellSize, obs.Y * cellSize, cellSize - 1, cellSize - 1);
                }
            }

            // Kígyó
            for (int i = 0; i < snake.Count; i++)
            {
                Brush b;
                Color baseColor;
                if (theme == "Neon")
                {
                    baseColor = i == 0 ? Color.Cyan : Color.MediumPurple;
                    DrawGlow(g, snake[i], baseColor, 6);
                }
                else if (theme == "Retro")
                {
                    b = i == 0 ? Brushes.Lime : Brushes.GreenYellow;
                    g.FillRectangle(b, snake[i].X * cellSize, snake[i].Y * cellSize, cellSize - 1, cellSize - 1);
                }
                else
                {
                    b = i == 0 ? Brushes.Lime : Brushes.GreenYellow;
                    g.FillRectangle(b, snake[i].X * cellSize, snake[i].Y * cellSize, cellSize - 1, cellSize - 1);
                }
            }

            // Pontszám
            Brush scoreBrush = theme == "Retro" ? Brushes.LawnGreen : Brushes.White;
            g.DrawString($"Score: {score}", new Font("Consolas", 14, FontStyle.Bold), scoreBrush, 5, 5);
        }

        // 🌈 Neon fényhatás (Glow)
        private void DrawGlow(Graphics g, Point p, Color color, int intensity)
        {
            for (int i = intensity; i >= 1; i--)
            {
                int alpha = (int)(255 * (0.1f + (float)i / intensity / 2));
                using (SolidBrush glow = new SolidBrush(Color.FromArgb(alpha, color)))
                {
                    int size = cellSize + (intensity - i);
                    int offset = (cellSize - size) / 2;
                    g.FillEllipse(glow, p.X * cellSize - offset, p.Y * cellSize - offset, size, size);
                }
            }
        }
    }
}
