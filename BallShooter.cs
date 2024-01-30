using System;

using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PeggleAI
{
    public class BallShooter
    {
        private Texture2D arrowTexture;
        private Vector2 arrowOrigin;

        private float rotation;

        public BallShooter(Texture2D texture)
        {   
            this.arrowTexture = texture;
            this.arrowOrigin = new Vector2( arrowTexture.Width/2f, 0 );
            this.rotation = (float)Math.PI;
        }

        public void draw(SpriteBatch spriteBatch)
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

    }
}