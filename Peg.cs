using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using nkast.Aether.Physics2D.Dynamics;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PeggleAI
{

    public class Peg
    {

        // Sprites
        private static Texture2D _pegTexture;
        private Vector2 _pegTextureSize;
        private Vector2 _pegTextureOrigin;
        Vector2 _pegTextureScale;

        // Physics
        private World _world;
        private float _pegRadius = 0.15f;
        public Body _pegBody { get; }

        public Peg(World world, float x, float y)
        {
            this._world = world;

            Vector2 pegPosition = new Vector2(x, y);

            _pegBody = _world.CreateBody(pegPosition, 0, BodyType.Static);
            var p_fixture = _pegBody.CreateCircle(_pegRadius, 1f);
            p_fixture.Restitution = 0.6f;
            p_fixture.Friction = 0.1f;
        }

        public void loadContent(Texture2D pegTexture)
        {
            _pegTexture = pegTexture;
            _pegTextureSize = new Vector2(_pegTexture.Width, _pegTexture.Height);
            _pegTextureOrigin = _pegTextureSize / 2f;
            _pegTextureScale = new Vector2(
                (_pegRadius * 2f) / _pegTextureSize.X, 
                (_pegRadius * 2f) / _pegTextureSize.Y
            );
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                   _pegTexture,
                   LevelComponent.convertVec(_pegBody.Position),
                   null,
                   Color.Blue,
                   _pegBody.Rotation,
                   LevelComponent.convertVec(_pegTextureOrigin),
                   LevelComponent.convertVec(_pegTextureScale),
                   SpriteEffects.FlipVertically,
                   0f
            );
        }


    }
}