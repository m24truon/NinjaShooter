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
    public class SpriteFromSheet : Sprite
    {
        protected Vector2 frameSize;      // Size of frames in sprite sheet
        public Vector2 FrameSize
        {
            get { return frameSize; }
            set { frameSize = value; }
        }
        protected Vector2 currentFrame;   // Index of current frame in sprite sheet
        public Vector2 CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }
        protected Vector2 sheetSize;      // Number of columns and rows in the sprite sheet
        public Vector2 SheetSize
        {
            get { return sheetSize; }
            set { sheetSize = value; }
        }
        // Time when we need to goto next frame
        private float TimeBetweenFrames { get; set; }
        // time so far towards next frame
        private float timeSinceLastFrame { get; set; }
        // total Time for whole sheet to play
        public float TotalSheetTime { get; set; }

        //rectangle occupied by texture
        public override Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), 
                    (int)(FrameSize.X * Scale), (int)(FrameSize.Y * Scale));
            }
        }

        //sprite sheet version
        public SpriteFromSheet(Texture2D textureImage, Vector2 position, Vector2 velocity, 
            bool setOrigin, float rotationSpeed, float scale, SpriteEffects spriteEffect, SoundEffect sound,
            Vector2 frameSize, Vector2 currentFrame, Vector2 sheetSize, float totalTime)
            : base(textureImage, position, velocity, setOrigin, rotationSpeed, scale, spriteEffect, sound)
        {
            FrameSize = frameSize;
            CurrentFrame = currentFrame;
            SheetSize = sheetSize;
            //time to next frame is total time to play the sheet divided by number of frames
            TotalSheetTime = totalTime;
            TimeBetweenFrames = TotalSheetTime / (sheetSize.X * sheetSize.Y);
            if (UseOrigin)
            {
                Origin = new Vector2(FrameSize.X / 2, FrameSize.Y / 2);
            }
        }        

        //loops through entire spritesheet, called by autosprites in dodgeblade so they
        //can use polymorphism and check against player position
        public virtual void Update(GameTime gameTime, Vector2 player)
        {
            Update(gameTime);
        }

        //loops through entire spritesheet, called by autosprites in dodgeblade so they
        //can use polymorphism and check against player position
        //--this version keeps sprite on screen
        public virtual void Update(GameTime gameTime, GraphicsDevice Device, Vector2 player)
        {
            Update(gameTime, Device);
        }

        //loops through entire spritesheet
        public override void Update(GameTime gameTime)
        {
            if (Alive)
            {
                //call base method to do rotation and basic movement
                base.Update(gameTime);
                //how long since last frame displayed
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //if it's time to display the next one, do so, then reset time back to 0
                if (timeSinceLastFrame > TimeBetweenFrames)
                {
                    if (currentFrame.X >= sheetSize.X - 1)
                    {
                        currentFrame.X = 0;
                        if (currentFrame.Y >= sheetSize.Y - 1)
                        {
                            currentFrame.Y = 0;
                        }
                        else
                        {
                            currentFrame.Y++;
                        }
                    }
                    else
                    {
                        currentFrame.X++;
                    }
                    timeSinceLastFrame = 0f;
                }
            }
        }

        //loops through entire spritesheet
        //--this version keeps sprite on screen
        public override void Update(GameTime gameTime, GraphicsDevice Device)
        {
            if (Alive)
            {
                //call overload to do rotation, basic movement and frame updating
                Update(gameTime);
                //keep on screen
                if (Position.X > Device.Viewport.Width - FrameSize.X + Origin.X)
                {
                    //GraphicsDevice.Viewport.Width or Window.ClientBounds.Width will both give us the width of the screen
                    position.X = Device.Viewport.Width - FrameSize.X + Origin.X;
                    velocity.X = -Velocity.X;
                }
                else if (Position.X < Origin.X)
                {
                    position.X = Origin.X;
                    velocity.X = -Velocity.X;
                }

                if (Position.Y > Device.Viewport.Height - FrameSize.Y + Origin.Y)
                {
                    position.Y = Device.Viewport.Height - FrameSize.Y + Origin.Y;
                    velocity.Y = -Velocity.Y;
                }
                else if (Position.Y < Origin.Y)
                {
                    position.Y = Origin.Y;
                    velocity.Y = -Velocity.Y;
                }
            }
        }

        //loops through one row of spritesheet (first row is row 0)
        public virtual void Update(GameTime gameTime, GraphicsDevice Device, int row)
        {
            if (Alive)
            {
                base.Update(gameTime, Device);
                //how long since last frame displayed
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //if it's time to display the next one, do so, then reset time back to 0
                if (timeSinceLastFrame > TimeBetweenFrames)
                {
                    currentFrame.Y = row;

                    if (currentFrame.X >= sheetSize.X - 1)
                    {
                        currentFrame.X = 0;
                    }
                    else
                    {
                        currentFrame.X++;
                    }
                    timeSinceLastFrame = 0f;
                }
            }
        }

        //loops through one row of spritesheet (first row is row 0)
        //--this version keeps sprite on screen
        public virtual void Update(GameTime gameTime, int row)
        {
            if (Alive)
            {
                base.Update(gameTime);
                //how long since last frame displayed
                timeSinceLastFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //if it's time to display the next one, do so, then reset time back to 0
                if (timeSinceLastFrame > TimeBetweenFrames)
                {
                    currentFrame.Y = row;

                    if (currentFrame.X >= sheetSize.X - 1)
                    {
                        currentFrame.X = 0;
                    }
                    else
                    {
                        currentFrame.X++;
                    }
                    timeSinceLastFrame = 0f;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                spriteBatch.Draw(Image,
                                 Position,
                                 new Rectangle((int)(currentFrame.X * frameSize.X),
                                               (int)(currentFrame.Y * frameSize.Y),
                                               (int)frameSize.X,
                                               (int)frameSize.Y),
                                 Color.White,
                                 Rotation,
                                 Origin,
                                 Scale,
                                 SpriteEffect,
                                 0);
            }
        }

        public override bool IsOffScreen(GraphicsDevice Device)
        {
            if (Position.X < -FrameSize.X * Scale ||
                Position.X > Device.Viewport.Width ||
                Position.Y < -FrameSize.Y * Scale ||
                Position.Y > Device.Viewport.Height)
            {
                return true;
            }
            return false;
        }


    }
}
