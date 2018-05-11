using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2
{
    class Player
    {
        public Animation PlayerAnimation;

        public Vector2 Position;

        // State of the player
        public bool Active;

        // Amount of hit points the player has
        public int Health;

        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
            set { PlayerAnimation.FrameWidth = value; }
        }
        
        public int Height
        { get { return PlayerAnimation.FrameHeight; }
            set { PlayerAnimation.FrameHeight = value; }
        }

        public void SizeUp(int amount)
        {
            this.PlayerAnimation.FrameHeight += amount;
            this.PlayerAnimation.FrameWidth += amount;
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;

            Position = position;
            Active = true;
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }
    }
}
