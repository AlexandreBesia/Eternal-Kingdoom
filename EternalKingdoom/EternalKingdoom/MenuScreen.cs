using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Media;

namespace EternalKingdoom
{
    public class MenuScreen : Screen
    {
        // Private attribute(s)

        private Texture2D _background;

        // Public constructor(s)

        public MenuScreen(MonoGame game) : base(game)
        {

        }

        // Public virtual method(s)

        /// <summary>
        /// Initialize the menu
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Load the menu content
        /// </summary>
        protected override void LoadContent()
        {
            _background = _monogame.Content.Load<Texture2D>("menus/menuBackground");

            int buttonSizeX = 250;
            int buttonSizeY = 80;

            Button playButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), buttonSizeX, buttonSizeY)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.5 - (buttonSizeX / 2)), (int)(_monogame.Window.ClientBounds.Height * 0.35)),
                Text = "Jouer",
            };

            Button quitButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), buttonSizeX, buttonSizeY)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.5 - (buttonSizeX / 2)), (int)(_monogame.Window.ClientBounds.Height * 0.65)),
                Text = "Quitter",
            };
            Button creditButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), buttonSizeX, buttonSizeY)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.5 - (buttonSizeX / 2)), (int)(_monogame.Window.ClientBounds.Height * 0.48)),
                Text = "Crédits",
            };

            playButton.Click += PlayButton_Click;
            quitButton.Click += QuitButton_Click;
            creditButton.Click += CreditButton_Click;

            _components.Add(playButton);
            _components.Add(quitButton);
            _components.Add(creditButton);
        }

        /// <summary>
        /// Update the game
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the menu
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // background
            spriteBatch.Begin();
            spriteBatch.Draw(_background, new Microsoft.Xna.Framework.Rectangle(new Microsoft.Xna.Framework.Point(0, 0), 
                new Microsoft.Xna.Framework.Point(_monogame.Window.ClientBounds.Width, _monogame.Window.ClientBounds.Height)), Color.White);

            // game title
            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("menus/Logo"), new Microsoft.Xna.Framework.Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.3), (int)(_monogame.Window.ClientBounds.Height * 0.05), (int)(_monogame.Window.ClientBounds.Width * 0.4), (int)(_monogame.Window.ClientBounds.Height * 0.2)), Color.White);

            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        // Private method(s)

        /// <summary>
        /// Start the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, System.EventArgs e)
        {
            MediaPlayer.Play(_monogame.Content.Load<Song>("music/maingame_CHIPTUNE_The_Old_Tower_Inn"));
            MediaPlayer.IsRepeating = true;
            _monogame.ActiveScreen = MonoGame.ScreenType.MainGame;
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitButton_Click(object sender, System.EventArgs e)
        {
            _monogame.Exit();
        }

        /// <summary>
        /// Display the credits
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreditButton_Click(object sender, System.EventArgs e)
        {
            _monogame.ActiveScreen = MonoGame.ScreenType.Credits;
        }
    }
}
