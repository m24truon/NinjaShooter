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
    public class Arkanoid : SpriteFromSheet
    {
        public Arkanoid(GraphicsDevice Device, ContentManager Content, SoundEffect sound)
            : base(Content.Load<Texture2D>("Images/arkanoidsheet"), new Vector2(Game1.random.Next(0, Device.Viewport.Width - 100), 0),
                        new Vector2(0, 100), true, 0, .5f, SpriteEffects.None, sound,
                        new Vector2(200, 99), new Vector2(0, 0), new Vector2(2, 8), 5.3f)
        {  }

    }
}
