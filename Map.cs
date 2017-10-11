using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FNAClassCode
{
    public class Map
    {
        public List<Decor> decor = new List<Decor>(); //All the non interactive images in our level... backgrounds etc.
        public List<Wall> walls = new List<Wall>(); //All the walls in the map that characters will collide with.
        Texture2D wallImage; //Used to draw the black walls on the map.

        public int mapWidth = 15; //How many grids wide is the map?
        public int mapHeight = 9; //How many grids tall is the map?
        public int tileSize = 128; //Dimensions of a single grid space. 128 x 128 is a good size to work in.

        public void LoadMap(ContentManager content)
        {
            //Load the images needed for the decor:
            for (int i = 0; i < decor.Count; i++)
                decor[i].Load(content, decor[i].imagePath);
        }

        public void Load(ContentManager content)
        {
            wallImage = TextureLoader.Load("pixel", content);
        }

        public void Update(List<GameObject> objects)
        {
            for (int i = 0; i < decor.Count; i++)
                decor[i].Update(objects, this);
        }

        public Rectangle CheckCollision(Rectangle input)
        {
            //Does the rectangle passed in collide with any of the walls on the map?
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].wall.Intersects(input) == true)
                    return walls[i].wall; //Return the exact wall that was collided with.
            }
            return Rectangle.Empty;
        }

        public void DrawWalls(SpriteBatch spriteBatch)
        {
            //Draw the walls of the map:
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i] != null && walls[i].active == true)
                    spriteBatch.Draw(wallImage, new Vector2((int)walls[i].wall.X, (int)walls[i].wall.Y), walls[i].wall, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .7f);
            }
        }

        /// <summary>
        /// Returns the tile index of the nearest tile to the position passed in.
        /// </summary>
        public Point GetTileIndex(Vector2 inputPosition)
        {
            if (inputPosition == new Vector2(-1, -1))
                return new Point(-1, -1);

            return new Point((int)inputPosition.X / tileSize, (int)inputPosition.Y / tileSize);
        }
    }

    public class Wall
    {
        public Rectangle wall;
        public bool active = true;

        public Rectangle EditorWall { get { return wall; } } //Used to show the wall positions in the editor and that's it.

        public Wall()
        {
        }

        public Wall(Rectangle inputRect)
        {
            wall = inputRect;
        }
    }

    public class Decor : GameObject //Used to add backgrounds and non interactive images into our game.
    {
        public string imagePath; //The path to load the image belonging to this decor object.
        public Rectangle sourceRect;

        public string Name { get { return imagePath; } } //Displayed by the editor.

        public Decor()
        {
            collidable = false;
        }

        public Decor(Vector2 inputPosition, string inputImagePath, float inputDepth)
        {
            //Constructor used for loading decor:
            position = inputPosition;
            imagePath = inputImagePath;
            layerDepth = inputDepth;
            active = true;
            collidable = false;
        }

        public virtual void Load(ContentManager content, string asset)
        {
            //Loads file from the asset passed in:
            image = TextureLoader.Load(asset, content);
            image.Name = asset;

            //Now set our boundingbox based on the image dimensions:
            boundingBox = new Rectangle((int)(position.X + boundingBoxOffset.X), (int)(position.Y + boundingBoxOffset.Y), image.Width, image.Height);

            if (sourceRect == Rectangle.Empty)
                sourceRect = new Rectangle(0, 0, image.Width, image.Height);
        }

        public void SetImage(Texture2D input, string newPath) //Used to manually set the image from our editor.
        {
            image = input;
            imagePath = newPath;
            boundingBox.Width = sourceRect.Width = image.Width;
            boundingBox.Height = sourceRect.Height = image.Height;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (image != null && active == true)
                spriteBatch.Draw(image, position, sourceRect, drawColor, rotation, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }
    }
}
