using System;
using System.Collections.Generic;
using System.Text;
using MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Media;

namespace EternalKingdoom
{
    public class CreditsScreen : Screen
    {
        private Texture2D _logoHEArc;

        public CreditsScreen(MonoGame game) : base(game)
        {
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_logoHEArc, new Microsoft.Xna.Framework.Rectangle(new Microsoft.Xna.Framework.Point(0, 0),
                new Microsoft.Xna.Framework.Point(_monogame.Window.ClientBounds.Width, _monogame.Window.ClientBounds.Height)), Color.White);

            MediaPlayer.IsRepeating = true;

            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
        protected override void LoadContent()
        {
            _logoHEArc = _monogame.Content.Load<Texture2D>("menus/logoHEArc");

            int buttonSizeX = 250;
            int buttonSizeY = 80;

            Button quitButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), buttonSizeX, buttonSizeY
)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.90 - buttonSizeX / 2), (int)(_monogame.Window.ClientBounds.Height * 0.85)),
                Text = "Quitter",
            };

            quitButton.Click += QuitButton_Click;

            _components.Add(quitButton);

        }
        private void QuitButton_Click(object sender, System.EventArgs e)
        {
            _monogame.ActiveScreen = MonoGame.ScreenType.Menu;
        }
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F9))
                _monogame.ActiveScreen = MonoGame.ScreenType.MainGame;

            base.Update(gameTime);
        }
    }
}
