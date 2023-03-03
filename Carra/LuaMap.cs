// This source code should not be thought of as open source. It is on GitHub
// for learning purposes only. Do not create pull requests.
// If you find a game breaking bug, please do create an issue.
// ALSO NOTE THAT CARRA IS NO LONGER OBFUSCATED


// Copyright (c) 2023 Novixx Systems
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Carra
{
    [MoonSharpUserData]
    class LuaMap
    {
        private delegate void MyDelegate();

        // Because I am bored, I present to you: Sofia by Alvaro Soler
        // Mira Sofia, sin tu mirada sigo sin tu mirada sigo
        // Dime Sofia ah ah, como te mira dime como te mira dime
        // Se que no, se que no oh, se que ya no soy oy oy oy
        // Mira Sofia, sin tu mirada sigo sin tu mirada Sofia
        // Thanks to co pilot for not knowing the lyrics

        public Form1 form1;
        // Lua functions for the map
        public void setTile(int x, int y, bool collision, string image)
        {
            if (form1.InvokeRequired)
            {
                form1.Invoke(new MyDelegate(() => setTile(x, y, collision, image)));
            }
            else
            {
                // Create the tile
                PictureBox tileBox = new PictureBox();
                // If collision attribute is "true" 
                if (collision)
                {
                    // Set the tile as a collision
                    tileBox.Tag = "COLLIDER";
                }
                tileBox.Size = new Size(80, 80);
                tileBox.Location = new Point(x * 80, y * 80);
                // Get the image from the ZIP file
                tileBox.Image = form1.getImageFromZips(image);
                // Add the tile to the map
                form1.mapPanel.Controls.Add(tileBox);
            }
        }
    }
}
