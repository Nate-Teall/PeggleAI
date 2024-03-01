using System;

using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using nkast.Aether.Physics2D.Dynamics;

namespace PeggleAI
{
    public class BallShooter
    {
        // Keep a reference to the level that this shooter belongs to.
        // This will be helpful when we have multiple instances of the game running at once
        // during the algorithm
        private LevelComponent level;

        private Texture2D arrowTexture;
        private Vector2 arrowOrigin;

        private float rotation;

        private Ball ball;

        private const float ROTATION_SPEED = 1f;
        // Max angle the arrow can point left/right
        private const float MAX_LEFT = (float)(Math.PI / 2);
        private const float MAX_RIGHT = (float)(3 * Math.PI / 2);

        public BallShooter(Texture2D texture, LevelComponent level)
        {   
            this.level = level;
            this.arrowTexture = texture;
            this.arrowOrigin = new Vector2( arrowTexture.Width/2f, 0 );
            this.rotation = (float)Math.PI;
        }

        public Ball getBall() { return this.ball; }

        public void moveLeft(float totalSeconds)
        {
            float newAngle = rotation - ROTATION_SPEED * totalSeconds;
            rotation = newAngle < MAX_LEFT ? MAX_LEFT : newAngle;
        }

        public void moveRight(float totalSeconds)
        {
            float newAngle = rotation + ROTATION_SPEED * totalSeconds;
            rotation = newAngle > MAX_RIGHT ? MAX_RIGHT : newAngle;
        }

        public void shoot()
        {
            this.ball = new Ball(0, 3.6f, rotation);
        }

        public void removeBall(World world)
        {
            if (ball != null)
            {
                world.Remove(ball.ballBody);
                ball = null;
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            // Only draw the arrow while the player is aiming
            if (!level.ballShot)
            {
                spriteBatch.Draw(
                       arrowTexture,
                       new Vector2(0, 3.6f),
                       null,
                       Color.White,
                       rotation,
                       arrowOrigin,
                       0.0125f,
                       SpriteEffects.None,
                       0f
                );
            }

            if (ball is not null)
                ball.draw(spriteBatch);
        }

    }
}