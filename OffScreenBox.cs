using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PeggleAI
{
    public class OffScreenBox
    {
        private LevelComponent level;

        // Textures are not necessary for this, but will remain commented in case I need to see the box for debugging 

        /*
        private static Texture2D groundTexture;
        private static Vector2 groundTextureSize;
        private static Vector2 groundTextureOrigin;
        private static Vector2 groundTextureScale;
        */

        private Body groundBody;
        private static Vector2 groundBodySize = new Vector2(11f, 1f);

        // The OffScreenBox is a rectangle placed at the bottom of the camera view.
        // It is used to detect when the ball reaches the bottom of the screen, allowing the player to shoot again
        public OffScreenBox(float x, float y, World world, LevelComponent level) 
        {
            this.level = level;

            Vector2 groundPosition = new Vector2(x, y);

            // Create the ground fixture
            groundBody = world.CreateBody(groundPosition, 0, BodyType.Static);
            var gfixture = groundBody.CreateRectangle(groundBodySize.X, groundBodySize.Y, 1f, Vector2.Zero);

            groundBody.OnCollision += OnCollision;
        }

        public bool OnCollision(Fixture sender, Fixture other, Contact contact)
        {
            // Once a ball collides with this box, tell the level that the turn is over
            level.ballShot = false;

            return true;
        }

        /*
        public static void loadContent(Texture2D groundTexture)
        {
            OffScreenBox.groundTexture = groundTexture;
            OffScreenBox.groundTextureSize = new Vector2(groundTexture.Width, groundTexture.Height);
            OffScreenBox.groundTextureOrigin = groundTextureSize / 2f;
            OffScreenBox.groundTextureScale = new Vector2(
                groundBodySize.X / groundTextureSize.X,
                groundBodySize.Y / groundTextureSize.Y
            );
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                groundTexture, 
                LevelComponent.convertVec(groundBody.Position), 
                null, 
                Color.White, 
                groundBody.Rotation,
                LevelComponent.convertVec(groundTextureOrigin),
                LevelComponent.convertVec(groundTextureScale), 
                SpriteEffects.FlipVertically, 
                0f
             );
        }
        */
    }
}