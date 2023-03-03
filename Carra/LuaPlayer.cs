// This source code should not be thought of as open source. It is on GitHub
// for learning purposes only. Do not create pull requests.
// If you find a game breaking bug, please do create an issue.
// ALSO NOTE THAT CARRA IS NO LONGER OBFUSCATED


// Copyright (c) 2023 Novixx Systems
using MoonSharp.Interpreter;
using spaghetto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace Carra
{
    [MoonSharpUserData]
    class LuaPlayer
    {
        Label label = new Label();
        public Form1 form1;
        // Lua functions for the player
        public void setPositionNoMove(int x, int y)
        {
            // Set the player's position
            form1.ActiveControl.Location = new Point(x, y);
        }
        
        public void setAccelSpeed(int speed)
        {
            form1.accel.Interval = speed;
    }
        
        public void setSpeed(int speed)
        {
            form1.speed = speed;
        }
        // Get the map's position
        public spaghetto.Value getPositionSpag()
        {
            spaghetto.StringValue val = new spaghetto.StringValue(form1.mapPanel.Location.X + "," + form1.mapPanel.Location.Y);
            return val;
        }
        // Lua version of above
        public string getPosition()
        {
            return form1.mapPanel.Location.X + "," + form1.mapPanel.Location.Y;
        }
        // Show message 
        public int showMessage(string message)
        {
            if (form1.InvokeRequired)
            {
                return (int)form1.Invoke(new Func<int>(() => showMessage(message)));
            }
            if (form1.Controls.Contains(label))
            {
                label.Text = message;
                return 1;
            }
            label.Tag = "DONOTMOVE";
            label.Text = message;
            label.AutoSize = true;
            label.Font = new Font("Arial", 25);
            // Center top of screen
            label.Location = new Point(form1.Width / 2 - label.Width / 2, 0);
            form1.Controls.Add(label);
            label.BringToFront();
            Timer timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += (sender, e) =>
            {
                form1.Controls.Remove(label);
                timer.Stop();
            };
            timer.Start();
            return 1;
        }
        // Check if the player is colliding with a tile
        public bool isColliding(int id)
        {
            if (form1.InvokeRequired)
            {
                if (form1.IsDisposed == true)
                {
                    return false;
                }
                return (bool)form1.Invoke(new Func<bool>(() => isColliding(id)));
            }
            // Get the player's position
            Point playerPos = form1.ActiveControl.Location;
            // Get the player's size
            Size playerSize = form1.ActiveControl.Size;
            // Get the player's collision box
            Rectangle playerRect = new Rectangle(playerPos, playerSize);
            // Loop through all the tiles
            foreach (Control tile in form1.mapPanel.Controls)
            {
                if (tile.Tag == null)
                {
                    continue;
                }
                // If the tile's ID is not the same as the ID given
                if (tile.Tag.ToString() != id.ToString())
                {
                    // Skip the tile
                    continue;
                }
                // Get the tile's position
                Point tilePos = tile.Location;
                // Get the tile's size
                Size tileSize = tile.Size;
                // Get the tile's collision box
                Rectangle tileRect = new Rectangle(tilePos, tileSize);
                // If the player's collision box intersects with the tile's collision box

                int x = form1.ActiveControl.PointToScreen(Point.Empty).X;
                int y = form1.ActiveControl.PointToScreen(Point.Empty).Y;
                int w = form1.ActiveControl.Width;
                int h = form1.ActiveControl.Height;
                int xx = tile.PointToScreen(Point.Empty).X;
                int yy = tile.PointToScreen(Point.Empty).Y;
                if (x < xx + tile.Width && x + w > xx && y < yy + tile.Height && y + h > yy)
                {
                    // Return true
                    return true;
                }
            }
            // Return false
            return false;
        }
    }
}
