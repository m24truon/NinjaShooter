using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Character : SpriteAnimationManager
    {
        //all these used for flashing and 
        //recovery after being hit           
        private bool flashOn = false;               //will be drawn only if true
        private bool recoveringActive = false;      //is it in recovery state or not
        private float recoverTimeTotal = 0.0f;      //how much of the recovery time has currently passed
        private const float RECOVER_LENGTH = 1.50f;  //total recovery time we want
        private float timeSinceLastFlash = 0.0f;    //time since last flash on or off
        const float TIME_BETWEEN_FLASHES = .1f;    //time we want between on and off flashes
        public int Lives { get; set; }

        //make the cells static so they will only get loaded once,
        //no matter how many objects are created - saves memory
        private static Texture2D[] cells;

        public Character(ContentManager content)
            : base(content.Load<Texture2D>("Images/hero11"),
                    new Vector2(100, 100), new Vector2(10, 10), true, 0, 2.5f, SpriteEffects.None, null)
        {
            Lives = 7;

            //first time only (when cell array is null), 
            //initialize array and load the images
            if (cells == null)
            {
                //initialize static array of cells
                cells = new Texture2D[2];
                //can use a loop with a call to StringUtilities.NextImageName
                //if there are a lot of cells that use our naming convention 
                //- we did this in Lab 6
                try
                {
                    cells[0] = content.Load<Texture2D>("images/hero11");
                    cells[1] = content.Load<Texture2D>("images/hero12");
                }
                catch (ContentLoadException ex1)
                {
                    throw ex1;
                }
                catch (Exception ex2)
                {
                    throw ex2;
                }
            }

            //create one animation at a time and add to animation list
            //--need an animation object to work with
            Animation animation = new Animation();
            //can use a loop for this if there are a lot of cells
            animation.AddCell(cells[0]);
            AddAnimation("idle", animation);

            //use the same animation object, but reinitialize it for the next animation
            animation = new Animation();
            animation.AddCell(cells[1]);
            //only right is needed, for left, we can flip the image
            AddAnimation("right", animation);

            //start off idling
            CurrentAnimation = "idle";
            //statement like the following needed 
            //if the first animation contains a bunch of cells
            //(not needed for ship since it only has a one cell animation)
            //animationDictionary["idle"].LoopAll(1.5f);
        }

        public override void Update(GameTime gameTime, GraphicsDevice Device)
        {
            base.Update(gameTime, Device);

            //if he's been hit and is recovering
            if (recoveringActive)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0F;
                timeSinceLastFlash += elapsed;
                recoverTimeTotal += elapsed;
                //if we've been recovering longer than RECOVER_LENGTH, stop
                //recovery
                if (recoverTimeTotal > RECOVER_LENGTH)
                    recoveringActive = false;
                //still recovering, see if it's time to change flashing
                else
                {
                    //flashOn determines if the player will be drawn
                    //or not next time - this toggles it on and off
                    //every .25 seconds so the player flashs
                    if (timeSinceLastFlash > TIME_BETWEEN_FLASHES)
                    {
                        flashOn = !flashOn;
                        timeSinceLastFlash = 0.0f;
                    }
                }
            }
        }

        // Draw the player
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!recoveringActive ||
                (recoveringActive && !flashOn))
            {
                base.Draw(spriteBatch);
            }
        }

        public override bool CollisionSprite(Sprite sprite)
        {
            if (base.CollisionSprite(sprite))
            {
                if (!recoveringActive)
                {
                    //decrease lives left
                    Lives--;
                    //set up recovery
                    recoveringActive = true;
                    recoverTimeTotal = 0.0f;
                }
                return true;
            }
            return false;
        }

        // These match up with the Arrow keys
        public override void Up()
        {
            base.Up();
            SpriteEffect = SpriteEffects.None;
            CurrentAnimation = "idle";
            //statement like the following needed if the animation contains a bunch of cells
            //(not needed for ship since it only has a one cell animation)
            //animationDictionary["idle"].LoopAll(1.5f);
        }

        public override void Down()
        {
            base.Down();
            SpriteEffect = SpriteEffects.None;
            CurrentAnimation = "idle";
            //statement like the following needed 
            //if the animation contains a bunch of cells
            //(not needed for ship since it only has a one cell animation)
            //animationDictionary["idle"].LoopAll(1.5f);
        }

        public override void Right()
        {
            base.Right();
            SpriteEffect = SpriteEffects.None;
            CurrentAnimation = "right";
            //statement like the following needed if the animation contains a bunch of cells
            //(not needed for ship since it only has a one cell animation)
            //animationDictionary["right"].LoopAll(5f);
        }

        public override void Left()
        {
            base.Left();
            SpriteEffect = SpriteEffects.FlipHorizontally;
            CurrentAnimation = "right";
            //statement like the following needed if the animation contains a bunch of cells
            //(not needed for ship since it only has a one cell animation)
            //animationDictionary["right"].LoopAll(5f);
        }

        public override void Idle()
        {
            base.Idle();
            SpriteEffect = SpriteEffects.None;
            CurrentAnimation = "idle";
            //statement like the following needed if the animation contains a bunch of cells
            //(not needed for ship since it only has a one cell animation)
            //animationDictionary["idle"].LoopAll(1.5f);
        }


    }
}
