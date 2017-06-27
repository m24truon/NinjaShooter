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
using SpriteClasses;

namespace NinjaShooter
{
    public class Enemy : Sprite
    {
        public Enemy(Texture2D textureImage, GraphicsDevice Device, SoundEffect sound)
            : base(textureImage, new Vector2(Game1.random.Next(0, Device.Viewport.Width - textureImage.Width), 0),
                        new Vector2(0, 50), true, 0, 2.0f, SpriteEffects.None, sound)
        {  }

        //bound and offset of how far it will move across screen
        private float xLength = 0.0f;
        private float xStart = 0.0f;

        //these control whether the enemy fires or not
        //and how fast
        private bool firingActive;
        private bool firing;
        private const float TIME_BETWEEN_FIRES = 1.0f;
        private float timeSinceLastFire = 0.0f;

        //used for collision detection
        float radius = 40.0f;

        public bool FiringActive
        {
            get { return firingActive; }
            set { firingActive = value; }
        }

        public bool Firing
        {
            get { return firing; }
            set { firing = value; }
        }

        public void SetAcrossMovement(float deltaX, float xLength)
        {
            //deltaX is the speed it will move across
            velocity.X = deltaX;
            //xLength is the width of the area it will stay inside
            this.xLength = xLength;
            //it will start where it is now
            xStart = Position.X;
        }
        //used for bounding sphere collisions
        public float Radius
        {
            get { return radius; }
        }

        //uses bounding sphere collisions
        //check for collision with a fireball (from Sprite)
        //return index # of fireball we collided with if there's a collision,
        //-1 if no collision
        public int CollisionBall(List<Fireball> fireballList)
        {
            for (int i = 0; i < fireballList.Count; i++)
            {
                //don't need to sum the radii since we consider the ball a point
                if ((fireballList[i].Position - Position).Length() < Radius)
                { 
                    return i;
                }
            }
            return -1;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //when it's at the end of the range, reverse it's direction
            if (Position.X < xStart - xLength || Position.X > xStart + xLength)
            {
                velocity.X *= -1.0f;
            }
            //set firing to true if they are set to fire 
            //and enough time has elapsed 
            if (FiringActive)
            {
                // time between frames
                timeSinceLastFire += (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                if (timeSinceLastFire >= TIME_BETWEEN_FIRES)
                {
                    timeSinceLastFire = 0.0f;
                    Firing = true;
                }
            }
        }
    }
}
