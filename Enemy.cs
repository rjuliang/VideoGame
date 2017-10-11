using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio; //NOTE: We need to use this library since we're playing a sound effect.

namespace FNAClassCode
{
    public class Enemy : Character
    {
        int respawnTimer; //After we're dead we'll wait a bit and then respawn somewhere so the player can shoot us again!
        const int maxRespawnTimer = 60;
        Random random = new Random(); //A random number generator!
        SoundEffect explosion, claps, uuuu; //A sound effect to play when the enemy is hit with a bullet.

        public Enemy()
        {

        }

        public Enemy(Vector2 inputPosition)
        {
            position = inputPosition;
        }

        public override void Initialize()
        {
            active = true;
            collidable = false;
            flipLeftFrames = false;
            flipRightFrames = true;
            position.X = random.Next(0, 1245); //This function returns a random number between the range of 0 and 1279. 1280 is the cap, we'll never get that number or anything above it.

            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            //image = TextureLoader.Load("SingleMexican", content);
            explosion = content.Load<SoundEffect>("Audio\\mexicanAudio");
            claps = content.Load<SoundEffect>("Audio\\aplausos");
            uuuu = content.Load<SoundEffect>("Audio\\uuuu");
            //Load our image/sprite sheet:   
            image = TextureLoader.Load("mexicans1", content);

            //Load any animation stuff if this object animates:   
            LoadAnimation("mexicans", content);
            ChangeAnimation(Animations.RunRight);//Set our default animation.
            
            base.Load(content);

            boundingBoxOffset.X = 0;
            boundingBoxOffset.Y = 0;
            boundingBox.Width = animationSet.width;
            boundingBox.Height = animationSet.height;
        }

        public override void Update(List<GameObject> objects, Map map)
        {
            if (respawnTimer > 0)
            {
                respawnTimer--;
                if (respawnTimer <= 0)
                    Initialize(); //Respawn this enemy!
            }

            if (active == true && position.X > 623)
            {
                MoveLeft();
            }
            else if (active == true && position.X < 623)
            {
                MoveRight();     
            }

            //in the case the mexican gets the jobs building
            if (position.X >= 621 && position.X <= 623)
            {                
                active = false;
                velocity.X = 0;

                if (active == false)
                {
                    Player.score--;
                    Destroy();
                    //maxSpeed = maxSpeed + (maxSpeed / 4);

                    Initialize();
                }
                else
                    return;
            }



            base.Update(objects, map);
        }

        public override void BulletResponse() //If this function is called, that means we've been hit with a bullet!
        {

            active = false;
            respawnTimer = maxRespawnTimer;
            Player.score++;

            /*if (Player.score < 0 && Global.levelName != "Sixth.jorge")
            {
                Global.Game.LoadLevel("Lost.jorge");
                uuuu.Play(1f, 0f, 0f);
            }*/

            if (Player.score == 10 && Global.levelName != "Sixth.jorge")
            {
                Global.Game.LoadLevel("Fourth.jorge");
            }
            else if (Player.score == 50 && Global.levelName == "Sixth.jorge")
            {
                Global.Game.LoadLevel("Seventh.jorge");
                claps.Play(1f, 0f, 0f);
            }

            

                //Time to play a sound effect! There's three different variables we can pass in to customize the sound effect, all values range from 0 to 1.0f.;
                //First variable is the volume we want to play the explosion at, second variable is the pitch, last variable is how much we want to pan the sound to the left or right.
                explosion.Play(1f, 0f, 0f);
            
            base.BulletResponse();
        }
        protected override void UpdateAnimations()
        {
            if (currentAnimation == null)
                return; //Animation wasn't loaded, so return.

            base.UpdateAnimations();

            //Select which animation we should currently be in based on what we're doing:

                if (direction.X < 0 && AnimationIsNot(Animations.RunLeft))
                    ChangeAnimation(Animations.RunLeft);
                else if (direction.X > 0 && AnimationIsNot(Animations.RunRight))
                    ChangeAnimation(Animations.RunRight);
            
        }
        public void Destroy()
        {
            active = false;
        }

    }
}
