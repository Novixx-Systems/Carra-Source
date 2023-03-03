// This source code should not be thought of as open source. It is on GitHub
// for learning purposes only. Do not create pull requests.
// If you find a game breaking bug, please do create an issue.
// ALSO NOTE THAT CARRA IS NO LONGER OBFUSCATED


// Copyright (c) 2023 Novixx Systems
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carra
{
    public partial class Intro : Form
    {
        public Intro()
        {
            InitializeComponent();
        }

        private async void Intro_Load(object sender, EventArgs e)
        {
            // Play the intro music ( Star Wars theme song )
            Console.Beep(440, 500);
            Console.Beep(440, 500);
            Console.Beep(440, 500);
            Console.Beep(349, 350);
            Console.Beep(523, 150);
            Console.Beep(440, 500);
            Console.Beep(349, 350);
            Console.Beep(523, 150);
            Console.Beep(440, 650);
            Console.Beep(659, 500);
            Console.Beep(659, 500);
            Console.Beep(659, 500);
            Console.Beep(698, 350);
            Console.Beep(523, 150);
            Console.Beep(415, 500);
            Console.Beep(349, 350);
            Console.Beep(523, 150);
            Console.Beep(440, 650);
            Console.Beep(880, 500);
            Console.Beep(440, 350);
            Console.Beep(440, 150);
            Console.Beep(880, 500);
            Console.Beep(830, 250);
            Console.Beep(784, 250);
            Console.Beep(740, 125);
            Console.Beep(698, 125);
            Console.Beep(740, 250);
            Console.Beep(300, 1000);
            // Start the police siren
            NAudio.Wave.WaveOutEvent outputDevice = new NAudio.Wave.WaveOutEvent();
            NAudio.Wave.WaveFileReader reader = new NAudio.Wave.WaveFileReader(Properties.Resources.police);
            outputDevice.Init(reader);
            outputDevice.Play();
            // Make a blue police car
            PictureBox car = new PictureBox();
            car.BackColor = Color.Blue;
            car.Size = new Size(100, 50);
            car.Location = new Point(0, 150);
            car.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(car);
            // Move car across the screen
            System.Windows.Forms.Timer timer = new();
            timer.Interval = 2;
            timer.Tick += async (s, a) =>
            {
                car.Location = new Point(car.Location.X + 2, car.Location.Y);
                if (car.Location.X > 500)
                {
                    outputDevice.Stop();
                    timer.Stop();
                    car.Dispose();
                    // Play intro pop
                    NAudio.Wave.WaveOutEvent outputDevice2 = new NAudio.Wave.WaveOutEvent();
                    NAudio.Wave.WaveFileReader reader2 = new NAudio.Wave.WaveFileReader(Properties.Resources.intropop);
                    outputDevice2.Init(reader2);
                    outputDevice2.Play();
                    // Show intro text for 5 seconds
                    Label label = new Label();
                    label.Text = "Welcome to Carra! \r\n \r\n (c) 2023 Novixx Systems \r\n This game is not affiliated with Star Wars, Lucasfilm, or Rockstar Games. \r\n \r\n Please do not distribute this game without permission.";
                    label.AutoSize = true;
                    label.Font = new Font("Arial", 20);
                    label.ForeColor = Color.White;
                    // Center the text by taking half the screen width and subtracting half the text width
                    label.Location = new Point((this.Width / 2) - (label.Width / 2), (this.Height / 2) - (label.Height / 2));
                    this.Controls.Add(label);
                    await Task.Delay(5000);
                    // Start the game
                    Form1 game = new();
                    game.Show();
                    this.Hide();
                }
            };
            timer.Start();
        }
    }
}
