using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private const float PlayerMoveSpeed = 4;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int Score;

        Player _player;

        Texture2D bulletTexture;
        Texture2D enemybulletTexture;
        Texture2D healthbulletTexture;
        List<Bullet> Bullets;
        List<Bullet> EnemyBullets;

        SpriteFont font;

        GamePadState _currentGamePadState;
        GamePadState _prevGamePadState;

        TimeSpan bulletSpawnTime;
        TimeSpan previousBulletSpawnTime;

        TimeSpan HurtTime;
        TimeSpan LastHurt;

        TimeSpan BigTime;
        TimeSpan LastBig;

        TimeSpan SpeedUp;
        TimeSpan LastSpedUp;

        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        //Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        Random random;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferHeight = 1080;
            //graphics.PreferredBackBufferWidth = 1920;
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
            Bullets = new List<Bullet>();
            EnemyBullets = new List<Bullet>();
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 1000f;
            bulletSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousBulletSpawnTime = TimeSpan.Zero;
            Score = 0;

            _player = new Player();

            enemies = new List<Enemy>();

            //used to determine how fast the enemies will respawn.
            enemySpawnTime = TimeSpan.FromSeconds(.5f);
            SpeedUp = TimeSpan.FromSeconds(2);
            LastSpedUp = TimeSpan.Zero;
            LastHurt = TimeSpan.Zero;
            HurtTime = TimeSpan.FromSeconds(1);
            LastBig = TimeSpan.Zero;
            BigTime = TimeSpan.FromSeconds(.5);

            random = new Random();

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

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            var playerPosition = new Vector2(titleSafeArea.X, titleSafeArea.Y + titleSafeArea.Height / 2);

            Texture2D playerTexture = Content.Load<Texture2D>("player");
            Animation playerAnimation = new Animation();
            playerAnimation.Initialize(playerTexture, playerPosition, 50, 50, 1, 30, Color.White, true, 0, 0);

            _player.Initialize(playerAnimation, playerPosition);

            bulletTexture = this.Content.Load<Texture2D>("bullet");
            enemybulletTexture = this.Content.Load<Texture2D>("enemybullet");
            healthbulletTexture = this.Content.Load<Texture2D>("healthbullet");
            font = Content.Load<SpriteFont>("Font1");

            enemyTexture = Content.Load<Texture2D>("enemy");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            _prevGamePadState = _currentGamePadState;
            // Read the current state of the keyboard and gamepad and store it.
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
            {
                _player.SizeUp(2);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
            {
                _player.SizeUp(-2);
            }
            if (gameTime.TotalGameTime - LastSpedUp > SpeedUp)
            {
                LastSpedUp = gameTime.TotalGameTime;

                foreach (var e in enemies)
                {
                    //e.enemyMoveSpeed += 5;
                }
            }

            UpdateBullets(gameTime);

            UpdatePlayer(gameTime);

            UpdateEnemies(gameTime);

            UpdateCollisions(gameTime);

            base.Update(gameTime);
        }

        protected void UpdateEnemies(GameTime gameTime)
        {
            // spawn a new enemy every 1.5 seconds.
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime && enemies.Count < 2)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // add an enemy
                AddEnemy();
            }

            // update the enemies
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime, GraphicsDevice);
                if (!enemies[i].Active)
                {
                    enemies.Remove(enemies[i]);
                }
                else
                {
                    FireEnemyBullet(gameTime, enemies[i]);
                }
            }
        }

        protected void AddEnemy()
        {
            // create the animation object
            Animation enemyAnimation = new Animation();

            // Init the animation with the correct 
            // animation information
            enemyAnimation.Initialize(enemyTexture,
                Vector2.Zero,
                100,
                100,
                1,
                30,
                Color.White,
                true, 0, 0);

            // randomly generate the postion of the enemy
            Vector2 position;

            float directionX = 0;
            float directionY = 0;

            var rndNum = random.Next(99);

            if (rndNum < 25)
            {
                position = new Vector2(
                -100,
                random.Next(100, GraphicsDevice.Viewport.Height - 100));

                directionX = random.Next(100);
                directionY = position.Y < 200 ? random.Next(100) : (position.Y < (GraphicsDevice.Viewport.Height - 200) ? random.Next(-100, 0) : random.Next(-100, 100));
            }
            else if (rndNum >= 25 && rndNum < 50)
            {
                position = new Vector2(
                random.Next(100, GraphicsDevice.Viewport.Width - 100),
                -100);

                directionX = position.X < 200 ? random.Next(100) : (position.X < (GraphicsDevice.Viewport.Width - 200) ? random.Next(-100, 0) : random.Next(-100, 100));
                directionY = random.Next(100);
            }
            else if (rndNum >= 50 && rndNum < 75)
            {
                position = new Vector2(
                GraphicsDevice.Viewport.Width,
                random.Next(100, GraphicsDevice.Viewport.Height - 100));

                directionX = random.Next(-100, 0);
                directionY = position.Y < 200 ? random.Next(100) : (position.Y < (GraphicsDevice.Viewport.Height - 200) ? random.Next(-100, 0) : random.Next(-100, 100));
            }
            else
            {
                position = new Vector2(
                random.Next(100, GraphicsDevice.Viewport.Width - 100),
                GraphicsDevice.Viewport.Width);

                directionX = position.X < 200 ? random.Next(100) : (position.X < (GraphicsDevice.Viewport.Width - 200) ? random.Next(-100, 0) : random.Next(-100, 100));
                directionY = random.Next(-100, 0);
            }

            // create an enemy
            Enemy enemy = new Enemy();

            // init the enemy
            enemy.Initialize(enemyAnimation, position, directionX, directionY);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);

        }

        void UpdatePlayer(GameTime gameTime)
        {
            _player.Update(gameTime);

            // Thumbstick controls
            _player.Position.X += _currentGamePadState.ThumbSticks.Left.X * PlayerMoveSpeed;
            _player.Position.Y -= _currentGamePadState.ThumbSticks.Left.Y * PlayerMoveSpeed;

            if (_currentGamePadState.ThumbSticks.Right.X != 0 || _currentGamePadState.ThumbSticks.Right.Y != 0)
            {
                FireBullet(gameTime, _currentGamePadState.ThumbSticks.Right.X, _currentGamePadState.ThumbSticks.Right.Y * -1);
            }

            // Make sure that the player does not go out of bounds
            _player.Position.X = MathHelper.Clamp(_player.Position.X, 0, GraphicsDevice.Viewport.Width - _player.Width);
            _player.Position.Y = MathHelper.Clamp(_player.Position.Y, 0, GraphicsDevice.Viewport.Height - _player.Height);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (!_player.Active)
            {
                spriteBatch.DrawString(font, "You died!", new Vector2(200, 200), Color.Red);
            }
            else
            {
                _player.Draw(spriteBatch);

                foreach (var b in Bullets)
                {
                    b.Draw(spriteBatch);
                }

                foreach (var e in enemies)
                {
                    e.Draw(spriteBatch);
                };

                foreach (var b in EnemyBullets)
                {
                    b.Draw(spriteBatch);
                }

                spriteBatch.DrawString(font, "Health: " + _player.Health.ToString(), new Vector2(20, 20), Color.Blue);
            }

            spriteBatch.DrawString(font, "Score: " + Score.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.Width - 100, 20), Color.Blue);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void UpdateBullets(GameTime gameTime)
        {

            // update laserbeams
            for (var i = 0; i < Bullets.Count; i++)
            {
                var b = Bullets[i];
                b.Update(gameTime);

                // Remove the beam when its deactivated or is at the end of the screen.
                if (!b.Active || b.Position.X > GraphicsDevice.Viewport.Width || b.Position.X < -50 || b.Position.Y > GraphicsDevice.Viewport.Height || b.Position.Y < -50)
                {
                    Bullets.Remove(b);
                }
            }

            for (var i = 0; i < EnemyBullets.Count; i++)
            {
                var eb = EnemyBullets[i];

                eb.Update(gameTime);

                // Remove the beam when its deactivated or is at the end of the screen.
                if (!eb.Active || eb.Position.X > GraphicsDevice.Viewport.Width || eb.Position.X < -50 || eb.Position.Y > GraphicsDevice.Viewport.Height || eb.Position.Y < -50)
                {
                    EnemyBullets.Remove(eb);
                }
            }
        }

        protected void FireBullet(GameTime gameTime, float X, float Y)
        {
            // govern the rate of fire for our lasers
            if (gameTime.TotalGameTime - previousBulletSpawnTime > bulletSpawnTime)
            {
                previousBulletSpawnTime = gameTime.TotalGameTime;

                // Add the laer to our list.
                AddBullet(X, Y);
            }

        }

        protected void FireEnemyBullet(GameTime gameTime, Enemy enemy)
        {
            // govern the rate of fire for our lasers
            if (gameTime.TotalGameTime - enemy.LastFired > enemy.TimeToFire)
            {
                enemy.LastFired = gameTime.TotalGameTime;

                bool healthbullet = random.Next(100) < 5;

                // Add the laer to our list.
                AddEnemyBullet(enemy, healthbullet);
            }

        }

        protected void AddEnemyBullet(Enemy enemy, bool healthbullet)
        {
            Animation bulletAnimation = new Animation();

            var bulletPosition = enemy.Position;
            var playerPosition = _player.Position;

            var direction = playerPosition - enemy.Position;
            direction.Normalize();

            bulletPosition.X += direction.X * enemy.enemyMoveSpeed;

            bulletPosition.X += (enemy.Width / 2f) - 7.5f;
            bulletPosition.Y += (enemy.Height / 2f) - 7.5f;

            // initlize the laser animation
            bulletAnimation.Initialize(healthbullet ? healthbulletTexture : enemybulletTexture,
                bulletPosition,
                15,
                15,
                1,
                30,
                Color.White,
                true, direction.X * enemy.enemyMoveSpeed, direction.Y * enemy.enemyMoveSpeed);

            Bullet bullet = new Bullet();
            bullet.laserMoveSpeed = .1f;

            // init the laser
            bullet.Initialize(bulletAnimation, bulletPosition);
            bullet.Friendly = healthbullet;

            EnemyBullets.Add(bullet);

            /* todo: add code to create a laser. */
            //laserSoundInstance.Play();
        }

        protected void AddBullet(float X, float Y)
        {
            Animation bulletAnimation = new Animation();

            float tX = X;
            float tY = Y;
            if (tX < 0) { tX = tX * -1; }
            if (tY < 0) { tY = tY * -1; }

            var bulletPosition = _player.Position;

            if (tX < tY)
            {
                var ratio = tX / (tX + tY);
                float radius = 25;
                var xplus = Math.Sin(Math.PI * (ratio * 90f) / 180.0) * radius;
                var yplus = Math.Sqrt((radius * radius) - (xplus * xplus));
                bulletPosition.X += (float)xplus * (X < 0 ? -1 : 1);
                bulletPosition.Y += (float)yplus * (Y < 0 ? -1 : 1);
            }
            else
            {
                var ratio = tY / (tX + tY);
                float radius = 25;
                var yplus = Math.Sin(Math.PI * (ratio * 90f) / 180.0) * radius;
                var xplus = Math.Sqrt((radius * radius) - (yplus * yplus));
                bulletPosition.X += (float)xplus * (X < 0 ? -1 : 1);
                bulletPosition.Y += (float)yplus * (Y < 0 ? -1 : 1);
            }

            bulletPosition.X += (_player.Width / 2f) - 7.5f;
            bulletPosition.Y += (_player.Height / 2f) - 7.5f;

            // initlize the laser animation
            bulletAnimation.Initialize(bulletTexture,
                bulletPosition,
                15,
                15,
                1,
                30,
                Color.White,
                true, X, Y);

            Bullet bullet = new Bullet();

            // init the laser
            bullet.Initialize(bulletAnimation, bulletPosition);

            Bullets.Add(bullet);

            /* todo: add code to create a laser. */
            //laserSoundInstance.Play();
        }

        protected void UpdateCollisions(GameTime gameTime)
        {

            // we are going to use the rectangle's built in intersection
            // methods.

            Circle playerRectangle;
            Circle enemyRectangle;
            Circle bulletRectangle;
            Circle ebRectangle;

            // create the rectangle for the player
            playerRectangle = new Circle(
                (int)_player.Position.X,
                (int)_player.Position.Y,
                _player.Width / 2);

            // detect collisions between the player and all enemies.
            for (var i = 0; i < enemies.Count; i++)
            {
                enemyRectangle = new Circle(
                   (int)enemies[i].Position.X,
                   (int)enemies[i].Position.Y,
                   enemies[i].Width / 2);

                // determine if the player and the enemy intersect.
                if (gameTime.TotalGameTime - LastHurt > HurtTime && playerRectangle.Intersects(enemyRectangle))
                {
                    LastHurt = gameTime.TotalGameTime;
                    // deal damge to the player
                    _player.Health -= enemies[i].Damage;

                    // if the player has no health destroy it.
                    if (_player.Health <= 0)
                    {
                        _player.Active = false;
                    }
                }

                for (var l = 0; l < Bullets.Count; l++)
                {
                    // create a rectangle for this laserbeam
                    bulletRectangle = new Circle(
                        (int)Bullets[l].Position.X,
                        (int)Bullets[l].Position.Y,
                        Bullets[l].Width / 2);

                    // test the bounds of the laer and enemy
                    if (bulletRectangle.Intersects(enemyRectangle))
                    {
                        Score++;
                        // kill off the enemy
                        enemies[i].Health -= Bullets[l].Damage;
                        enemies[i].SizeUp(2);

                        // kill off the laserbeam
                        Bullets[l].Active = false;
                    }
                }
            }

            for (var i = 0; i < EnemyBullets.Count; i++)
            {
                ebRectangle = new Circle(
                   (int)EnemyBullets[i].Position.X,
                   (int)EnemyBullets[i].Position.Y,
                   EnemyBullets[i].Width / 2);

                for (var l = 0; l < Bullets.Count; l++)
                {
                    // create a rectangle for this laserbeam
                    bulletRectangle = new Circle(
                        (int)Bullets[l].Position.X,
                        (int)Bullets[l].Position.Y,
                        Bullets[l].Width / 2);

                    // test the bounds of the laer and enemy
                    if (bulletRectangle.Intersects(ebRectangle) && !EnemyBullets[i].Friendly)
                    {
                        Score++;
                        // kill off the enemy
                        EnemyBullets[i].Active = false;

                        // kill off the laserbeam
                        Bullets[l].Active = false;
                    }
                }

                if (gameTime.TotalGameTime - LastBig > BigTime && ebRectangle.Intersects(playerRectangle))
                {
                    LastBig = gameTime.TotalGameTime;
                    if (EnemyBullets[i].Friendly)
                    {
                        _player.SizeUp(-10);
                        if (_player.Width < 25)
                        {
                            _player.Width = 25;
                            _player.Height = 25;
                        }
                    }
                    else
                    {
                        _player.SizeUp(4);
                        if (_player.Width > 400)
                        {
                            _player.Width = 400;
                            _player.Height = 400;
                        }
                    }
                    EnemyBullets[i].Active = false;
                }
            }
        }
    }
}
