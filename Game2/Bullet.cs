using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace Game2
{
    public class Bullet
    {

        // animation the represents the laser animation.
        public Animation BulletAnimation;

        // the speed the laser traves
        public float laserMoveSpeed = .5f;

        // postion of the laser
        public Vector2 Position;

        // The damage the laser deals.
        public int Damage = 10;

        // set the laser to active
        public bool Active;

        // Range of the laser.
        int Range;

        public bool Friendly;

        // the width of the player image.
        public int Width
        {
            get { return BulletAnimation.FrameWidth; }
        }

        // the height of the player image.
        public int Height
        {
            get { return BulletAnimation.FrameHeight; }

        }

        public void Initialize(Animation animation, Vector2 position)
        {
            BulletAnimation = animation;
            Position = position;
            Active = true;
            Friendly = false;
        }

        public void Update(GameTime gameTime)
        {
            float X = BulletAnimation.X;
            float Y = BulletAnimation.Y;
            if (X < 0) { X = X * -1; }
            if (Y < 0) { Y = Y * -1; }

            if (X < Y)
            {
                var ratio = X / (X + Y);
                float radius = 25;
                var xplus = Math.Sin(Math.PI * (ratio * 90f) / 180.0) * radius;
                var yplus = Math.Sqrt((radius * radius) - (xplus * xplus));
                Position.X += (float)xplus * laserMoveSpeed * (BulletAnimation.X < 0 ? -1 : 1);
                Position.Y += laserMoveSpeed * (float)yplus * (BulletAnimation.Y < 0 ? -1 : 1);
            }
            else
            {
                var ratio = Y / (X + Y);
                float radius = 25;
                var yplus = Math.Sin(Math.PI * (ratio * 90f) / 180.0) * radius;
                var xplus = Math.Sqrt((radius * radius) - (yplus * yplus));
                Position.X += (float)xplus * laserMoveSpeed * (BulletAnimation.X < 0 ? -1 : 1);
                Position.Y += laserMoveSpeed * (float)yplus * (BulletAnimation.Y < 0 ? -1 : 1);
            }
            

            BulletAnimation.Position = Position;
            BulletAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            BulletAnimation.Draw(spriteBatch);
        }
    }
}
