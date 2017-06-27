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
    public class Fireball : Sprite
    {

        public Fireball(Texture2D image, Vector2 startPosition, float velocity, SoundEffect sound)
            : base(image, startPosition, new Vector2(0, velocity),
                    true, 0, 3.0f, SpriteEffects.None, sound)        
         {
             //here sound only plays once when the fireball is
             //fired (created)
             if (sound != null)
             {
                 sound.Play();
             }


           
        }
    }
}
