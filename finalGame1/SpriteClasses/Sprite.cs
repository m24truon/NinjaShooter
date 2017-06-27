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
    public class Sprite
    {
        
        // This variable will hold our position - make it a property so game class
        //can use it to change position when mouse moved
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        protected Vector2 acceleration;
        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        protected Vector2 force;
        public Vector2 Force
        {
            get { return force; }
            set { force = value; }
        }
        public Texture2D Image { get; set; }
        //origin of sprite, either null or the center of the image
        protected Vector2 origin;
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        //tells us whether or not to set the origin. 
        //it needs to be set if we are scaling or rotating the sprite
        public bool UseOrigin { get; set; }
        //rectangle occupied by texture - bounding rectangle
        public virtual Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y),
                    (int)(Image.Width * Scale), (int)(Image.Height * Scale));
            }
        }

        //vector so it has independant x and y values
        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        //velocity set in constructor, needs a 
        //separate property so it doesn't get zeroed
        //when sprite idles
        protected Vector2 initialVelocity;
        public Vector2 InitialVelocity
        {
            get { return initialVelocity; }
            set { initialVelocity = value; }
        }
        //current rotation value
        public float Rotation { get; set; }
        //speed of rotation of the sprite
        public float RotationSpeed { get; set; }
        public float Scale { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        //sound effect when enemy is hit
        //here, it needs to be a property
        //since it doesn't play until he gets hit
        public SoundEffect Sound { get; set; }

        //is he active or not (should he be updated and drawn?)
        public bool Alive { get; set; }     

        // base version
        public Sprite(Texture2D textureImage, Vector2 position, Vector2 velocity, bool useOrigin,
            float rotationSpeed, float scale, SpriteEffects spriteEffect, SoundEffect sound)
        {
            
            Position = position;
            Image = textureImage;
            InitialVelocity = velocity;
            Velocity = velocity;
            UseOrigin = useOrigin;
            if (UseOrigin)
            {
                Origin = new Vector2(Image.Width/2, Image.Height/2);
            }
            RotationSpeed = rotationSpeed;
            Scale = scale;            
            SpriteEffect = spriteEffect;
            Sound = sound;
            Alive = true;
        }
        //version that does not keep sprite on screen
        public virtual void Update(GameTime gameTime)
        {
           
            if (Alive)
            {

                // time between frames
                float timeLapse = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                //move the sprite
                position += Velocity * timeLapse;
                
                // Scale radians by time between frames so rotation is uniform
                // rate on all systems. Cap between 0 & 2PI for full rotation.
                Rotation += RotationSpeed * timeLapse;
                Rotation = Rotation % (MathHelper.Pi * 2.0f);
            }
        }
        //version that keeps sprite on screen
        public virtual void Update(GameTime gameTime, GraphicsDevice Device)
        {
            float timeLapse = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (Alive)
            {
               
                //call overload to do rotation and basic movement
                Update(gameTime);
                //keep on screen
                if (Position.X > Device.Viewport.Width - Origin.X * Scale)
                {
                    //GraphicsDevice.Viewport.Width or Window.ClientBounds.Width will both give us the width of the screen
                    position.X = Device.Viewport.Width - Origin.X * Scale;
                    velocity.X = -Velocity.X;
                }
                else if (Position.X < Origin.X * Scale)
                {
                    position.X = Origin.X * Scale;
                    velocity.X = -Velocity.X;
                }

                if (Position.Y > Device.Viewport.Height - Origin.Y * Scale)
                {
                    position.Y = Device.Viewport.Height - Origin.Y * Scale;
                    velocity.Y = -Velocity.Y;
                }
                else if (Position.Y < Origin.Y * Scale)
                {
                    position.Y = Origin.Y * Scale;
                    velocity.Y = -Velocity.Y;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                spriteBatch.Draw(Image,
                     Position,
                     null,
                     Color.White,
                     Rotation,
                     Origin,
                     Scale,
                     SpriteEffect,
                     0);
                }
        }

        //is there a collision with another sprite?
        public virtual bool CollisionSprite(Sprite sprite)
        {
            return CollisionRectangle.Intersects(sprite.CollisionRectangle);
        }
        //is there a collision with the mouse?
        public bool CollisionMouse(int x, int y)
        {
            return CollisionRectangle.Contains(x, y);
        }

        //used to flip images if we only have right animation
        public void SetMoveLeft()
        {
            SpriteEffect = SpriteEffects.FlipHorizontally;
        }
        public void SetMoveRight()
        {
            SpriteEffect = SpriteEffects.None;
        }

        // These match up with the Arrow keys
        public virtual void Up()
        {
            velocity.Y -= InitialVelocity.Y;
        }

        public virtual void Down()
        {
            velocity.Y += InitialVelocity.Y;
        }

        public virtual void Right()
        {
            velocity.X += InitialVelocity.X;
        }

        public virtual void Left()
        {
            velocity.X -= InitialVelocity.X;
        }

        public virtual void Idle()
        {
            Velocity *= .95f;
        }

        //determine if sprite has gone off screen
        public virtual bool IsOffScreen(GraphicsDevice Device)
        {
            if (Position.X < -Image.Width * Scale ||
                Position.X > Device.Viewport.Width ||
                Position.Y < -Image.Height * Scale ||
                Position.Y > Device.Viewport.Height)
            {
                return true;
            }
            return false;
        }


    }
}

