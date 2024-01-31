using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using nkast.Aether.Physics2D.Dynamics;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PeggleAI
{

    public class Peg
    {

        // Sprites
        private static Texture2D pegTexture;
        private static Vector2 pegTextureSize;
        private static Vector2 pegTextureOrigin;
        private static Vector2 pegTextureScale;

        // Physics
        private static World world;
        private static float pegRadius = 0.15f;
        private Body pegBody;
        private const float PEG_BOUNCINESS = 0.6f;
        private const float PEG_FRICTION = 0.1f;

        public Peg(float x, float y)
        {
            Vector2 pegPosition = new Vector2(x, y);

            pegBody = world.CreateBody(pegPosition, 0, BodyType.Static);

            // Fixtures are what binds a shape to a body for collision.
            var p_fixture = pegBody.CreateCircle(pegRadius, 1f);
            // Fixtures hold data for bounciness and friction as well
            p_fixture.Restitution = PEG_BOUNCINESS;
            p_fixture.Friction = PEG_FRICTION;
        }

        public static void loadContent(Texture2D pegTexture, World world)
        {
            Peg.pegTexture = pegTexture;
            Peg.world = world;

            // Scale the texture to the collision body dimensions
            Peg.pegTextureSize = new Vector2(pegTexture.Width, pegTexture.Height);
            Peg.pegTextureScale = new Vector2(
                (pegRadius * 2f) / pegTextureSize.X, 
                (pegRadius * 2f) / pegTextureSize.Y
            );

            // Draw the texture at the center of the shapes
            Peg.pegTextureOrigin = pegTextureSize / 2f;

        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                   pegTexture,
                   LevelComponent.convertVec(pegBody.Position),
                   null,
                   Color.Blue,
                   pegBody.Rotation,
                   LevelComponent.convertVec(pegTextureOrigin),
                   LevelComponent.convertVec(pegTextureScale),
                   SpriteEffects.FlipVertically,
                   0f
            );
        }


    }
}