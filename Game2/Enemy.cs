using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Game2
{
    public class Enemy
    {
        // animation represneting the enemy.
        public Animation EnemyAnimation;

        // The postion of the enemy ship relative to the 
        // top of left corner of the screen
        public Vector2 Position;

        // state of the enemy ship
        public bool Active;

        // Hit points of the enemy, if this goes
        // to zero the enemy dies.      
        public int Health;

        // the amount of damage that the enemy
        // ship inflicts on the player.
        public int Damage;

        // the amount of the score enemy is worth.
        public int Value;

        public float DirectionX;

        public float DirectionY;

        public TimeSpan LastFired;
        public TimeSpan TimeToFire;

        // Get the width of the enemy ship
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        // Get the height of the enemy ship.
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        public void SizeUp(int amount)
        {
            this.EnemyAnimation.FrameHeight += amount;
            this.EnemyAnimation.FrameWidth += amount;
        }

        public float enemyMoveSpeed;

        public void Initialize(Animation animation,
            Vector2 position, float directionX, float directionY)
        {
            // load the enemy ship texture;
            EnemyAnimation = animation;

            // set the postion of th enemy ship
            Position = position;

            // set the enemy to be active
            Active = true;

            // set the health of the enemy
            Health = 250;

            // Set the amount of damage the enemy does
            Damage = 10;

            // Set how fast the enemy moves.
            enemyMoveSpeed = 5;

            // set the value of the enemy
            Value = 100;

            LastFired = TimeSpan.Zero;
            TimeToFire = TimeSpan.FromSeconds(.2f);

            DirectionX = directionX;
            DirectionY = directionY;
        }

        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            // the enemy always moves to the left.
            //Position.X -= enemyMoveSpeed

            float X = DirectionX;
            float Y = DirectionY;
            if (X < 0) { X = X * -1; }
            if (Y < 0) { Y = Y * -1; }

            var ratio = X / (X + Y);
            Position.X += ratio * enemyMoveSpeed * (DirectionX < 0 ? -1 : 1);
            Position.Y += enemyMoveSpeed * (1f - ratio) * (DirectionY < 0 ? -1 : 1);

            if ((DirectionX < 0 && Position.X < 0) || (DirectionX > 0 && Position.X > graphicsDevice.Viewport.TitleSafeArea.Width - this.Width))
            {
                this.DirectionX = this.DirectionX * -1;
                Position.X += ratio * enemyMoveSpeed * 2;
            }
            else if ((DirectionY < 0 && Position.Y < 0) || (DirectionY > 0 && Position.Y > graphicsDevice.Viewport.TitleSafeArea.Height - this.Height))
            {
                this.DirectionY = this.DirectionY * -1;
                Position.Y += enemyMoveSpeed * (1f - ratio) * 2;
            }

            // Update the postion of the anaimation
            EnemyAnimation.Position = Position;

            // Update the animation;
            EnemyAnimation.Update(gameTime);

            /* If the enenmy is past the screen or its
             * health reaches 0 then deactivate it. */
            if (Position.X < -Width || Health <= 0)
            {
                //deactivate the enemy
                Active = false;

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }
    }
}
