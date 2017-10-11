using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FNAClassCode
{
    public class Character : AnimatedObject
    {
        public Vector2 velocity; //How fast we're moving.

        //Customize the feel of movement:
        protected float decel = 1.2f; //The lower your decel is, the slower you slow down.
        protected float accel = .78f; //The lower your accel is, the slower you take off.
        protected float maxSpeed = 5f; //What's the fastest the character move? Character.maxspeed = ++f;

        const float gravity = 1f;     // Gravitation pull applied to the player.
        const float jumpVelocity = 16f; // How much we jump.
        const float maxFallVelocity = 32; //If we fall we need to cap how fast we fall down so we don't go through collidable objects.

        protected bool jumping;

        public override void Initialize()
        {
            velocity = Vector2.Zero;
            jumping = false;
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
        }

        public override void Update(List<GameObject> objects, Map map)
        {
            UpdateMovement(objects, map);
            base.Update(objects, map);
        }

        private void UpdateMovement(List<GameObject> objects, Map map)
        {
            //Stop if we're going to collide this way, then update our position:
            if (velocity.X != 0 && CheckCollisions(map, objects, true) == true)
                velocity.X = 0;

            //Move our position by how fast we're going:
            position.X += velocity.X;

            //Repeat the same thing for our Y axis:
            if (velocity.Y != 0 && CheckCollisions(map, objects, false) == true)
                velocity.Y = 0;

            position.Y += velocity.Y;

            //Now we can add gravity, if it's needed in our game:
            ApplyGravity(map);

            //Slowly decel our velocity so we come to a clean stop:
            velocity.X = TendToZero(velocity.X, decel);
            //velocity.Y = TendToZero(velocity.Y, decel); If your game doesn't need gravity, comment this back in.
        }

        private void ApplyGravity(Map map)
        {
            if (jumping == true || OnGround(map) == Rectangle.Empty)
                velocity.Y += gravity;

            if (velocity.Y > maxFallVelocity)
                velocity.Y = maxFallVelocity;
        }

        protected void MoveRight()
        {
            //Move right, use accel + decel to compensate for when we tend towards zero later:
            velocity.X += accel + decel;

            //Prevent the velocity from ever going over the max speed:
            if (velocity.X > maxSpeed)
                velocity.X = maxSpeed;

            //If we're moving right, update our direction:
            direction.X = 1;
        }

        protected void MoveLeft()
        {
            velocity.X -= accel + decel;

            if (velocity.X < -maxSpeed)
                velocity.X = -maxSpeed;

            direction.X = -1;
        }

        protected void MoveDown()
        {
            velocity.Y += accel + decel;

            if (velocity.Y > maxSpeed)
                velocity.Y = maxSpeed;

            direction.Y = 1;
        }

        protected void MoveUp()
        {
            velocity.Y -= accel + decel;

            if (velocity.Y < -maxSpeed)
                velocity.Y = -maxSpeed;

            direction.Y = -1;
        }
        
        protected virtual bool CheckCollisions(Map map, List<GameObject> objects, bool xAxis)
        {
            //Check our future position with the map's walls, we'll check each axis in seperate calls:
            Rectangle futureBoundingBox = new Rectangle((int)(position.X + boundingBoxOffset.X), (int)(position.Y + boundingBoxOffset.Y), (int)boundingBox.Width, (int)boundingBox.Height);

            //What are the max speeds we can go on each axis?
            int maxX = (int)maxSpeed;
            int maxY = (int)jumpVelocity;

            //Whichever direction we're traveling we want to add the max speed we can possibly travel in that direction:
            if (xAxis == true && velocity.X != 0)
                futureBoundingBox.X += velocity.X > 0 ? maxX : -maxX; //Is velocity.x > than 0 ? If so, add max, otherwise add -max.
            else if (xAxis == false && velocity.Y != gravity)
                futureBoundingBox.Y += velocity.Y > 0 ? maxY : -maxY; 

            Rectangle wallCollision = map.CheckCollision(futureBoundingBox);

            //See if this character has collided with any of the walls on the map:
            if (wallCollision != Rectangle.Empty)
            {
                //If we're making a game with gravity, this first if statement detects if our bounding box hit the top of a wall while falling down. If so we will land on it:
                if (velocity.Y >= gravity && (futureBoundingBox.Bottom > wallCollision.Top - maxSpeed) && (futureBoundingBox.Bottom <= wallCollision.Top + velocity.Y))
                {
                    LandResponse(wallCollision);
                    return true;
                }
                else //If you don't need gravity, comment out the above if block and just return true...
                    return true;
            }

            //Check our future position with the objects in the game:
            for (int i = 0; i < objects.Count; i++)
                if (objects[i] != this && objects[i].active == true && objects[i].collidable == true && objects[i].CheckCollision(futureBoundingBox) == true)
                    return true;

            //Check our future position with the collidable decor:
            for (int i = 0; i < map.decor.Count; i++)
                if (map.decor[i].collidable == true && map.decor[i].CheckCollision(futureBoundingBox) == true)
                    return true;

            //We can move this way; no collisions detected!
            return false;
        }

        protected float TendToZero(float val, float amount)
        {
            if (val > 0f && (val -= amount) < 0f) return 0f;
            if (val < 0f && (val += amount) > 0f) return 0f;
            return val;
        }

        protected Rectangle OnGround(Map map)
        {
            Rectangle futureBoundingBox = new Rectangle((int)(position.X + boundingBoxOffset.X), (int)(position.Y + boundingBoxOffset.Y + (velocity.Y + gravity)), boundingBox.Width, boundingBox.Height); 

            return map.CheckCollision(futureBoundingBox);
        }

        protected bool Jump(Map map)
        {
            if (jumping == true)
                return false;

            if (velocity.Y == 0 && OnGround(map) != Rectangle.Empty) //If we're standing...
            {
                velocity.Y -= jumpVelocity;

                jumping = true;
                return true;
            }

            return false;
        }

        public void LandResponse(Rectangle wallCollision)
        {
            //If we're falling too fast it's possible we can get stuck inside of a wall, so once we've landed on the ground immediately stop us and set our Y position to be on the top of the wall we collided with:
            position.Y = wallCollision.Top - (boundingBox.Height + boundingBoxOffset.Y); //So we always stick to the bottom of the ground.
            velocity.Y = 0;
            jumping = false; //We've landed.

            //Immediately update our bounding boxes since we altered our position to land:
            UpdateBoundingBoxes();
        }
    }
}
