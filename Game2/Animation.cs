using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game2
{
    public class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;
        
        // The time since we last updated the frame
        int elapsedTime;

        // The time we display a frame until the next one
        int frameTime;

        // The number of frames that the animation contains
        int frameCount;

        // The index of the current frame we are displaying
        int currentFrame;

        // The color of the frame we will be displaying
        Color color;

        // The area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();

        // The area where we want to display the image strip in the game
        Rectangle destinationRect = new Rectangle();

        // Width of a given frame
        public int FrameWidth;

        // Height of a given frame
        public int FrameHeight;

        public int OriginalWidth;

        public int OriginalHeight;

        // The state of the Animation
        public bool Active;

        // Determines if the animation will keep playing or deactivate after one run
        public bool Looping;

        // Width of a given frame
        public Vector2 Position;

        public float X;

        public float Y;

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frametime, Color color, bool looping, float X, float Y)
        {
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.OriginalHeight = frameHeight;
            this.OriginalWidth = frameWidth;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.X = X;
            this.Y = Y;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            elapsedTime = 0;
            currentFrame = 0;

            // Set the Animation to active by default.
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if(Active == false) return;

            elapsedTime += (int) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                currentFrame++;

                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    if (Looping == false)
                        Active = false;
                }

                elapsedTime = 0;
            }

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, OriginalWidth, OriginalHeight);

            // Grab the frame in the image strip by multiplying the currentFrame index by the frame width.
            destinationRect = new Rectangle((int) Position.X,
                                            (int) Position.Y,
                                            (int) (FrameWidth),
                                            (int) (FrameHeight));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}
