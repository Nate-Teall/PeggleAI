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
        // Keep a reference to the level that this peg belongs to.
        // This will be helpful when we have multiple instances of the game running at once
        // during the algorithm
        private LevelComponent level;

        // Sprites
        private static Texture2D bluePegTexture;
        private static Texture2D orangePegTexture;
        private static Vector2 pegTextureSize;
        private static Vector2 pegTextureOrigin;
        private static Vector2 pegTextureScale;
        private bool isOrange;

        // Physics
        private World world;
        private static float pegRadius = 0.15f;
        public Body pegBody { get; private set; }
        private Fixture pegFixture;
        private const float PEG_BOUNCINESS = 0.6f;
        private const float PEG_FRICTION = 0.1f;

        public Peg(float x, float y, LevelComponent level, bool isOrange)
        {
            this.level = level;
            this.isOrange = isOrange;
            
            Vector2 pegPosition = new Vector2(x, y);

            pegBody = level.world.CreateBody(pegPosition, 0, BodyType.Static);

            // Fixtures are what binds a shape to a body for collision.
            pegFixture = pegBody.CreateCircle(pegRadius, 1f);
            // Fixtures hold data for bounciness and friction as well
            pegFixture.Restitution = PEG_BOUNCINESS;
            pegFixture.Friction = PEG_FRICTION;

            pegBody.OnCollision += OnCollision;
            pegBody.OnSeparation += OnSeparation;
        }

        public static void loadContent(Texture2D bluePegTexture, Texture2D orangePegTexture)
        {
            Peg.bluePegTexture = bluePegTexture;
            Peg.orangePegTexture = orangePegTexture;

            // Scale the texture to the collision body dimensions
            Peg.pegTextureSize = new Vector2(bluePegTexture.Width, bluePegTexture.Height);
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
            level.pegsHit.Enqueue(this);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            Texture2D currentTexture;
            if (isOrange)
            {
                currentTexture = orangePegTexture;
            } 
            else
            {
                currentTexture = bluePegTexture;
            }

            spriteBatch.Draw(
                currentTexture,
                LevelComponent.convertVec(pegBody.Position),
                null,
                Color.White,
                pegBody.Rotation,
                LevelComponent.convertVec(pegTextureOrigin),
                LevelComponent.convertVec(pegTextureScale),
                SpriteEffects.FlipVertically,
                0f
            );
        }

    }
}