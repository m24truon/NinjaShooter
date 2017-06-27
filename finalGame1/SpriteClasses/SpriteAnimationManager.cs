using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

namespace SpriteClasses
{
    public class SpriteAnimationManager : Sprite
    {
        //public AnimationManager spriteAnimations = new AnimationManager();

        //dictionary of animations rather than list - with a dictionary, they can have names
        //so it's easier to work with
        public Dictionary<string, Animation> animationDictionary = new Dictionary<string, Animation>();
        //which animation is currently playing
        public string CurrentAnimation { get; set; }

        // animation version
        public SpriteAnimationManager(Texture2D textureImage, Vector2 position, Vector2 velocity, bool setOrigin,
            float rotationSpeed, float scale, SpriteEffects spriteEffect, SoundEffect sound)
            : base(textureImage, position, velocity, setOrigin, rotationSpeed, scale, spriteEffect, sound)
         {   }

        // This method adds an animation to the dictionary
        public void AddAnimation(String animationName, Animation animation)
        {
            animationDictionary.Add(animationName, animation);
        }

        //keep it on the screen
        public override void Update(GameTime gameTime, GraphicsDevice Device)
        {
            //call private method to update animation
            UpdateAndLoadAnimation(gameTime);
            //update the position and rotation, and keep it on the screen
            base.Update(gameTime, Device);
        }

        //don't keep it on the screen
        public override void Update(GameTime gameTime)
        {
            //call private method to update animation
            UpdateAndLoadAnimation(gameTime);
            //update the position and rotation, but don't keep it on the screen
            base.Update(gameTime);
        }

        //updates animation and loads next image into TextureImage for drawing
        private void UpdateAndLoadAnimation(GameTime gameTime)
        {
            if (Alive)
            {
                //update the animation
                animationDictionary[CurrentAnimation].Update(gameTime);
                //load the current image using the animation dictionary's CurrentAnimation name, and the CurrentCell number
                //draw will draw this image next time around
                Image = animationDictionary[CurrentAnimation].cellList[animationDictionary[CurrentAnimation].CurrentCell];
            }
        }
    }
}
