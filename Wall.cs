using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PeggleAI
{
    // Simple walls on the edges of the play area
    public class Wall
    {
        // Textures are not necessary for this, but will remain commented in case I need to see the box for debugging 

        /*
        private static Texture2D wallTexture;
        private static Vector2 wallTextureSize;
        private static Vector2 wallTextureOrigin;
        private static Vector2 wallTextureScale;
        */

        private Body wallBody;
        private static Vector2 wallBodySize = new Vector2(0.25f, 10f);

        public Wall(float x, float y, World world)
        {
            Vector2 wallPosition = new Vector2(x, y);

            // Create the wall fixture
            wallBody = world.CreateBody(wallPosition, 0, BodyType.Static);
            var wfixture = wallBody.CreateRectangle(wallBodySize.X, wallBodySize.Y, 1f, Vector2.Zero);
        }

        /*
        public static void loadContent(Texture2D wallTexture)
        {
            Wall.wallTexture = wallTexture;
            Wall.wallTextureSize = new Vector2(wallTexture.Width, wallTexture.Height);
            Wall.wallTextureOrigin = wallTextureSize / 2f;
            Wall.wallTextureScale = new Vector2(
                wallBodySize.X / wallTextureSize.X,
                wallBodySize.Y / wallTextureSize.Y
            );
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                wallTexture, 
                LevelComponent.convertVec(wallBody.Position), 
                null, 
                Color.White, 
                wallBody.Rotation,
                LevelComponent.convertVec(wallTextureOrigin),
                LevelComponent.convertVec(wallTextureScale), 
                SpriteEffects.FlipVertically, 
                0f
             );
        }
        */
    }
}