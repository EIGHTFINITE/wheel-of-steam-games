﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using OlegEngine;

namespace WheelOfSteamGames
{
    class Program : GameWindow
    {
        public const string SettingsFile = "settings.cfg";
        public Engine engine;
        public Program(Settings settings)
            : base(settings.Width, settings.Height, new GraphicsMode(32, 24, 0, settings.Samples), "Wheel of Steam Games", settings.WindowMode == WindowState.Fullscreen && settings.NoBorder ? GameWindowFlags.Fullscreen : GameWindowFlags.Default )
        {
            Utilities.EngineSettings = settings;
            VSync = settings.VSync;
            if (settings.NoBorder)
            {
                this.WindowBorder = OpenTK.WindowBorder.Hidden;
            }
            if (!settings.NoBorder && settings.WindowMode == OpenTK.WindowState.Fullscreen)
                this.WindowState = settings.WindowMode;
            else if (settings.NoBorder) this.WindowState = OpenTK.WindowState.Normal;

            engine = new Engine(this); //Create the engine class that'll do all the heavy lifting
            engine.OnRenderSceneOpaque += new Action<FrameEventArgs>(RenderSceneOpaque);
            engine.OnRenderSceneTranslucent += new Action<FrameEventArgs>(RenderSceneTranslucent);

            try
            {
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            }
            catch (Exception e)
            {
                Utilities.Print("Failed to load icon! {0}", Utilities.PrintCode.WARNING, e.Message);
            }

            if (!settings.ShowConsole)
            {
                IntPtr consoleHandle = ConsoleManager.GetConsoleWindow();
                ConsoleManager.ShowWindow(consoleHandle, ConsoleManager.SW_HIDE);
            }
        }


        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            engine.OnLoad(e);

            Splash.Initialize();
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            engine.OnResize(e);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            engine.OnUpdateFrame(e);

            MainRoom.Think();

            //if (this.Keyboard[Key.Escape]) this.Exit();
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            engine.OnRenderFrame(e);

            SwapBuffers();
        }

        /// <summary>
        /// Called by the engine when it is time to render opaque renderables
        /// </summary>
        /// <param name="e"></param>
        private void RenderSceneOpaque(FrameEventArgs e)
        {
            //Draw opaque geometry
            MainRoom.Draw();
            OlegEngine.Entity.EntManager.DrawOpaque(e);

            //Draw debug stuff
            Graphics.DrawDebug();
        }

        /// <summary>
        /// Called by the engine when it is time to render potentially-translucent renderables
        /// </summary>
        /// <param name="e"></param>
        void RenderSceneTranslucent(FrameEventArgs e)
        {
            //Now draw geometry that is potentially transcluent
            GL.Enable(EnableCap.Blend);
            OlegEngine.Entity.EntManager.DrawTranslucent(e);
            GL.Disable(EnableCap.Blend);
        }

        [STAThread]
        static void Main(string[] args)
        {
            Utilities.Print("==================================", Utilities.PrintCode.INFO);
            Utilities.Print("ENGINE STARTUP", Utilities.PrintCode.INFO);
            Utilities.Print("==================================\n", Utilities.PrintCode.INFO);

            using (Program game = new Program(new Settings(SettingsFile)))
            {
                game.Run(60.0);
            }
        }
    }
}
