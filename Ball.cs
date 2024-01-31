using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using nkast.Aether.Physics2D.Dynamics;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PeggleAI
{
    public class Ball
    {
        // Sprites
        private static Texture2D ballTexture;
        private static Vector2 ballTextureSize;
        private static Vector2 ballTextureOrigin;
        private static Vector2 ballTextureScale;

        // Physics
        private static World world;
        private static float ballRadius = 0.125f;
        private Body ballBody;
        private const float BALL_SPEED = 0.5f;
        private const float BALL_BOUNCINESS = 0.6f;
        private const float BALL_FRICTION = 0.1f;

        
        public Ball(float x, float y, float rotation)
        {

            Vector2 ballPosition = new Vector2(x, y);

            ballBody = world.CreateBody(ballPosition, 0, BodyType.Dynamic);
            var p_fixture = ballBody.CreateCircle(ballRadius, 1f);
            p_fixture.Restitution = BALL_BOUNCINESS;
            p_fixture.Friction = BALL_FRICTION;

            // When the ball is spawned, give it some velocity in the direction the player is aiming
            Vector2 ballVelocity = new Vector2((float)Math.Sin(rotation) * -1, (float)Math.Cos(rotation));
            ballBody.ApplyLinearImpulse( ballVelocity * BALL_SPEED);
        }

        public static void loadContent(Texture2D ballTexture, World world)
        {
            Ball.ballTexture = ballTexture;
            Ball.world = world;

            Ball.ballTextureSize = new Vector2(ballTexture.Width, ballTexture.Height);
            Ball.ballTextureScale = new Vector2(
                (ballRadius * 2f) / ballTextureSize.X,
                (ballRadius * 2f) / ballTextureSize.Y
            );

            Ball.ballTextureOrigin = ballTextureSize / 2f;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                ballTexture,
                LevelComponent.convertVec(ballBody.Position),
                null,
                Color.White,
                ballBody.Rotation,
                LevelComponent.convertVec(ballTextureOrigin),
                LevelComponent.convertVec(ballTextureScale),
                SpriteEffects.FlipVertically,
                0f
            );
        }
    }
}