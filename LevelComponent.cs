using System;

using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using nkast.Aether.Physics2D.Dynamics;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PeggleAI
{ 
	public class LevelComponent : DrawableGameComponent
	{
		// Sprites
		private SpriteBatch spriteBatch;
		private BasicEffect spriteBatchEffect;

		// Input
		private KeyboardState oldKbState;
		private MouseState oldMouseState;

		// Camera
		private Vector3 cameraPosition = new Vector3(0, 0, 0);
		float cameraViewWidth = 12.5f;
		private Vector2 cameraView;

		// Physics
		private World world;

		// Level Objects
		private Texture2D background;
		private HashSet<Peg> pegs;
		private BallShooter shooter;
		private OffScreenBox offScreenBox;

		// This variable tracks if the player has shot the ball.
		// If so, the player should not be able to shoot until the turn is finished
		public bool ballShot { get; set; } 
		public Queue<Peg> pegsHit = new Queue<Peg>();

		public LevelComponent(Game game) : base(game)
		{

		}

		public override void Initialize()
		{
			// Create a new world that holds all physics information
			world = new World();
			ballShot = false;

			// Get the width and height of the screen
			var vp = GraphicsDevice.Viewport;
			cameraView = new Vector2(cameraViewWidth, cameraViewWidth / vp.AspectRatio);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(Game.GraphicsDevice);
			// BasicEffect is used with the camera
			spriteBatchEffect = new BasicEffect(Game.GraphicsDevice);
			spriteBatchEffect.TextureEnabled = true;

			// Load sprites
			Texture2D ballTexture = Game.Content.Load<Texture2D>("CircleSprite");
            background = Game.Content.Load<Texture2D>("Level1");
			Texture2D arrowTexture = Game.Content.Load<Texture2D>("Arrow");
			Texture2D groundTexture = Game.Content.Load<Texture2D>("GroundSprite");

			// Call loadContent to give each game object the textures they need
			Peg.loadContent(ballTexture, world);
			Ball.loadContent(ballTexture, world);
			OffScreenBox.loadContent(groundTexture);

			// Create all of the level objects
			loadLevel();
			shooter = new BallShooter(arrowTexture, this);
			offScreenBox = new OffScreenBox(world, this);

		}

		private void loadLevel()
		{
			// Load data from level1 file
			string[] pegPositions = File.ReadAllLines("../../../Level1.txt");

			// Create all of the pegs in the level
			pegs = new HashSet<Peg>();
			foreach (string position in pegPositions)
			{
				string[] pos = position.Split(' ');
				pegs.Add( new Peg(float.Parse(pos[0]), float.Parse(pos[1]), this) );
			}
			
		}

		public override void Update(GameTime gameTime)
		{
			// Input
			HandleKeyboard(gameTime);
			//HandleMouse();

			// If the turn has ended, remove the ball and all the pegs that have been hit
			if(ballShot == false)
			{
				shooter.removeBall(world);

				Peg peg;
				while (pegsHit.Count > 0)
				{
					peg = pegsHit.Dequeue();

					// This is necessary to avoid attepting to remove the same peg from the world twice
					if (pegs.Contains(peg))
					{
						world.Remove(peg.pegBody);
						pegs.Remove(peg);
					}
					
				}
			}
			
			// Otherwise, update physics world
			world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		private void HandleMouse()
		{
			MouseState state = Mouse.GetState();

			oldMouseState = state;
		}

		private void HandleKeyboard(GameTime gameTime)
		{
			KeyboardState state = Keyboard.GetState();
			float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			
			// Aim the arrow
			if (state.IsKeyDown(Keys.Left))
				shooter.moveLeft(totalSeconds);

			if (state.IsKeyDown(Keys.Right))
				shooter.moveRight(totalSeconds);

			// Shoot ball
			if (!ballShot && state.IsKeyDown(Keys.Space) && oldKbState.IsKeyUp(Keys.Space)) 
			{
				shooter.shoot();
				ballShot = true;
			}

			oldKbState = state;
		}

		public override void Draw(GameTime gameTime)
		{
			// Update camera View and Projection
			var vp = GraphicsDevice.Viewport;
			spriteBatchEffect.View = Matrix.CreateLookAt(cameraPosition, cameraPosition + Vector3.Forward, Vector3.Up);
			spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0f, -1f);

			// Draw player and ground. 
			// Our View/Projection requires RasterizerState.CullClockwise and SpriteEffects.FlipVertically.
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullClockwise, spriteBatchEffect);

			// Draw the background
			Microsoft.Xna.Framework.Vector2 bgPos = new Microsoft.Xna.Framework.Vector2(background.Width/2f, background.Height/2f);
			spriteBatch.Draw(
				background,
				Microsoft.Xna.Framework.Vector2.Zero,
				null,
				Color.White,
				0f,
				bgPos,
				0.0125f,
				SpriteEffects.FlipVertically,
				0f
			);

			// Draw each peg
			foreach(Peg peg in pegs)
				peg.draw(spriteBatch);

			// Draw the ball shooter
			shooter.draw(spriteBatch);

			offScreenBox.draw(spriteBatch);

			spriteBatch.End();
		}

		// For some reason I need to convert between Aether2D vectors to Microsoft Vectors manually
		// This method is used by game objects such as pegs and balls when drawing
		public static Microsoft.Xna.Framework.Vector2 convertVec(Vector2 aetherVec)
		{
			return new Microsoft.Xna.Framework.Vector2(aetherVec.X, aetherVec.Y);
		}

	}	
}
