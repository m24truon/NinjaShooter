#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using SpriteClasses;
#endregion

namespace NinjaShooter
{
    public enum EnemyType
    {
        Green,
        Blue, 
        Red,
        Arkanoid
    }
    public enum GameState
    {
        Start,
        Running,
        End
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Random random;

        float vel;

        ParallaxBackground background;
        Sprite spriteMain;

        //MouseState prevMouseState;

        //enemies and their fireballs     
        List<Sprite> enemyList;
        Texture2D enemy1;
        Texture2D enemy2;
        Texture2D enemy3;
        //used to determine when to spawn enemies
        float totalTime = 0.0f;
        //spawn times for enemies
        const int SPAWN_TIME_1 = 5;
        const int SPAWN_TIME_2 = 7;
        const int SPAWN_TIME_3 = 11;
        //enemy fireballs
        Texture2D enemyFireballTexture;
        List<Fireball> enemyFireballList = new List<Fireball>();
        //player fireballs and related variables
        Texture2D playerFireballTexture;
        List<Fireball> playerFireballList = new List<Fireball>();
        float timeSinceLastFireball;
        const float TIME_BETWEEN_FIREBALLS = .25f;

        //sound track
        SoundEffect song;
        //sound effects
        SoundEffect fire;
        SoundEffect hit;

        //points our ship has earned
        float points;

        //text related 
        SpriteFont verdana; 
        Vector2 textLeft;
        Vector2 textRight;

        // Here we define our GameState variable
        GameState gameState = GameState.Start;
        // Textures for our splash screen
        Texture2D startScreen;
        Texture2D endScreen;


        Vector2 gravity = new Vector2(0.0f, 300.0f);
        float elapsed;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            random = new Random();
            enemyList = new List<Sprite>();

            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = new ParallaxBackground(GraphicsDevice);
            // Add the space layer to the background, 
            // this is also in ParallaxBackground.cs
            // 2nd parameter is depth (zero being closest to the bottom), 
            // 3rd parameter is moveRate
            try
            {
                background.AddLayer(Content.Load<Texture2D>("Images/citybackground1"), 0.0f, 80.0f);
                background.SetMoveLeftRight();
                background.StartMoving();

                spriteMain = new Character(Content);
                enemy1 = Content.Load<Texture2D>("Images/enemy41");
                enemy2 = Content.Load<Texture2D>("Images/enemy3v31");
                enemy3 = Content.Load<Texture2D>("Images/sprite_1");
                enemyFireballTexture = Content.Load<Texture2D>("Images/enemyfire1");
                playerFireballTexture = Content.Load<Texture2D>("Images/ninjastar1");
                

                //sounds
                //load and play background music
                song = Content.Load<SoundEffect>("Audio/astralWave2");
                //need a soundeffectinstance to make it loop
                //song doesn't always work in Monogame along with soundeffects 
                //- the song sometimes stops when a soundeffect plays
                SoundEffectInstance backSong = song.CreateInstance();
                backSong.IsLooped = true;
                backSong.Play();
                fire = Content.Load<SoundEffect>("Audio/decapacitated");
                hit = Content.Load<SoundEffect>("Audio/deathSound");

                //font and related loads
                verdana = Content.Load<SpriteFont>("Fonts/verdana");
                textRight = new Vector2(GraphicsDevice.Viewport.Width / 1.3f, GraphicsDevice.Viewport.Height / 33);
                textLeft = new Vector2(GraphicsDevice.Viewport.Width / 33, GraphicsDevice.Viewport.Height / 33);

                // Load the start and end screen textures
                startScreen = Content.Load<Texture2D>("Images/splashscreen");
                endScreen = Content.Load<Texture2D>("Images/endsplashscreen1");
            }
            catch (ContentLoadException ex)
            {
                System.Windows.Forms.MessageBox.Show("A file is missing from the content" +
                    " folder: " + ex.Message, ex.GetType().ToString());
                this.Exit();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, ex.GetType().ToString());
                Exit();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            

            background.Update(gameTime);

            //update elapsed time
            float elapsed = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            totalTime += elapsed;
            timeSinceLastFireball += elapsed;

            // If we are starting or ending just update Splash screen
            if (gameState == GameState.Start || gameState == GameState.End)
                UpdateSplashScreen();
            else // GameState is running
            {

                SpawnEnemyTime();
                UpdateInput();
                spriteMain.Update(gameTime, GraphicsDevice);
               //update enemy ships
                //for (int i = enemyShipList.Count - 1; i >= 0 ; i--)
                for (int i = 0; i < enemyList.Count; i++)
                {
                    enemyList[i].Update(gameTime);
                    //added so arkanoid doesn't try to test for firing
                    if (enemyList[i] is Enemy)
                    {
                        //if its firing property is true, create a fireball for the enemy
                        if (((Enemy)enemyList[i]).Firing)
                        {
                            //start fireball 5 pixels away from the enemy
                            Fireball fireball = new Fireball(enemyFireballTexture,
                                new Vector2(enemyList[i].Position.X,
                                        enemyList[i].Position.Y + 35f), 300, null);

                            fireball.Update(gameTime, GraphicsDevice);
                            enemyFireballList.Add(fireball);
                            ((Enemy)enemyList[i]).Firing = false;
                        }
                    }
                    //check for collision between enemy and player's fireball
                    int collide = -1;
                    if (enemyList[i] is Enemy)
                    {
                        //check for collision between green or blue enemy and player's fireball
                        collide = ((Enemy)enemyList[i]).CollisionBall(playerFireballList);
                    }
                    else //if (enemyShipList[i] is SheetEnemy)
                    {
                        //check for collision between arkanoid and player's fireball
                        for (int j = 0; j < playerFireballList.Count; j++)
                        {
                            if (enemyList[i].CollisionSprite(playerFireballList[j]))
                            {
                                collide = j;
                            }
                        }
                    }
                    //if collision, play sound, remove enemy and fireball
                    if (collide != -1)
                    {
                        //add to points when enemy is killed
                        points += 200;
                        enemyList[i].Sound.Play();
                        enemyList.RemoveAt(i);
                        playerFireballList.RemoveAt(collide);
                    }
                    else
                    {
                        //if player has collided with an enemy ship, remove enemy
                        if (spriteMain.CollisionSprite(enemyList[i]))
                        {
                            enemyList.RemoveAt(i);
                            //check for game over
                            if (((Character)spriteMain).Lives <= 0)
                            {
                                gameState = GameState.End;
                            }
                        }
                    }
                }
                //update enemy lasers
                for (int i = 0; i < enemyFireballList.Count; i++)
                //for (int i = enemyFireballList.Count - 1; i >= 0; i--)
                {
                    enemyFireballList[i].Update(gameTime);
                    //check for player collision with enemy laser 
                    if (spriteMain.CollisionSprite(enemyFireballList[i]))
                    {
                        //remove colliding laser from list 
                        enemyFireballList.RemoveAt(i);
                        //check for game over
                        if (((Character)spriteMain).Lives <= 0)
                        {
                            gameState = GameState.End;
                        }
                    }
                }
                //update player fireballs
                for (int i = 0; i < playerFireballList.Count; i++)
                {
                    playerFireballList[i].Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Used to detect and process Sprite input
        /// Called from Update
        /// </summary>
        private void UpdateInput()
        {
            //set keyPressed to false to start.
            //if it's still false after all keys have been tested, 
            //nothing relevant has been pressed
            //and we should idle
            bool keyPressed = false;
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);


            if (keyState.IsKeyDown(Keys.Up)
              || keyState.IsKeyDown(Keys.W)
              || gamePadState.DPad.Up == ButtonState.Pressed
              || gamePadState.ThumbSticks.Left.Y > 0)
            {
                //spriteShip.Up();
                spriteMain.Rotation += 100;
                keyPressed = true;
            }
            if (keyState.IsKeyDown(Keys.Down)
              || keyState.IsKeyDown(Keys.S)
              || gamePadState.DPad.Down == ButtonState.Pressed
              || gamePadState.ThumbSticks.Left.Y < -0.5f)
            {
                //spriteShip.Down();
                spriteMain.Rotation -= 100;
                keyPressed = true;
            }
            if (keyState.IsKeyDown(Keys.Left)
              || keyState.IsKeyDown(Keys.A)
              || gamePadState.DPad.Left == ButtonState.Pressed
              || gamePadState.ThumbSticks.Left.X < -0.5f)
            {
                spriteMain.Left();
                keyPressed = true;

            }
            if (keyState.IsKeyDown(Keys.Right)
              || keyState.IsKeyDown(Keys.D)
              || gamePadState.DPad.Right == ButtonState.Pressed
              || gamePadState.ThumbSticks.Left.X > 0.5f)
            {
                spriteMain.Right();
                keyPressed = true;
            }
            if (!keyPressed)
            {
                spriteMain.Idle();
            }

            //G creates green enemy
            if (keyState.IsKeyDown(Keys.G))   
            {
                    //use enumeration to dictate which enemy to create
                    SpawnEnemy(EnemyType.Green);                   
            }
            //B creates blue enemy
            if (keyState.IsKeyDown(Keys.B))  
            {
                    SpawnEnemy(EnemyType.Blue);
            }

            //add a player fireball to the list if space pressed
            if (gamePadState.Buttons.A == ButtonState.Pressed ||
                gamePadState.Triggers.Right >= 0.5f ||
                keyState.IsKeyDown(Keys.Space))
            {
                //if space bar pressed, check fire delay to see if
                //enough time has passed to fire again
                if (timeSinceLastFireball >= TIME_BETWEEN_FIREBALLS)
                {
                    //fireball will start in center of sprite because draw method
                    //uses SpriteOrigin to offset the image
                    //move it up a bit though, or it looks like it's coming from the middle of the ship
                    FireballUpdate();
                    Fireball shot = new Fireball(playerFireballTexture, 
                        new Vector2(spriteMain.Position.X, spriteMain.Position.Y - 30), -600, fire); //vel was originally -600
                    playerFireballList.Add(shot);
                    //reset fire delay
                    timeSinceLastFireball = 0f;
                    for (int i = 0; i < 200; i++) { 
                        
                    }
                }

            }
                
        }

        public void SpawnEnemyTime()
        {
            //another technique for spawning timing
            //every 5 seconds or so (note the range in case the number isn't exact)
            if (totalTime % SPAWN_TIME_1 > .2f && totalTime % SPAWN_TIME_1 < .22f)
                for (int i = 0; i < 3; i++)
                    SpawnEnemy((EnemyType)random.Next(3));
            //every 7 seconds or so
            if (totalTime % SPAWN_TIME_2 > .2f && totalTime % SPAWN_TIME_2 < .22f)
                for (int i = 0; i < 5; i++)
                    SpawnEnemy((EnemyType)random.Next(3));
            //every 11 seconds or so
            if (totalTime % SPAWN_TIME_3 > .2f && totalTime % SPAWN_TIME_3 < .22f)
                for (int i = 0; i < 3; i++)
                    SpawnEnemy((EnemyType)random.Next(3));
        }

        public void FireballUpdate() {

            foreach (Fireball ball in playerFireballList) {

                ball.Force = gravity;

                ball.Acceleration = ball.Force / 1; // 1 is fireball mass
                ball.Velocity = ball.InitialVelocity + ball.Acceleration * elapsed;
                ball.Position += ball.InitialVelocity * elapsed + 0.5f * ball.Acceleration * elapsed * elapsed;
                vel = ball.Velocity.Y;
            }
            // Do gravity on fireballs

            // s = viT + 1/2aT^2
        
        }

        public void SpawnEnemy(EnemyType enemyType)
        {
            //green enemy moves sideways, but doesn't fire
            if (enemyType == EnemyType.Green)
            {
                Enemy enemy = new Enemy(enemy1, GraphicsDevice, hit);
                //set across movement speed and range
                enemy.SetAcrossMovement((float)random.Next(150), 50.0f);
                enemyList.Add(enemy);
            }
            //blue enemy fires but doesn't move sideways
            if (enemyType == EnemyType.Blue)
            {
                Enemy enemy = new Enemy(enemy2, GraphicsDevice, hit);
                enemy.FiringActive = true;
                enemyList.Add(enemy);
            }

            if (enemyType == EnemyType.Red)
            {
                Enemy enemy = new Enemy(enemy3, GraphicsDevice, hit);
                enemy.FiringActive = true;
                enemyList.Add(enemy);
            }

            //arkanoid is a sprite sheet
            if (enemyType == EnemyType.Arkanoid)
            {
                try
                {
                    Arkanoid enemy = new Arkanoid(GraphicsDevice, Content, hit);
                    enemyList.Add(enemy);
                }
                catch (ContentLoadException ex)
                {
                    System.Windows.Forms.MessageBox.Show("A file is missing from the content" +
                        " folder: " + ex.Message, ex.GetType().ToString());
                    this.Exit();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, ex.GetType().ToString());
                    Exit();
                }
            }
        }

         // Update splash screen, just check for keyboard input
        private void UpdateSplashScreen()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed
                || keyState.IsKeyDown(Keys.Space) && gameState == GameState.Start
                || (keyState.IsKeyDown(Keys.Space) && gameState == GameState.End))
            {
                gameState = GameState.Running;
                totalTime = 0.0f;
                points = 0;
                spriteMain.Position = new Vector2(400.0f, 450.0f);
                enemyFireballList.Clear();
                enemyList.Clear();
                playerFireballList.Clear();
                ((Character)spriteMain).Lives = 7;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            switch (gameState)
            {
                case GameState.Start:
                    // Draw the start screen
                    spriteBatch.Begin();
                    spriteBatch.Draw(startScreen, Vector2.Zero, null, Color.White);
                    spriteBatch.End();
                    break;

                case GameState.Running:
                    spriteBatch.Begin();

                    background.Draw(); //scrolling background - spritebatch is in the draw method
                    spriteMain.Draw(spriteBatch);

                    foreach (Sprite item in enemyList)
                    {
                        item.Draw(spriteBatch);
                    }
                    //draw enemy lasers
                    foreach (Fireball laser in enemyFireballList)
                    {
                        laser.Draw(spriteBatch);
                    }
                    //draw player fireballs
                    foreach (Fireball fireball in playerFireballList)
                    {
                        fireball.Draw(spriteBatch);
                    }

                    //put text on screen
                    //playerShip is declared as a Sprite, so need to cast it to a Ship object 
                    //to get access to its Lives property
                    //Alternatively, change the declaration of it to Ship instead of Sprite
                    string output = "Lives: " + ((Character)spriteMain).Lives.ToString();
                    spriteBatch.DrawString(verdana, output, textLeft, Color.White);
                    string pointString = points.ToString();       
                    pointString = "Points: " + pointString;
                    spriteBatch.DrawString(verdana, pointString, textRight, Color.Red);
                    spriteBatch.End();
                    break;

                case GameState.End:
                    spriteBatch.Begin();
                    spriteBatch.Draw(endScreen, Vector2.Zero, null, Color.White);
                    spriteBatch.End();
                    break;

                default:
                    break;
            }
            base.Draw(gameTime);
        }
    }
}
