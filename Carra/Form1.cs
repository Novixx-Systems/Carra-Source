// This source code should not be thought of as open source. It is on GitHub
// for learning purposes only. Do not create pull requests.
// If you find a game breaking bug, please do create an issue.
// ALSO NOTE THAT CARRA IS NO LONGER OBFUSCATED


// Copyright (c) 2023 Novixx Systems
//                                  
//             .........
//           .          .
//          .   .    .   .
//          .            .
//          .    ....    .
//           .          .
//             .........

//#define SAVEMAP
#define GENERATEDT

using Microsoft.VisualBasic;
using MoonSharp.Interpreter;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;
using spaghetto;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using static System.Windows.Forms.DataFormats;
#if DEBUG

#warning Debug mode: DO NOT DISTRIBUTE THIS
#warning Debug mode: DO NOT DISTRIBUTE THIS
#warning Debug mode: DO NOT DISTRIBUTE THIS

#endif
namespace Carra
{
    public partial class Form1 : Form
    {
        static Form loading = new Form();
        public ZipArchive? zip;
        public List<ZipArchive> Zipmods = new();
        static bool isLeftDown = false;
        static bool isRightDown = false;
        static bool isUpDown = false;
        static bool isDownDown = false;
        public System.Windows.Forms.Timer accel = new();

        static bool eighteensong = true;

        static int previousRad = -1; // Previous radio song, so it doesn't repeat
        static string[] radioNames = new string[] { "Polish Cow by Tańcząca Krowa", "Milan by MMP Plus", "Coconut Song by Ryan Cayabyab", "Villager Song by MMP Plus", "Monsieur Cannibale by Sacha Distel (MMP Plus version)" };
        static NAudio.Wave.DirectSoundOut wavRadio = new();

        static string policeSiren = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "police.wav");

        // Bools for player
        public static bool isPlayerWanted = false;

        // Global booleans for the game
        public static bool isGamePaused = false;
        public static int polices = 0;
        public static int npcs = 0;
        public static int radioCount = 5;   // No. of songs

        public static bool radioOn = false;
        public static System.Windows.Forms.Timer radioTimer = new();

        // Global variables for the game
        public static Bitmap npcImage;
        public static Bitmap policeImage;
        public static Random rnd = new();
        public Panel mapPanel = new Panel();

        bool inMap = true;

        public int speed = 3;
        public Form1()
        {
            InitializeComponent();
        }

        public Bitmap getImageFromZips(string img)
        {
            // First, try to get the image from the mods
            foreach (ZipArchive zip in Zipmods)
            {
                // Get the image from the ZIP file
                ZipArchiveEntry entry = zip.GetEntry(img);
                // Load the image
                if (entry != null)
                {
                    Bitmap bmp = new Bitmap(entry.Open());
                    // Return the image
                    return bmp;
                }
            }
            // If the image wasn't found in the mods, get it from the main game
            // Get the image from the ZIP file
            ZipArchiveEntry entry2 = zip.GetEntry(img);
            // Load the image
            if (entry2 != null)
            {
                Bitmap bmp2 = new Bitmap(entry2.Open());
                // Return the image
                return bmp2;
            }
            else
            {
                // If the image wasn't found in the main game, return a default image
                return new Bitmap(16, 16);
            }
        }

        public void ParseMap(XmlDocument xml)
        {
            // Get the root node
            XmlNode root = xml.DocumentElement;
            // Get the map node
            XmlNode map = root.SelectSingleNode("map");
            // Get the map size
            int width = int.Parse(map.Attributes["width"].Value);
            int height = int.Parse(map.Attributes["height"].Value);
            // Create the map
            mapPanel.Size = new Size(width * 16, height * 16);
            mapPanel.Location = new Point(0, 0);
            mapPanel.BackColor = Color.Green; // Grass
            // Add the map to the form
            this.Controls.Add(mapPanel);
            // Get the layers
            XmlNodeList layers = map.SelectNodes("layer");
            // Loop through the layers
            foreach (XmlNode layer in layers)
            {
                // Get the layer values
                string name = layer.Attributes["name"].Value;

                // Get the tiles
                XmlNodeList tiles = layer.SelectNodes("tile");
                // Loop through the tiles
                foreach (XmlNode tile in tiles)
                {
                    // Get the tile values
                    int x = int.Parse(tile.Attributes["x"].Value);
                    int y = int.Parse(tile.Attributes["y"].Value);
                    int id = int.Parse(tile.Attributes["id"].Value);
                    // Create the tile
                    PictureBox tileBox = new PictureBox();
                    tileBox.Tag = id;
                    // If collision attribute is "true" 
                    if (tile.Attributes["collision"] != null && tile.Attributes["collision"].Value == "true")
                    {
                        // Set the tile as a collision
                        tileBox.Tag = "COLLIDER";
                    }
                    // Width and height
                    if (tile.Attributes["width"] != null && tile.Attributes["height"] != null)
                    {
                        int w = int.Parse(tile.Attributes["width"].Value);
                        int h = int.Parse(tile.Attributes["height"].Value);
                        tileBox.Size = new Size(w * 80, h * 80);
                    }
                    else
                    {
                        tileBox.Size = new Size(80, 80);
                    }
                    tileBox.Location = new Point(x * 80, y * 80);
                    // Image node
                    XmlNode image = tile.SelectSingleNode("image");
                    // Get the image values
                    string source = image.Attributes["source"].Value;
                    // Get the image from the ZIP file
                    tileBox.BackgroundImage = getImageFromZips(source);
                    // Tile mode is "tile"
                    tileBox.BackgroundImageLayout = ImageLayout.Tile;
                    // Add the tile to the map
                    mapPanel.Controls.Add(tileBox);
                }
            }
        }
        #region DEBUG
#if DEBUG
        // Debug code includes generating maps and the archive, DO NOT DISTRIBUTE THIS
        private XmlDocument MapMaker()
        {
            // Create the map
            XmlDocument doc = new XmlDocument();
            // Create the root node
            XmlNode root = doc.CreateElement("root");
            doc.AppendChild(root);
            // Create the map node
            XmlNode map = doc.CreateElement("map");
            root.AppendChild(map);
            // Set the map attributes
            XmlAttribute width = doc.CreateAttribute("width");
            width.Value = "800";
            map.Attributes.Append(width);
            XmlAttribute height = doc.CreateAttribute("height");
            height.Value = "1000";
            map.Attributes.Append(height);
            // Create the layers
            XmlNode layer1 = doc.CreateElement("layer");
            map.AppendChild(layer1);
            // Set the layer attributes
            XmlAttribute name1 = doc.CreateAttribute("name");
            name1.Value = "BgScenery";
            layer1.Attributes.Append(name1);
            // Create the tiles
            #region Tiles
            // City road
            // The city road uses road.bmp and is for the cars to drive on
            // Road 1 (Horizontal)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "0";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "0";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "1";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "250";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 2 (Vertical)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "0";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "0";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "1";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "150";
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 3 (Vertical)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "10";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "0";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "12";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "150";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 4 (Horizontal)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "0";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "330";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "58";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "140";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 5 (Vertical)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "30";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "0";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "12";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "150";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 6 (Horizontal)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "0";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "100";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "150";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "140";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 7 (Vertical)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "30";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "0";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "12";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "150";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 8 (Horizontal)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "0";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "100";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "150";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "110";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 9 (Vertical)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "30";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "0";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "12";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "150";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 10 (Horizontal)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "0";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "100";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "150";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "80";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 11 (Vertical)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "30";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "0";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "12";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "150";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Road 12 (Horizontal)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer1.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "0";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "100";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "150";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "80";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "road.bmp";
                image.Attributes.Append(source);
            }
            // Buildings layer
            XmlNode layer2 = doc.CreateElement("layer");
            map.AppendChild(layer2);
            // Set the layer attributes
            XmlAttribute name = doc.CreateAttribute("name");
            name.Value = "Buildings";
            layer2.Attributes.Append(name);
            // Buildings
            // Row of houses
            for (int i = 0; i < 4; i++)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer2.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = (1 + i).ToString();
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = "1";
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "0";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                XmlAttribute collision = doc.CreateAttribute("collision");
                collision.Value = "true";
                tile.Attributes.Append(collision);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "house.bmp";
                image.Attributes.Append(source);
            }
            // Row of houses
            for (int i = 0; i < 4; i++)
            {
                // Create the tile
                XmlNode tile = doc.CreateElement("tile");
                layer2.AppendChild(tile);
                // Set the tile attributes
                XmlAttribute x = doc.CreateAttribute("x");
                x.Value = "9";
                tile.Attributes.Append(x);
                XmlAttribute y = doc.CreateAttribute("y");
                y.Value = (16 + i).ToString();
                tile.Attributes.Append(y);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = "0";
                tile.Attributes.Append(id);
                XmlAttribute widthX = doc.CreateAttribute("width");
                widthX.Value = "1";
                tile.Attributes.Append(widthX);
                XmlAttribute heightY = doc.CreateAttribute("height");
                heightY.Value = "1";
                tile.Attributes.Append(heightY);
                XmlAttribute collision = doc.CreateAttribute("collision");
                collision.Value = "true";
                tile.Attributes.Append(collision);
                // Create the image node
                XmlNode image = doc.CreateElement("image");
                tile.AppendChild(image);
                // Set the image attributes
                XmlAttribute source = doc.CreateAttribute("source");
                source.Value = "house.bmp";
                image.Attributes.Append(source);
            }
            #endregion
#if SAVEMAP
            // Save the map as XML
            doc.Save(System.IO.Path.Combine(Application.StartupPath, "map.xml"));
#endif
            // Return the map
            return doc;
        }
        private void SaveDT()
        {
#if GENERATEDT
            // Create the ZIP file
            ZipArchive newZip = new ZipArchive(File.Create(System.IO.Path.Combine(Application.StartupPath, "carra.dt.1")), ZipArchiveMode.Create);
            ZipArchive musicZip = new ZipArchive(File.Create(System.IO.Path.Combine(Application.StartupPath, "carra.dt.2")), ZipArchiveMode.Create);
            ZipArchiveEntry entry = null;
            // Add the image to the ZIP file
            entry = newZip.CreateEntry("stone.bmp");
            Bitmap bmp = new Bitmap(76, 76);
            // Fill BMP with grey
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Gray);
            using (var s = entry.Open())
            {
                bmp.Save(s, ImageFormat.Bmp);
            }
            entry = newZip.CreateEntry("road.bmp");
            Bitmap bmpa = new Bitmap(80, 80);
            // Fill BMP with grey
            Graphics ga = Graphics.FromImage(bmpa);
            ga.Clear(Color.Black);
            using (var s = entry.Open())
            {
                bmpa.Save(s, ImageFormat.Bmp);
            }
            entry = newZip.CreateEntry("house.bmp");
            bmpa = new Bitmap(80, 80);
            // Fill BMP with grey
            ga = Graphics.FromImage(bmpa);
            // Draw a small house (green bg, fill with brown, draw a door)
            ga.Clear(Color.Green);
            // Fill with brown (except for roof)
            ga.FillRectangle(Brushes.Brown, new Rectangle(0, 20, 80, 60));
            // Draw a door
            ga.FillRectangle(Brushes.Black, new Rectangle(30, 60, 20, 20));
            // Draw a roof (upside down triangle)
            Point[] roof = new Point[3];
            roof[0] = new Point(0, 20);
            roof[1] = new Point(80, 20);
            roof[2] = new Point(40, 0);
            ga.FillPolygon(Brushes.Brown, roof);

            using (var s = entry.Open())
            {
                bmpa.Save(s, ImageFormat.Bmp);
            }
            // player.def
            entry = newZip.CreateEntry("player.def");
            StreamWriter playerDefWriter = new StreamWriter(entry.Open());
            playerDefWriter.Write("TEXTURE=player.bmp");
            playerDefWriter.Close();
            // player.bmp
            entry = newZip.CreateEntry("player.bmp");
            Bitmap playerBmpBmp = new Bitmap(32, 64);
            // Fill BMP with dark grey
            Graphics playerBmpG = Graphics.FromImage(playerBmpBmp);
            playerBmpG.Clear(Color.DarkGray);
            using (var s = entry.Open())
            {
                playerBmpBmp.Save(s, ImageFormat.Bmp);
            }
            // npc.bmp
            entry = newZip.CreateEntry("npc.bmp");
            Bitmap npcBmpBmp = new Bitmap(32, 64);
            // Fill BMP with brown
            Graphics npcBmpG = Graphics.FromImage(npcBmpBmp);
            npcBmpG.Clear(Color.Brown);
            using (var s = entry.Open())
            {
                npcBmpBmp.Save(s, ImageFormat.Bmp);
            }
            // police.bmp
            entry = newZip.CreateEntry("police.bmp");
            Bitmap policeBmpBmp = new Bitmap(32, 64);
            // Fill BMP with blue
            Graphics policeBmpG = Graphics.FromImage(policeBmpBmp);
            policeBmpG.Clear(Color.Blue);
            using (var s = entry.Open())
            {
                policeBmpBmp.Save(s, ImageFormat.Bmp);
            }
            // Add the police.wav resource to ZIP
            entry = newZip.CreateEntry("police.wav");
            using (var s = entry.Open())
            {
                Properties.Resources.police.CopyTo(s);
            }
            // Check if theres a dir called "resources"
            if (Directory.Exists("resources"))
            {
                // Loop through all files in the dir
                foreach (string f in Directory.GetFiles("resources"))
                {
                    if (f.EndsWith(".mp3"))
                    {
                        // Add the file to the carra.dt.2
                        entry = musicZip.CreateEntry(System.IO.Path.GetFileName(f));
                        using (var s = entry.Open())
                        {
                            File.OpenRead(f).CopyTo(s);
                        }
                    }
                    else
                    {
                        // Add the file to the ZIP
                        entry = newZip.CreateEntry(System.IO.Path.GetFileName(f));
                        using (var s = entry.Open())
                        {
                            File.OpenRead(f).CopyTo(s);
                        }
                    }
                }
            }
            // Close the ZIP file
            newZip.Dispose();
            musicZip.Dispose();
            // Encrypt the file (replace 0x0000 with 0xacaaaa0a)
            byte[] data = File.ReadAllBytes(System.IO.Path.Combine(Application.StartupPath, "carra.dt.1"));
            for (int i = 0; i < data.Length; i++)
            {
                if (data.Length > i + 1)
                {
                    if (data[i] == 0x00 && data[i + 1] == 0x00)
                    {
                        data[i] = 0xac;
                        data[i + 1] = 0xaa;
                        // Insert 
                        List<byte> b = data.ToList();
                        b.Insert(i + 2, 0xaa);
                        b.Insert(i + 3, 0x0a);
                        data = b.ToArray();
                    }
                }
            }
            // Encrypt the file (replace 0x0000 with 0xacaaaa0a)
            byte[] data2 = File.ReadAllBytes(System.IO.Path.Combine(Application.StartupPath, "carra.dt.2"));
            for (int i = 0; i < data2.Length; i++)
            {
                if (data2.Length > i + 1)
                {
                    if (data2[i] == 0x00 && data2[i + 1] == 0x00)
                    {
                        data2[i] = 0xac;
                        data2[i + 1] = 0xaa;
                        // Insert 
                        List<byte> b = data2.ToList();
                        b.Insert(i + 2, 0xaa);
                        b.Insert(i + 3, 0x0a);
                        data2 = b.ToArray();
                    }
                }
            }
            // Save the file
            File.WriteAllBytes(System.IO.Path.Combine(Application.StartupPath, "carra.dt.1"), data);
            File.WriteAllBytes(System.IO.Path.Combine(Application.StartupPath, "carra.dt.2"), data2);
#endif
        }
#endif
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            SaveDT();
#endif
            // Load carra.dt.1
            string path = System.IO.Path.Combine(Application.StartupPath, "carra.dt.1");
            byte[] data = File.ReadAllBytes(path);
            // Decrypt (replace 0xacaaaa0a with 0x0000)
            for (int i = 0; i < data.Length; i++)
            {
                if (data.Length > i + 3)
                {
                    if (data[i] == 0xac && data[i + 1] == 0xaa && data[i + 2] == 0xaa && data[i + 3] == 0x0a)
                    {
                        data[i] = 0x00;
                        data[i + 1] = 0x00;
                        // Remove the other 2
                        List<byte> b = data.ToList();
                        b.RemoveAt(i + 2);
                        b.RemoveAt(i + 2);
                        data = b.ToArray();
                    }
                }
            }
            // Read the data as a ZIP file
            MemoryStream ms = new MemoryStream(data);
            zip = new ZipArchive(ms);

            // Load carra.dt.2
            string path2 = System.IO.Path.Combine(Application.StartupPath, "carra.dt.2");
            byte[] data2 = File.ReadAllBytes(path2);
            // Decrypt (replace 0xacaaaa0a with 0x0000)
            for (int i = 0; i < data2.Length; i++)
            {
                if (data2.Length > i + 3)
                {
                    if (data2[i] == 0xac && data2[i + 1] == 0xaa && data2[i + 2] == 0xaa && data2[i + 3] == 0x0a)
                    {
                        data2[i] = 0x00;
                        data2[i + 1] = 0x00;
                        // Remove the other 2
                        List<byte> b = data2.ToList();
                        b.RemoveAt(i + 2);
                        b.RemoveAt(i + 2);
                        data2 = b.ToArray();
                    }
                }
            }
            // Read the data as a ZIP file
            MemoryStream ms2 = new MemoryStream(data2);
            ZipArchive music = new ZipArchive(ms2);

            // Read the file
            ZipArchiveEntry entry = zip.GetEntry("map.xml");
            StreamReader sr = new StreamReader(entry.Open());
            string xml = sr.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            // Load the player
            entry = zip.GetEntry("player.def");
            sr = new StreamReader(entry.Open());
            string playerDef = sr.ReadToEnd();
            // Load the player texture
            entry = zip.GetEntry(playerDef.Split('=')[1]);
            Bitmap playerBmp = new Bitmap(entry.Open());

            // Set police bmp and npc bmp
            entry = zip.GetEntry("police.bmp");
            policeImage = new Bitmap(entry.Open());
            entry = zip.GetEntry("npc.bmp");
            npcImage = new Bitmap(entry.Open());

            // Delete police.wav from temp folder
            if (File.Exists(policeSiren))
            {
                File.Delete(policeSiren);
            }

            accel.Interval = 500;
            accel.Tick += accel_Tick;
            accel.Enabled = true;
            accel.Start();

            // Check if a mods folder exists
            if (Directory.Exists(System.IO.Path.Combine(Application.StartupPath, "mods")))
            {
                // Load all mods
                string[] mods = Directory.GetFiles(System.IO.Path.Combine(Application.StartupPath, "mods"), "*.dt");
                foreach (string mod in mods)
                {
                    // Load the mod
                    byte[] modData = File.ReadAllBytes(mod);
                    // Decrypt is not needed, since mods are not encrypted
                    // Read the data as a ZIP file
                    MemoryStream modMs = new MemoryStream(modData);
                    ZipArchive modZip = new ZipArchive(modMs);
                    Zipmods.Add(modZip);

                    // If the mod has a map.xml, load it
                    ZipArchiveEntry modEntry = modZip.GetEntry("map.xml");
                    if (modEntry != null)
                    {
                        StreamReader modSr = new StreamReader(modEntry.Open());
                        string modXml = modSr.ReadToEnd();
                        // Load the XML
                        XmlDocument modDoc = new XmlDocument();
                        modDoc.LoadXml(modXml);
                    }

                    // If the mod has a player.bmp, load it
                    modEntry = modZip.GetEntry("player.bmp");
                    if (modEntry != null)
                    {
                        // Load the player texture
                        playerBmp = new Bitmap(modEntry.Open());
                    }

                    // If the mod has a police.bmp, load it
                    modEntry = modZip.GetEntry("police.bmp");
                    if (modEntry != null)
                    {
                        // Load the police texture
                        policeImage = new Bitmap(modEntry.Open());
                    }

                    // If the mod has a npc.bmp, load it
                    modEntry = modZip.GetEntry("npc.bmp");
                    if (modEntry != null)
                    {
                        // Load the npc texture
                        npcImage = new Bitmap(modEntry.Open());
                    }

                    // If the mod has a police.wav, load it
                    modEntry = modZip.GetEntry("police.wav");
                    if (modEntry != null)
                    {
                        // Put police.wav into temp folder
                        modEntry.ExtractToFile(policeSiren);
                    }

                    // If the mod has any *.lua files, load them
                    ZipArchiveEntry[] luaFiles2 = modZip.Entries.Where(x => x.Name.EndsWith(".lua")).ToArray();
                    foreach (ZipArchiveEntry luaFile in luaFiles2)
                    {
                        // Load the lua file
                        string lua = System.Text.Encoding.UTF8.GetString(luaFile.Open().ReadToEnd());
                        // Create thread to run script
                        Thread t = new Thread(() => RunScript(lua));
                        t.Start();
                    }

                    // If the mod has any *.spag files, load them
                    ZipArchiveEntry[] spagFiles2 = modZip.Entries.Where(x => x.Name.EndsWith(".spag")).ToArray();
                    foreach (ZipArchiveEntry spagFile in spagFiles2)
                    {
                        // Load the spag file
                        string spag = System.Text.Encoding.UTF8.GetString(spagFile.Open().ReadToEnd());
                        // Create thread to run script
                        Thread t = new Thread(() => RunScriptSpaghetto(spag));
                        t.Start();
                    }
                }
            }
            // Create the player
            PictureBox player = new PictureBox();
            player.Image = playerBmp;
            player.Size = new Size(32, 64);

            // Parse the map
            ParseMap(doc);

            // If the ZIP has any *.lua files, load them
            ZipArchiveEntry[] luaFiles = zip.Entries.Where(x => x.Name.EndsWith(".lua")).ToArray();
            foreach (ZipArchiveEntry luaFile in luaFiles)
            {
                // Load the lua file
                string lua = System.Text.Encoding.UTF8.GetString(luaFile.Open().ReadToEnd());
                // Create thread to run script
                Thread t = new Thread(() => RunScript(lua));
                t.Start();
            }

            // If the ZIP has any *.spag files, load them
            ZipArchiveEntry[] spagFiles = zip.Entries.Where(x => x.Name.EndsWith(".spag")).ToArray();
            foreach (ZipArchiveEntry spagFile in spagFiles)
            {
                // Load the spag file
                string spag = System.Text.Encoding.UTF8.GetString(spagFile.Open().ReadToEnd());
                // Create thread to run script
                Thread t = new Thread(() => RunScriptSpaghetto(spag));
                t.Start();
            }
            // Put police.wav into temp folder
            if (!File.Exists(policeSiren))
            {
                entry = zip.GetEntry("police.wav");
                entry.ExtractToFile(policeSiren);
            }
            
            // Also extract all radio_*.mp3 files
            foreach (ZipArchiveEntry radioFile in music.Entries.Where(x => x.Name.StartsWith("radio_") && x.Name.EndsWith(".mp3")))
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetTempPath(), radioFile.Name)))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(System.IO.Path.GetTempPath(), radioFile.Name));
                }
                // Put the radio file into temp folder
                radioFile.ExtractToFile(System.IO.Path.Combine(System.IO.Path.GetTempPath(), radioFile.Name));
            }
            
            // Remove the loading screen
            loading.Close();

            // Location = center of form
            player.Location = new Point(this.Width / 2 - player.Width / 2, this.Height / 2 - player.Height / 2);
            player.BackColor = Color.Transparent;
            player.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(player);
            // Set the player as the active control
            this.ActiveControl = player;
            player.BringToFront();
            // Add arrow key movement
            player.KeyDown += Player_KeyDown;
            player.KeyUp += Player_KeyUp;

            // Create spawnNPC timer
            System.Windows.Forms.Timer spawnNPCTimer = new();
            spawnNPCTimer.Interval = 2000;
            spawnNPCTimer.Tick += (s, a) =>
            {
                // Create a new NPC
                PictureBox npc = new PictureBox();
                npc.Image = npcImage;
                npc.Size = new Size(32, 64);
                // Location = random
                npc.Location = new Point(rnd.Next(0, mapPanel.Width), rnd.Next(0, mapPanel.Height));
                npc.BackColor = Color.Transparent;
                npc.SizeMode = PictureBoxSizeMode.StretchImage;
                npc.Tag = "NPC";
                this.Controls.Add(npc);
                npc.BringToFront();
            };
            spawnNPCTimer.Enabled = true;
            // NPCDespawn timer
            spawnNPCTimer.Start();
            // Create NPCDespawn timer
            System.Windows.Forms.Timer NPCDespawnTimer = new();
            NPCDespawnTimer.Interval = 5000;
            NPCDespawnTimer.Tick += (s, a) =>
            {
                // Loop through all controls
                foreach (Control c in this.Controls)
                {
                    // If it is an NPC
                    if (c.Tag == "NPC")
                    {
                        // If they are outside of the player's view
                        if (c.Location.X < player.Location.X - 2500 || c.Location.X > player.Location.X + 2500 || c.Location.Y < player.Location.Y - 2500 || c.Location.Y > player.Location.Y + 2500)
                        {
                            // Remove the control
                            this.Controls.Remove(c);
                        }
                    }
                }
            };
            NPCDespawnTimer.Enabled = true;
            NPCDespawnTimer.Start();
            // Create policeSpawn timer
            System.Windows.Forms.Timer policeSpawnTimer = new();
            policeSpawnTimer.Interval = 10000;
            policeSpawnTimer.Tick += (s, a) =>
            {
                // Create a new police
                PictureBox police = new PictureBox();
                police.Image = policeImage;
                police.Size = new Size(32, 64);
                // Location = random
                police.Location = new Point(rnd.Next(0, mapPanel.Width), rnd.Next(0, mapPanel.Height));
                police.BackColor = Color.Transparent;
                police.SizeMode = PictureBoxSizeMode.StretchImage;
                police.Tag = "POLICE";
                this.Controls.Add(police);
                police.BringToFront();
            };
            policeSpawnTimer.Enabled = true;
            policeSpawnTimer.Start();
            // Create policeDespawn timer
            System.Windows.Forms.Timer policeDespawnTimer = new();
            policeDespawnTimer.Interval = 21000;
            policeDespawnTimer.Tick += (s, a) =>
            {
                // Loop through all controls
                foreach (Control c in this.Controls)
                {
                    // If it is a police
                    if (c.Tag == "Police")
                    {
                        // Remove it
                        this.Controls.Remove(c);
                        c.Dispose();
                    }
                }
            };
            policeDespawnTimer.Enabled = true;  // Start the timer
            policeDespawnTimer.Start();
            radioTimer.Interval = 1000;
            radioTimer.Tick += radioTimer_Tick;
            radioTimer.Enabled = true;
            radioTimer.Start(); // Start it ofc
        }
        
        private void RunScript(string lua)
        {
            // Automatically register all MoonSharpUserData types
            UserData.RegisterType<LuaPlayer>();
            UserData.RegisterType<LuaMap>();

            Script script = new();
            LuaPlayer plr = new();
            LuaMap lmap = new();
            plr.form1 = this;
            lmap.form1 = this;

            // Pass an instance of MyClass to the script in a global
            script.Globals["player"] = plr;
            script.Globals["map"] = lmap;

            script.Options.DebugPrint = Console.WriteLine;

            script.DoString(lua);
        }

        void Player_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    // Make player face up
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    ((PictureBox)sender).Size = new Size(32, 64);
                    isUpDown = true;
                    break;
                case Keys.Down:
                    // Make player face down
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    ((PictureBox)sender).Size = new Size(32, 64);
                    isDownDown = true;
                    break;
                case Keys.Left:
                    // Make player face left
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    ((PictureBox)sender).Size = new Size(64, 32);
                    isLeftDown = true;
                    break;
                case Keys.Right:
                    // Make player face right
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    ((PictureBox)sender).Size = new Size(64, 32);
                    isRightDown = true;
                    break;
                case Keys.R:
                    // Radio
                    radioOn = !radioOn;
                    radioTimer.Enabled = radioOn;
                    if (radioOn == false)
                    {
                        wavRadio.Stop();
                    }
                    break;
                    // This is commented out because everyone playing this is probably 13+ anyway, so the songs are enabled by default
                    //case Keys.F6:
                    //    // Enable age restricted songs
                    //    LuaPlayer temp = new();
                    //    temp.form1 = this;
                    //    temp.showMessage("13+ Songs enabled (still no swearing, but still)");
                    //    eighteensong = true;
                    //    break;
            }
        }
        void radioTimer_Tick(object sender, EventArgs e)
        {
            if (!wavRadio.PlaybackState.Equals(NAudio.Wave.PlaybackState.Playing) && radioOn)
            {
                // Play a random radio sound
                // Random sound from temp folder (radio_*.mp3)
                int rad = rnd.Next(1, radioCount + 1);
                while (rad == previousRad)
                {
                    rad = rnd.Next(1, radioCount + 1);
                }
                NAudio.Wave.WaveStream mainOutputStream = new NAudio.Wave.BlockAlignReductionStream(new NAudio.Wave.Mp3FileReader(System.IO.Path.GetTempPath() + "radio_" + rad + ".mp3"));
                NAudio.Wave.WaveChannel32 volumeStream = new NAudio.Wave.WaveChannel32(mainOutputStream);
                volumeStream.PadWithZeroes = false;
                volumeStream.Volume = 0.5f;
                wavRadio = new NAudio.Wave.DirectSoundOut();
                wavRadio.Init(volumeStream);
                wavRadio.Play();
                LuaPlayer temp = new();
                temp.form1 = this;
                previousRad = rad;
                temp.showMessage("Now playing: " + radioNames[rad - 1]);
            }
        }
        void Player_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    // Make player face up
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    ((PictureBox)sender).Size = new Size(32, 64);
                    isUpDown = false;
                    break;
                case Keys.Down:
                    // Make player face down
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    ((PictureBox)sender).Size = new Size(32, 64);
                    isDownDown = false;
                    break;
                case Keys.Left:
                    // Make player face left
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    ((PictureBox)sender).Size = new Size(64, 32);
                    isLeftDown = false;
                    break;
                case Keys.Right:
                    // Make player face right
                    ((PictureBox)sender).Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    ((PictureBox)sender).Size = new Size(64, 32);
                    isRightDown = false;
                    break;
            }
        }

        private void walker_Tick(object sender, EventArgs e)
        {
            // If the player is not on the map, stop
            if (this.ActiveControl == null)
                return;

            // If player is in a object that has COLLIDER tag, push them back
            foreach (Control c in this.mapPanel.Controls)
            {
                if (c.Tag.ToString() == "COLLIDER")
                {
                    int x = ActiveControl.PointToScreen(Point.Empty).X;
                    int y = ActiveControl.PointToScreen(Point.Empty).Y;
                    int w = ActiveControl.Width;
                    int h = ActiveControl.Height;
                    int xx = c.PointToScreen(Point.Empty).X;
                    int yy = c.PointToScreen(Point.Empty).Y;
                    if (x < xx + c.Width && x + w > xx && y < yy + c.Height && y + h > yy)
                    {
                        foreach (Control c2 in this.Controls)
                        {
                            // Move everything except the player
                            if (c2 != this.ActiveControl)
                            {
                                int x2 = c2.Location.X;
                                int y2 = c2.Location.Y;
                                c2.Location = new Point(x2, y2 - speed);
                            }
                        }
                        return;
                    }
                }
            }

            // Move the player
            // Move everything except the player
            foreach (Control c in this.Controls)
            {
                int x = ActiveControl.Location.X;
                int y = ActiveControl.Location.Y;
                int w = ActiveControl.Width;
                int h = ActiveControl.Height;
                int xx = c.Location.X;
                int yy = c.Location.Y;
                if (x - 4 < mapPanel.Location.X)
                {
                    xx = c.Location.X - speed; // Move it back
                    if (c != this.ActiveControl && c.Tag != "POLICE" && c.Tag != "DONOTMOVE")
                    {
                        c.Location = new Point(xx, yy);
                    }
                    continue;
                }
                else if ((x + w) + 4 > mapPanel.Location.X + mapPanel.Width)
                {
                    xx = c.Location.X + speed;
                    if (c != this.ActiveControl && c.Tag != "POLICE" && c.Tag != "DONOTMOVE")
                    {
                        c.Location = new Point(xx, yy);
                    }
                    continue;
                }
                else if (y - 4 < mapPanel.Location.Y)
                {
                    yy = c.Location.Y - speed;
                    if (c != this.ActiveControl && c.Tag != "POLICE" && c.Tag != "DONOTMOVE")
                    {
                        c.Location = new Point(xx, yy);
                    }
                    continue;
                }
                else if ((y + h) + 4 > mapPanel.Location.Y + mapPanel.Height)
                {
                    yy = c.Location.Y + speed;
                    if (c != this.ActiveControl && c.Tag != "POLICE" && c.Tag != "DONOTMOVE")
                    {
                        c.Location = new Point(xx, yy);
                    }
                    continue;
                }
                if (isUpDown)
                {
                    yy = c.Location.Y + speed;
                }
                if (isDownDown)
                {
                    yy = c.Location.Y - speed;
                }
                if (isLeftDown)
                {
                    xx = c.Location.X + speed;
                }
                if (isRightDown)
                {
                    xx = c.Location.X - speed;
                }
                if (c != this.ActiveControl && c.Tag != "POLICE" && c.Tag != "DONOTMOVE")
                {
                    c.Location = new Point(xx, yy);
                }
            }
            // Move the NPCs
            foreach (Control c in this.Controls)
            {
                if (c.Tag == "POLICE")
                {
                    // Move only the police
                    int xx = c.Location.X;
                    int yy = c.Location.Y;
                    if (isPlayerWanted)
                    {
                        speed += 2;
                    }
                    if (isUpDown)
                    {
                        yy = c.Location.Y + speed;
                    }
                    if (isDownDown)
                    {
                        yy = c.Location.Y - speed;
                    }
                    if (isLeftDown)
                    {
                        xx = c.Location.X + speed;
                    }
                    if (isRightDown)
                    {
                        xx = c.Location.X - speed;
                    }
                    if (isPlayerWanted)
                    {
                        speed -= 2;
                    }
                    if (c != this.ActiveControl)
                    {
                        c.Location = new Point(xx, yy);
                    }
                    int speed2 = 2;
                    if (!isPlayerWanted)
                    {
                        int x = c.Location.X;
                        int y = c.Location.Y;
                        int w = c.Width;
                        int h = c.Height;
                        // Wander
                        // Move in a random direction
                        switch (rnd.Next(0, 3))
                        {
                            case 0:
                                // Up
                                c.Location = new Point(x, y - speed2);
                                break;
                            case 1:
                                // Down
                                c.Location = new Point(x, y + speed2);
                                break;
                            case 2:
                                // Left
                                c.Location = new Point(x - speed2, y);
                                break;
                            case 3:
                                // Right
                                c.Location = new Point(x + speed2, y);
                                break;
                        }
                    }
                    else
                    {
                        int x = c.Location.X;
                        int y = c.Location.Y;
                        speed2 = 7;
                        // Follow the player
                        // Move towards the center of the screen (which is the player)
                        if (x < this.Width / 2)
                        {
                            // Move right
                            c.Location = new Point(x + speed2, y);
                        }
                        else if (x > this.Width / 2)
                        {
                            // Move left
                            c.Location = new Point(x - speed2, y);
                        }
                        if (y < this.Height / 2)
                        {
                            // Move down
                            c.Location = new Point(x, y + speed2);
                        }
                        else if (y > this.Height / 2)
                        {
                            // Move up
                            c.Location = new Point(x, y - speed2);
                        }
                    }
                    // If police touches player
                    if (c.Bounds.IntersectsWith(this.ActiveControl.Bounds))
                    {
                        if (!isPlayerWanted)
                        {
                            isPlayerWanted = true;
                            // Play police siren
                            NAudio.Wave.WaveOutEvent outputDevice = new NAudio.Wave.WaveOutEvent();
                            NAudio.Wave.WaveFileReader reader = new NAudio.Wave.WaveFileReader(policeSiren);
                            outputDevice.Init(reader);
                            outputDevice.Play();
                        }
                        else
                        {
                            // 1 in 30 chance of being caught
                            if (rnd.Next(1, 31) == 1)
                            {
                                // Restart this with --skip-load args
                                Process.Start(Application.ExecutablePath, "--skip-load");
                                // Close this
                                Application.Exit();
                            }
                        }
                    }
                }
                else if (c.Tag == "NPC")
                {
                    // Wander
                    int x = c.Location.X;
                    int y = c.Location.Y;
                    int w = c.Width;
                    int h = c.Height;
                    int speed = 2;
                    // Move in a random direction
                    switch (rnd.Next(0, 3))
                    {
                        case 0:
                            // Up
                            c.Location = new Point(x, y - speed);
                            break;
                        case 1:
                            // Down
                            c.Location = new Point(x, y + speed);
                            break;
                        case 2:
                            // Left
                            c.Location = new Point(x - speed, y);
                            break;
                        case 3:
                            // Right
                            c.Location = new Point(x + speed, y);
                            break;
                    }
                }
            }
        }

        private void RunScriptSpaghetto(string code)
        {
            LuaPlayer plr = new LuaPlayer();
            plr.form1 = this;
            LuaMap map = new LuaMap();
            map.form1 = this;

            // Because the creator of spaghetto spelled Interpreter wrong, I'm going to spell it wrong too (or else it won't work)
            Intepreter.globalSymbolTable.Add("playerSetSpeed", new NativeFunction("playerSetSpeed", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
            {
                plr.setSpeed(Convert.ToInt32(((Number)args[0]).ToString()));
                return new Number(0);
            }, new() { "speed" }, false));
            Intepreter.globalSymbolTable.Add("playerGetLocation", new NativeFunction("playerGetLocation", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
            {
                return plr.getPositionSpag();
            }, new() { }, false));
            Intepreter.globalSymbolTable.Add("mapSetTile", new NativeFunction("mapSetTile", (List<Value> args, Position posStart, Position posEnd, Context ctx) =>
            {
                map.setTile(Convert.ToInt32(((Number)args[0]).ToString()), Convert.ToInt32(((Number)args[1]).ToString()), bool.Parse(args[2].ToString()), args[3].ToString());
                return new Number(0);
            }, new() { "x", "y", "collision", "image" }, false));
            (RuntimeResult res, SpaghettoException err) = Intepreter.Run(string.Empty, code);
        }

        private void accel_Tick(object sender, EventArgs e)
        {
            if (isUpDown || isDownDown || isLeftDown || isRightDown)
            {
                // Decrement the walker timer
                if (walker.Interval > 1)
                {
                    speed++;
                }
            }
            else
            {
                // Increment the walker timer
                if (walker.Interval < 10)
                {
                    if (speed > 0)
                    {
                        speed--;
                    }
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //for (int i = 0; i < radioCount; i++)
            //{
            //    if (File.Exists(System.IO.Path.GetTempPath() + "radio_" + i + ".mp3"))
            //    {
            //        try
            //        {
            //            File.Delete(System.IO.Path.GetTempPath() + "radio_" + i + ".mp3");
            //        }
            //        catch { };
            //    }
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}