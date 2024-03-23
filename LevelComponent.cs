using System;

using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using nkast.Aether.Physics2D.Dynamics;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PeggleAI
{ 
	public class LevelComponent
	{
		// Input
		private KeyboardState oldKbState;
		private MouseState oldMouseState;

		// Physics
		public World world { get; private set; }

		// Level Objects
		//private Texture2D background;
		private HashSet<Peg> pegs;
		private BallShooter shooter;
		private OffScreenBox offScreenBox;
		private Wall lWall;
		private Wall rWall;

		// This variable tracks if the player has shot the ball.
		// If so, the player should not be able to shoot until the turn is finished
		public bool ballShot { get; set; } 
		public Queue<Peg> pegsHit { get; private set; } = new Queue<Peg>();

		// Threading
		public EventWaitHandle finishHandle;

		// Store an integer for the score of the previous shot.
		// The thread that started this shot will check this score when it is updated
		// Shots are scored by the number of pegs hit. Triple score is rewarded for orange pegs
		public int previousShotScore { get; private set; }
		private static int OrangePegScore = 3;
		private static int BluePegScore = 1;

		public LevelComponent()
		{

			// Create a new world that holds all physics information
			world = new World();
			ballShot = false;

			// Create all of the level objects
			loadLevel();
			shooter = new BallShooter(this);
			offScreenBox = new OffScreenBox(0, -6, world, this);
			lWall = new Wall(-5.25f, 0, world);
			rWall = new Wall(5.25f, 0, world);
		}

		private void loadLevel()
		{
			// Load data from level1 file
			string[] pegPositions = File.ReadAllLines("../../../Level1.txt");

			// Create all of the pegs in the level
			pegs = new HashSet<Peg>();
			bool isOrange;
			foreach (string position in pegPositions)
			{
				string[] pos = position.Split(' ');
				// Lines have the X pos, Y pos, and wether the peg is orange or blue				
				isOrange = pos[2] == "O"? true : false;
				pegs.Add( new Peg(float.Parse(pos[0]), float.Parse(pos[1]), this, isOrange) );
			}
			
		}

		// This function takes in an angle in degrees as input, and shoots the ball at that angle
		public void shootAtAngle(int angle)
		{ 
			shooter.shoot(angle);
			ballShot = true;
		}

		private void shotFinished()
		{
			// Currently, this function runs every frame *before* a ball is shot, and that causes a lot of issues
			// So, this ensures the function will notify the previous thread only when a shot has been made and finished
			if(pegsHit.Count < 1)
				return;

			shooter.removeBall();

			int score = 0;

			Peg peg;
			while (pegsHit.Count > 0)
			{
				peg = pegsHit.Dequeue();

				score = peg.isOrange ? score + OrangePegScore : score + BluePegScore;

				// This is necessary to avoid attepting to remove the same peg from the world twice
				if (pegs.Contains(peg))
				{
					world.Remove(peg.pegBody);
					pegs.Remove(peg);
				}
			}

			ballShot = false;
			previousShotScore = score;
			
			// Notify the algorithm thread that the shot has finished.
			finishHandle.Set();
		}

		public void Update(GameTime gameTime)
		{
			// Input
			HandleKeyboard(gameTime);
			//HandleMouse();

			// If the turn has ended, remove the ball and all the pegs that have been hit
			// This will be checked every frame that the ball isn't active, may not be the best
			if(ballShot == false)
			{
				shotFinished();
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

		public void Draw(SpriteBatch spriteBatch)
		{
			// Draw each peg
			foreach(Peg peg in pegs)
				peg.draw(spriteBatch);

			// Draw the ball shooter

			shooter.draw(spriteBatch);
		}

		// For some reason I need to convert between Aether2D vectors to Microsoft Vectors manually
		// This method is used by game objects such as pegs and balls when drawing
		public static Microsoft.Xna.Framework.Vector2 convertVec(Vector2 aetherVec)
		{
			return new Microsoft.Xna.Framework.Vector2(aetherVec.X, aetherVec.Y);
		}

	}	
}
