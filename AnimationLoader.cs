using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;

namespace FNAClassCode
{
    public static class AnimationLoader
    {
        /// <summary>
        /// Fills out a AnimationData object with all the items we need for the requested level.
        /// </summary>
        public static AnimationData Load(string name)
        {
            //Fill out the AnimationData and return it:
            AnimationData animationData = new AnimationData();
            animationData.animation = new AnimationSet();
            animationData.animation.animationList = new List<Animation>();

            switch (name)
            {
                case "DonaldTrump":
                    {
                        //ShyBoy.txt
                        /*animationData.animation.gridX = 14;
                        animationData.animation.gridY = 8;
                        animationData.animation.width = 128;
                        animationData.animation.height = 128;
                        animationData.animation.animationList = new List<Animation> { new Animation("Run Left", 4, new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, }), new Animation("Run Right", 4, new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, }), new Animation("Idle Left", 16, new List<int> { 98, 99, 100, 101, 102, }), new Animation("Idle Right", 16, new List<int> { 98, 99, 100, 101, 102, }), };
                        */
                        animationData.animation.gridX = 6;
                        animationData.animation.gridY = 3;
                        animationData.animation.width = 144;
                        animationData.animation.height = 150;
                        animationData.animation.animationList = new List<Animation> { new Animation("Run Right", 5, new List<int> { 0, 1, 2, 3, 4, 5, }), new Animation("Run Left", 5, new List<int> { 0, 1, 2, 3, 4, 5, }), new Animation("Idle Right", 5, new List<int> { 6, }), new Animation("Idle Left", 5, new List<int> { 6, }), new Animation("Punch Right", 5, new List<int> { 7, 7, 7, 7, 7 }), new Animation("Punch Left", 5, new List<int> { 7, 7, 7, 7, 7 }), };
                        break;
                    }
                case "orb":
                    {
                        //orbe.txt
                        animationData.animation.gridX = 15;
                        animationData.animation.gridY = 10;
                        animationData.animation.width = 128;
                        animationData.animation.height = 128;
                        animationData.animation.animationList = new List<Animation> { new Animation("orb", 8, new List<int> { 45,46,47,48,49,30,31,32,33,34, }), };
                        break;
                    }
                case "mexicans":
                    {
                        //mexicans.txt
                        animationData.animation.gridX = 4;
                        animationData.animation.gridY = 4;
                        animationData.animation.width = 93;
                        animationData.animation.height = 139;
                        animationData.animation.animationList = new List<Animation> { new Animation("Run Left", 4, new List<int> { 7, 6, 5, 4, }), new Animation("Run Right", 4, new List<int> { 7, 6, 5, 4, }), };
                        break;
                    }
                case "islamics":
                    {
                        //islamicguycode.txt
                        animationData.animation.gridX = 6;
                        animationData.animation.gridY = 1;
                        animationData.animation.width = 111;
                        animationData.animation.height = 128;
                        animationData.animation.animationList = new List<Animation> { new Animation("Idle Left", 11, new List<int> { 0, 1, 2, 3, 4, 5, }), new Animation("Idle Right", 11, new List<int> { 0, 1, 2, 3, 4, 5, }), };
                        break;
                    }
               
            }
            //8, 9, 10, 11, 
            //
            return animationData;
        }
    }
}
