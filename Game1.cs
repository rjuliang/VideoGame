using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FNAClassCode
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public List<GameObject> objects = new List<GameObject>(); //A list of all the objects in our game.
        public Map map = new Map(); //Map of our current level.
        GameHUD gameHUD = new GameHUD(); //Heads up display!

        Editor editor;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set up the window resolution:
            Resolution.Init(ref graphics);
            Resolution.SetVirtualResolution(1280, 720); //Resolution our assets are based in / the resolution we're working in.

            //Now set the resolution we want to actually render onto the screen.
            //The Resolution manager automatically adjusts the art to match whatever resolution we want to render at:
            Resolution.SetResolution(1246,720, false);
        }

        protected override void Initialize()
        {
#if DEBUG
            editor = new Editor(this);
            editor.Show();
#endif

            Global.Game = this;

            Camera.Initialize();
            Camera.updateXAxis = false;
            Camera.updateYAxis = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

#if DEBUG
            editor.LoadTextures(Content);
#endif

            map.Load(Content);
            gameHUD.Load(Content);
            LoadLevel("Third.jorge");
            
        }

        protected override void Update(GameTime gameTime)
        {
            //Update the input for this frame, then the objects:
            Input.Update();
            UpdateObjects();
            map.Update(objects);
            UpdateCamera();

#if DEBUG
            editor.Update(objects, map);
#endif

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Take care of any viewport adjustments for our final draw:
            Resolution.BeginDraw();

            //Pass the camera matrix into SpriteBatch.Begin(), it will move all of our images based on the camera's location etc:
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.GetTransformMatrix());
#if DEBUG
            editor.Draw(spriteBatch);
#endif
            DrawObjects();
            map.DrawWalls(spriteBatch);
            spriteBatch.End();

            //After drawing the things effected by the camera, we can draw the GameHUD on top of it:
            gameHUD.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        #region Helper Functions
        public void LoadLevel(string fileName)
        {
            Global.levelName = fileName;
            if (objects.Count > 0)
                ((Player)objects[0]).StopMusic();

            //Load the level:
            LevelData levelData = XmlHelper.Load("Content\\Levels\\" + fileName);

            map.walls = levelData.walls;
            map.decor = levelData.decor;
            objects = levelData.objects;

            //Load the things in the map:
            map.LoadMap(Content);

            //Now initialize and load the objects in our level:
            LoadObjects();
        }

        public void LoadObjects()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Initialize();
                objects[i].Load(Content);
            }
        }

        private void UpdateObjects()
        {
#if DEBUG
            if (editor.paused.Checked == true)
                return;
#endif

            //Update all of the objects in our game:
            for (int i = 0; i < objects.Count; i++)
                objects[i].Update(objects, map);
        }

        private void UpdateCamera()
        {
            if (objects.Count == 0)
                return; 

#if DEBUG
            if (editor.paused.Checked == true)
                return;
#endif

            //Pass in the position of the player (usually the first object in the list):
            Camera.Update(objects[0].position);
            
            //NOTE: If you don't want the camera to follow the player, you can simply set updateYAxis and updateXAxis to false in the Camera.cs file. 
            //You can also update one axis or both depending on the game you're making. If you're making a space shooter you might only want
            //the camera to follow the player on the Y axis. For a platformer, the camera usually only follows on the x axis. Up to you!
        }

        private void DrawObjects()
        {
            //Draw all of the objects in our game at once:
            for (int i = 0; i < objects.Count; i++)
                objects[i].Draw(spriteBatch);

            //Now draw the decor:
            for (int i = 0; i < map.decor.Count; i++)
                map.decor[i].Draw(spriteBatch);
        }
        #endregion
    }
}
