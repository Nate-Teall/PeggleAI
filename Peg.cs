using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;

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
        public Body pegBody { get; private set; }
        private Fixture pegFixture;
        private const float PEG_BOUNCINESS = 0.6f;
        private const float PEG_FRICTION = 0.1f;

        public Peg(float x, float y)
        {
            Vector2 pegPosition = new Vector2(x, y);

            pegBody = world.CreateBody(pegPosition, 0, BodyType.Static);

            // Fixtures are what binds a shape to a body for collision.
            pegFixture = pegBody.CreateCircle(pegRadius, 1f);
            // Fixtures hold data for bounciness and friction as well
            pegFixture.Restitution = PEG_BOUNCINESS;
            pegFixture.Friction = PEG_FRICTION;

            pegBody.OnCollision += OnCollision;
            pegBody.OnSeparation += OnSeparation;
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

        public bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            return true;
        }

        public void OnSeparation(Fixture sender, Fixture other, Contact contact)
        {
            LevelComponent.pegsHit.Enqueue(this);
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