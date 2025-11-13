using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class MainMenuForm : Form
    {
        private ComboBox themeSelector;
        private Button btnStart, btnExit;
        private Label lblTitle, lblTheme;
        private Timer titleTimer;
        private SoundPlayer musicPlayer;
        private bool titleVisible = true;

        public MainMenuForm()
        {
            InitializeComponent();
            PlayBackgroundMusic();
        }

        private void InitializeComponent()
        {
            this.Text = "🐍 Snake Game - Main Menu";
            this.Size = new Size(400, 300);
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.FormClosing += MainMenuForm_FormClosing;

            lblTitle = new Label()
            {
                Text = "🐍 SNAKE GAME 🐍",
                Font = new Font("Consolas", 26, FontStyle.Bold),
                ForeColor = Color.Lime,
                AutoSize = true,
                Location = new Point(35, 40)
            };
            this.Controls.Add(lblTitle);

            // Timer a villogó címhez
            titleTimer = new Timer();
            titleTimer.Interval = 400; // fél másodpercenként vált
            titleTimer.Tick += (s, e) =>
            {
                titleVisible = !titleVisible;
                lblTitle.ForeColor = titleVisible ? Color.Lime : Color.FromArgb(40, 100, 40);
            };
            titleTimer.Start();

            lblTheme = new Label()
            {
                Text = "Select Theme:",
                Font = new Font("Consolas", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(40, 130)
            };
            this.Controls.Add(lblTheme);

            themeSelector = new ComboBox()
            {
                Location = new Point(180, 125),
                Width = 140,
                Font = new Font("Consolas", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            themeSelector.Items.AddRange(new string[] { "Classic", "Neon", "Retro" });
            themeSelector.SelectedIndex = 0;
            this.Controls.Add(themeSelector);

            btnStart = CreateButton("▶️ Start Game", 180);
            btnStart.BackColor = Color.DarkGreen;
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            btnExit = CreateButton("❌ Exit", 225);
            btnExit.BackColor = Color.Maroon;
            btnExit.Click += (s, e) => this.Close();
            this.Controls.Add(btnExit);
        }

        private Button CreateButton(string text, int y)
        {
            Button b = new Button()
            {
                Text = text,
                Location = new Point(100, y),
                Size = new Size(180, 35),
                Font = new Font("Consolas", 11, FontStyle.Bold),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderSize = 0;
            b.MouseEnter += (s, e) => b.BackColor = ControlPaint.Light(b.BackColor);
            b.MouseLeave += (s, e) => b.BackColor = ControlPaint.Dark(b.BackColor);
            return b;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            string selectedTheme = themeSelector.SelectedItem.ToString();
            musicPlayer?.Stop();
            this.Hide();
            Form1 gameForm = new Form1(selectedTheme);
            gameForm.ShowDialog();
            this.Show();
            PlayBackgroundMusic();
        }

        private void PlayBackgroundMusic()
        {
            try
            {
                string musicPath = System.IO.Path.Combine(Application.StartupPath, "menu_music.wav");
                if (System.IO.File.Exists(musicPath))
                {
                    musicPlayer = new SoundPlayer(musicPath);
                    musicPlayer.PlayLooping();
                }
            }
            catch
            {
                // ha nincs zene, ne dobjon hibát
            }
        }

        private void MainMenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            musicPlayer?.Stop();
        }
    }
}

