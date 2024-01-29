using System;

using System.Collections;
using System.Collections.Generic;
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
		private SpriteBatch _spriteBatch;
		private BasicEffect _spriteBatchEffect;

		private Texture2D _playerTexture;
		private Texture2D _groundTexture;
		private Vector2 _playerTextureSize;
		private Vector2 _groundTextureSize;
		private Vector2 _playerTextureOrigin;
		private Vector2 _groundTextureOrigin;

		// Input
		private KeyboardState _oldKbState;
		private MouseState _oldMouseState;

		// Camera
		private Vector3 _cameraPosition = new Vector3(0, 0, 0); // Camera is 1.7 meters above the ground
		float cameraViewWidth = 12.5f; // Camera is 12.5 meters wide

		// Physics
		private World _world;
		private Body _playerBody;
		private Body _groundBody;
		private float _playerBodyRadius = 1.5f / 2f; // Diameter of the player is 1.5 meters
		private Vector2 _groundBodySize = new Vector2(8f, 1f); // Ground is 8x1 meters

		// Level Objects
		private Texture2D _bg;
		List<Peg> pegs;

		public LevelComponent(Game game) : base(game)
		{

		}

		public override void Initialize()
		{
			// Create a new world
			_world = new World();

			/* Circle */
			Vector2 playerPosition = new Vector2(0, _playerBodyRadius);

			// Create the player fixture
			// Fixtures are what binds a shape to a body for collision.
			_playerBody = _world.CreateBody(playerPosition, 0, BodyType.Dynamic);
			var p_fixture = _playerBody.CreateCircle(_playerBodyRadius, 1f);

			// Fixtures hold data for bounciness and friction as well
			p_fixture.Restitution = 0.6f;
			p_fixture.Friction = 0.5f;


			/* Ground */
			Vector2 groundPosition = new Vector2(0, -(_groundBodySize.Y / 2f));

			// Create ground fixture
			_groundBody = _world.CreateBody(groundPosition, 0, BodyType.Static);
			var g_fixture = _groundBody.CreateRectangle(_groundBodySize.X, _groundBodySize.Y, 1f, Vector2.Zero);

			g_fixture.Restitution = 0.3f;
			g_fixture.Friction = 0.5f;

			// Create all of the pegs in the level
			pegs = new List<Peg>();
			pegs.Add( new Peg(_world, -5f, 1.5f) );

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
			// Use a BasicEffect for texturing, never done this before
			_spriteBatchEffect = new BasicEffect(Game.GraphicsDevice);
			_spriteBatchEffect.TextureEnabled = true;

			// Load sprites
			_playerTexture = Game.Content.Load<Texture2D>("CircleSprite");
			_groundTexture = Game.Content.Load<Texture2D>("GroundSprite");
            _bg = Game.Content.Load<Texture2D>("Level1");

			// Scale the texture to the collision body dimensions
			_playerTextureSize = new Vector2(_playerTexture.Width, _playerTexture.Height);
			_groundTextureSize = new Vector2(_groundTexture.Width, _groundTexture.Height);

			// Draw the texture at the center of the shapes
			_playerTextureOrigin = _playerTextureSize / 2f;
			_groundTextureOrigin = _groundTextureSize / 2f;

			foreach(Peg peg in pegs)
				peg.loadContent(_playerTexture);
		}

		public override void Update(GameTime gameTime)
		{
			// Input
			HandleKeyboard(gameTime);
			HandleMouse();

			// Update world 
			_world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		private void HandleMouse()
		{
			MouseState state = Mouse.GetState();

			if( state.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
				System.Diagnostics.Debug.WriteLine( pegs[0]._pegBody.Position.X + " " + pegs[0]._pegBody.Position.Y );

			_oldMouseState = state;
		}

		private void HandleKeyboard(GameTime gameTime)
		{
			KeyboardState state = Keyboard.GetState();
			float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

			/*
			// Move Camera
			if (state.IsKeyDown(Keys.Left))
				_cameraPosition.X -= totalSeconds * cameraViewWidth;

			if (state.IsKeyDown(Keys.Right))
				_cameraPosition.X += totalSeconds * cameraViewWidth;

			if (state.IsKeyDown(Keys.Up))
				_cameraPosition.Y += totalSeconds * cameraViewWidth;

			if (state.IsKeyDown(Keys.Down))
				_cameraPosition.Y -= totalSeconds * cameraViewWidth;
			*/

			// Move the peg for finding peg locations manually
			if (state.IsKeyDown(Keys.Left))
				pegs[0].moveHorizontal(-0.5f * totalSeconds);

			if (state.IsKeyDown(Keys.Right))
				pegs[0].moveHorizontal(0.5f * totalSeconds);

			if (state.IsKeyDown(Keys.Up))
				pegs[0].moveVertical(0.5f * totalSeconds);

			if (state.IsKeyDown(Keys.Down))
				pegs[0].moveVertical(-0.5f * totalSeconds);
			

			// Rotate player
			if (state.IsKeyDown(Keys.A))
				_playerBody.ApplyTorque(10);

			if (state.IsKeyDown(Keys.D))
				_playerBody.ApplyTorque(-10);

			if (state.IsKeyDown(Keys.Space) && _oldKbState.IsKeyUp(Keys.Space))
				_playerBody.ApplyLinearImpulse(new Vector2(0, 10));

			_oldKbState = state;
		}

		public override void Draw(GameTime gameTime)
		{
			// Update camera View and Projection
			var vp = GraphicsDevice.Viewport;
			_spriteBatchEffect.View = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + Vector3.Forward, Vector3.Up);
			_spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0f, -1f);

			// Draw player and ground. 
			// Our View/Projection requires RasterizerState.CullClockwise and SpriteEffects.FlipVertically.
			_spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullClockwise, _spriteBatchEffect);

			Microsoft.Xna.Framework.Vector2 bgPos = new Microsoft.Xna.Framework.Vector2(_bg.Width/2f, _bg.Height/2f);
			_spriteBatch.Draw(
				_bg,
				Microsoft.Xna.Framework.Vector2.Zero,
				null,
				Color.White,
				0f,
				bgPos,
				0.0125f,
				SpriteEffects.FlipVertically,
				0f
			);

			Vector2 playerTextureScale = new Vector2( (_playerBodyRadius * 2f) / _playerTextureSize.X, (_playerBodyRadius * 2f) / _playerTextureSize.Y);
			Vector2 groundTextureScale = new Vector2(_groundBodySize.X / _groundTextureSize.X, _groundBodySize.Y / _groundTextureSize.Y);
			/*
			_spriteBatch.Draw(
				_playerTexture, 
				convertVec(_playerBody.Position), 
				null, 
				Color.White, 
				_playerBody.Rotation, 
				convertVec(_playerTextureOrigin), 
				convertVec(playerTextureScale), 
				SpriteEffects.FlipVertically, 
				0f
			);

			_spriteBatch.Draw(
				_groundTexture, 
				convertVec(_groundBody.Position), 
				null, 
				Color.White, 
				_groundBody.Rotation, 
				convertVec(_groundTextureOrigin), 
				convertVec(groundTextureScale), 
				SpriteEffects.FlipVertically, 
				0f
			);
			*/

			foreach(Peg peg in pegs)
				peg.draw(_spriteBatch);

			_spriteBatch.End();
		}

		// For some reason I need to convert between Aether2D vectors to Microsoft Vectors manually
		public static Microsoft.Xna.Framework.Vector2 convertVec(Vector2 aetherVec)
		{
			return new Microsoft.Xna.Framework.Vector2(aetherVec.X, aetherVec.Y);
		}

	}	
}
