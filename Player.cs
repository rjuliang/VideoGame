using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FNAClassCode
{
    public class Player : FireCharacter //We are inheriting from the GameObject class by putting this here. We will automatically receive all of GameObject's functions and variables.
    {
        public static int score;
        bool kick = false;
        public bool die;
        SoundEffect song, yell;
        SoundEffectInstance songInstance;
        int standTimer; //After we're dead we'll wait a bit and then respawn somewhere so the player can shoot us again!
        const int maxstandTimer = 180;
        public Player()
        {
        }

        public Player(Vector2 inputPosition) //So we can pass a starting position to the player when we create him/her.
        {
            position = inputPosition;
            
        }

        override public void Initialize()
        {
            score = 0;
            collidable = false;
            flipRightFrames = false;
            flipLeftFrames = true;
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            //Load our sprite sheet:
            image = TextureLoader.Load("trumpSpritesheet", content);
            //Load our animation stuff:
            LoadAnimation("DonaldTrump", content);
            ChangeAnimation(Animations.RunLeft);



            //Load any stuff from our parent class:
            base.Load(content);

            //Load song to play:
            song = content.Load<SoundEffect>("Audio\\Vivacity");
            yell = content.Load<SoundEffect>("Audio\\yellTrump");
            if (songInstance == null)
                songInstance = song.CreateInstance();

            //Customize the size of our bounding box, each object will probably want a slightly different size:
            boundingBoxOffset.X = 0;
            boundingBoxOffset.Y = 0;
            boundingBox.Width = animationSet.width;
            boundingBox.Height = animationSet.height;
        }

        public override void Update(List<GameObject> objects, Map map)
        {
            
            CheckInput(objects, map);
            UpdateMusic();
            
            base.Update(objects, map);
        }

        private void UpdateMusic()
        {
            //If no music is playing, play that dank beat:
            if (songInstance.State != SoundState.Playing)
                songInstance.Play();

            //Change this bool to make the music loop:
            MediaPlayer.IsRepeating = true;

            //Volume can range from 0 to 1:
            MediaPlayer.Volume = 0.7f;

            //Press P to pause and unpause the music:
            if (Input.KeyPressed(Keys.P))
            {
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();
            }
        }

        public void StopMusic()
        {
            if (songInstance != null && songInstance.State == SoundState.Playing)
                songInstance.Stop();
        }
        
        private void CheckInput(List<GameObject> objects, Map map)
        {
            //Update movement:
            if (Input.IsKeyDown(Keys.D))
                MoveRight();
            else if (Input.IsKeyDown(Keys.A))
                MoveLeft();

            //If our game needs to move in all four directions, use this code:
            //if (Input.IsKeyDown(Keys.S))
            //    MoveDown();
            //else if (Input.IsKeyDown(Keys.W))
            //    MoveUp();

            //If our player needs to jump, use this code:
            if (Input.KeyPressed(Keys.W))
                Jump(map);

            //If our player needs to fire a bullet, use this code:
            if (Input.KeyPressed(Keys.Space))
                Fire();
            if (Input.KeyPressed(Keys.K))
            {
                Punch(map, objects);
                yell.Play(1f, 0f, 0f);
            }
                

            if (position.X >= 600 && position.X <= 620 && velocity.X == 0)
            {
                if (standTimer > 0)
                {
                    standTimer--;
                    if (standTimer <= 0)
                        Player.score--;
                }
                
            }
        }
        protected void Punch(Map map, List<GameObject> objects)
        {
            //change the animation
            //objects list and see distance
            for (int i =0; i<objects.Count; i++)
            {
                if (objects[i] is IslamicEnemy)
                {
                    float distance = Vector2.Distance(position, objects[i].position);
                    if (distance < 96)
                    {
                        ((IslamicEnemy)objects[i]).Destroy();
                        //((IslamicEnemy)objects[i]).Initialize();
                        //((IslamicEnemy)objects[i]).active = true;
                    }
                }
            }
            
            kick = true;

            //ChangeAnimation(Animations.PunchLeft);
        }
        protected override void UpdateAnimations()
        {
            if (currentAnimation == null)
                return; //Animation wasn't loaded, so return.
            base.UpdateAnimations();
            
            //Select which animation we should currently be in based on what we're doing:
            if (velocity != Vector2.Zero || jumping == true)
            {
                if (direction.X < 0 && AnimationIsNot(Animations.RunLeft))
                    ChangeAnimation(Animations.RunLeft);
                else if (direction.X > 0 && AnimationIsNot(Animations.RunRight))
                    ChangeAnimation(Animations.RunRight);
            }
            else if (velocity == Vector2.Zero && jumping == false && kick == false)
            {
                if (direction.X < 0 && AnimationIsNot(Animations.IdleLeft))
                    ChangeAnimation(Animations.IdleLeft);
                else if (direction.X > 0 && AnimationIsNot(Animations.IdleRight))
                    ChangeAnimation(Animations.IdleRight);
            }
            else if (velocity == Vector2.Zero)
            {
                if (direction.X < 0 && AnimationIsNot(Animations.IdleLeft))
                    ChangeAnimation(Animations.IdleLeft);
                else if (direction.X > 0 && AnimationIsNot(Animations.IdleRight))
                    ChangeAnimation(Animations.IdleRight);
            }
            if (kick == true)
            {
                if (direction.X < 0 && AnimationIsNot(Animations.PunchLeft))
                    ChangeAnimation(Animations.PunchLeft);
                else if (direction.X > 0 && AnimationIsNot(Animations.PunchRight))
                    ChangeAnimation(Animations.PunchRight);
                kick = false;
            }
        }
    }
}
