﻿using System;
using System.Linq;
using Humper.Responses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Humper.Sample.Basic
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			this.Window.AllowUserResizing = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			this.NextScene();

			this.scene.Initialize();

			base.Initialize();

		}

		private IScene scene;

		private Type[] scenes = new[] 
		{ 
			typeof(TopdownScene),
			typeof(PlatformerScene),
			typeof(ParticlesScene),
		};

		private int sceneIndex = -1;

		private SpriteFont font;

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			this.font = this.Content.Load<SpriteFont>("font");
		}

		private void NextScene()
		{
			this.sceneIndex = (this.sceneIndex + 1) % this.scenes.Length;
			this.scene = (IScene) Activator.CreateInstance(this.scenes[this.sceneIndex]);
			this.scene.Initialize();
		}

		private KeyboardState state;

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif
			if (Keyboard.GetState().IsKeyDown(Keys.Enter) && state.IsKeyUp(Keys.Enter))
				this.NextScene();
			state = Keyboard.GetState();

			this.scene.Update(gameTime);

			base.Update(gameTime);
		}

		private void DrawCell(int x, int y, int w, int h, float alpha)
		{
			if(Keyboard.GetState().IsKeyDown(Keys.Space))
				spriteBatch.DrawStroke(new Rectangle(x,y,w,h), new Color(Color.White, alpha));
		}

		private void DrawBox(IBox box)
		{
			Color color;

			if (box.HasTag(Tags.Group1))
				color = Color.White;
			else if (box.HasTag(Tags.Group3))
				color = Color.Red;
			else if (box.HasTag(Tags.Group4))
				color = Color.Green;
			else if (box.HasTag(Tags.Group5))
				color = Color.Yellow;
			else
				color = new Color(165, 155, 250);

			spriteBatch.Draw(box.Bounds, color, 0.3f);
		}

		private void DrawString(string message, int x, int y, float alpha)
		{
			var size = this.font.MeasureString(message);
			if (Keyboard.GetState().IsKeyDown(Keys.Space))
				spriteBatch.DrawString(this.font, message, new Vector2( x - size.X / 2, y - size.Y / 2), new Color(Color.White, alpha));
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(new Color(44,45,51));

			spriteBatch.Begin(blendState: BlendState.NonPremultiplied);

			var b = this.scene.World.Bounds;

			this.scene.World.DrawDebug((int)b.X, (int)b.Y, (int)b.Width, (int)b.Height, DrawCell, DrawBox, DrawString);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}

