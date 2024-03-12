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
        // Can aim between 90 and 270 degrees. Must be converted to radians.
        private static int MAX_LEFT_DEG = 90;
        private static int MAX_RIGHT_DEG = 270;
        private static float MAX_LEFT_RAD = (float)(Math.PI * MAX_LEFT_DEG / 180.0);
        private static float MAX_RIGHT_RAD = (float)(Math.PI * MAX_RIGHT_DEG / 180.0);

        public BallShooter(Texture2D texture, LevelComponent level)
        {
            this.level = level;
            this.arrowTexture = texture;
            this.arrowOrigin = new Vector2(arrowTexture.Width / 2f, 0);
            this.rotation = (float)Math.PI;
        }

        public Ball getBall() { return this.ball; }

        public static int getMaxLeft() { return MAX_LEFT_DEG; }

        public static int getMaxRight() { return MAX_RIGHT_DEG; }

        public void moveLeft(float totalSeconds)
        {
            float newAngle = rotation - ROTATION_SPEED * totalSeconds;
            rotation = newAngle < MAX_LEFT_RAD ? MAX_LEFT_RAD : newAngle;
        }

        public void moveRight(float totalSeconds)
        {
            float newAngle = rotation + ROTATION_SPEED * totalSeconds;
            rotation = newAngle > MAX_RIGHT_RAD ? MAX_RIGHT_RAD : newAngle;
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