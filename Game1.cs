using System.Diagnostics;
using System.Security.AccessControl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PeggleAI
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        private BasicEffect spriteBatchEffect;

        private Texture2D background;

        // AI stuffs
        private const int POPULATION_SIZE = 10;
        private LevelComponent[] levels;
        private PeggleAlgorithm peggleAI;


        // Camera
		private Vector3 cameraPosition = new Vector3(0, 0, 0);
		float cameraViewWidth = 12.5f;
		private Vector2 cameraView;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            peggleAI = new PeggleAlgorithm(this, POPULATION_SIZE);

            levels = new LevelComponent[POPULATION_SIZE];
            for (int i=0; i<POPULATION_SIZE; i++)
            {
                levels[i] = new LevelComponent();
            }

            // Get the width and height of the screen
			var vp = GraphicsDevice.Viewport;
			cameraView = new Vector2(cameraViewWidth, cameraViewWidth / vp.AspectRatio);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // BasicEffect is used with the camera
			spriteBatchEffect = new BasicEffect(GraphicsDevice);
			spriteBatchEffect.TextureEnabled = true;

            // Load sprites
            background = Content.Load<Texture2D>("Level1");
			Texture2D ballTexture = Content.Load<Texture2D>("CircleSprite");
			Texture2D arrowTexture = Content.Load<Texture2D>("Arrow");
			Texture2D bluePegTexture = Content.Load<Texture2D>("BluePeg");
			Texture2D orangePegTexture = Content.Load<Texture2D>("OrangePeg");

			// Call loadContent to give each game object the textures they need
			Peg.loadContent(bluePegTexture, orangePegTexture);
			Ball.loadContent(ballTexture);
			BallShooter.loadContent(arrowTexture);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            foreach (LevelComponent level in levels)
            {
                // Just as a note, each level is still tracking input, 
                // so although the algorithm will manually set angles, pressing buttons will still mess with it.
                level.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Update camera View and Projection
			var vp = GraphicsDevice.Viewport;
			spriteBatchEffect.View = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);
			spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0f, -1f);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullClockwise, spriteBatchEffect);

            Vector2 bgPos = new Vector2(background.Width/2f, background.Height/2f);
            spriteBatch.Draw(
				background,
				Vector2.Zero,
				null,
				Color.White,
				0f,
				bgPos,
				0.0156f,
				SpriteEffects.FlipVertically,
				0f
			);

            foreach (LevelComponent level in levels)
            {
                level.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}