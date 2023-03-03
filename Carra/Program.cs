// This source code should not be thought of as open source. It is on GitHub
// for learning purposes only. Do not create pull requests.
// If you find a game breaking bug, please do create an issue.
// ALSO NOTE THAT CARRA IS NO LONGER OBFUSCATED


// Copyright (c) 2023 Novixx Systems
namespace Carra
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string[] argv = Environment.GetCommandLineArgs();
            int argc = argv.Length;
            ApplicationConfiguration.Initialize();
            if (argc > 1 && argv[1] == "--skip-load")
            {
                Application.Run(new Form1());
            }
            else
            {
                Application.Run(new Intro());
            }
        }
    }
}