using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FNAClassCode
{
    public class GameObject 
    {
        protected Texture2D image;
        public Vector2 position;
        public Color drawColor = Color.White;
        public float scale = 1f, rotation = 0f;
        public float layerDepth = .5f; //Determines the order of what gets drawn first or last. Ranges from 0 - 1. 1 will be drawn first, 0 last.
        public bool active = true;
        protected Vector2 center; //What is the center of the image? If the image is 128 x 128 this would be set to 64 x 64. Use

        public bool collidable = true; //Is this something other objects can collide with?
        protected Rectangle boundingBox;
        protected Vector2 boundingBoxOffset;
        const float gravity = 1f;     // Gravitation pull applied to the player.
        protected Vector2 direction = new Vector2(1, 0); //What direction are we facing?

        Texture2D boundingBoxImage;
        const bool drawBoundingBoxes = false; //Change this to false if you want the bounding boxes to stop being drawn.

        public Vector2 startPosition = new Vector2(-1, -1); //Where we should start at the beginning of a level.

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
        }

        public GameObject() //Constructor, called when a GameObject is created.
        {
        }

        public virtual void Initialize()
        {
            if (startPosition == new Vector2(-1, -1))
                startPosition = position;
        }

        public virtual void SetToDefaultPosition()
        {
            position = startPosition;
        }

        public virtual void Load(ContentManager content)
        {
            boundingBoxImage = TextureLoader.Load("pixel", content);

            CalculateCenter();

            if (image != null)
            {
                boundingBox.Width = image.Width;
                boundingBox.Height = image.Height;
            }
        }

        public virtual void Update(List<GameObject> objects, Map map)
        {
            //We make these functions virtual so other GameObjects can overwrite them with specific functionality.
            //It might seem strange that we leave these functions empty, but since all GameObjects has an Update function,
            //we can simply call Update once and EVERYTHING gets updated! Very convenient, saves a lot of time!
            UpdateBoundingBoxes();
        }

        public virtual void UpdateBoundingBoxes()
        {
            //Update bounding box with our current position:
            boundingBox.X = (int)(position.X + boundingBoxOffset.X);
            boundingBox.Y = (int)(position.Y + boundingBoxOffset.Y);
        }

        /// <summary>
        /// Check to see if the bounding box sent in intersects with ours.
        /// </summary>
        public virtual bool CheckCollision(Rectangle input)
        {
            return boundingBox.Intersects(input);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (boundingBoxImage != null && drawBoundingBoxes == true && active == true)
                spriteBatch.Draw(boundingBoxImage, new Vector2(boundingBox.X, boundingBox.Y), boundingBox, new Color(120, 120, 120, 120), 0f, Vector2.Zero, 1f, SpriteEffects.None, .1f);

            if (image != null && active == true)
                spriteBatch.Draw(image, position, null, drawColor, rotation, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        private void CalculateCenter()
        {
            if (image == null)
                return;

            center.X = image.Width / 2;
            center.Y = image.Height / 2;
        }

        public virtual void BulletResponse()
        {
            //Called whenever this object is hit with a bullet... each individual object will fill this out because each response is different
            //(sometimes a bullet should do nothing, other times kill them or maybe take damage etc.)
        }
    }
}
