using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework.Media;

namespace EternalKingdoom
{
    public class EndGameScreen : Screen
    {
        // Private attribute(s)

        private SpriteFont _endText;
        private String endText;
        private Texture2D _background;

        // Public constructor(s)

        public EndGameScreen(MonoGame game) : base(game)
        {

        }

        // Public virtual method(s)

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Load the content of the end menu
        /// </summary>
        protected override void LoadContent()
        {
            _background = _monogame.Content.Load<Texture2D>("menus/defeatmenuBackground");

            _endText = _monogame.Content.Load<SpriteFont>("fonts/Arial24");

            int buttonSizeX = (int)(_monogame.Window.ClientBounds.Width * 0.2);
            int buttonSizeY = (int)(_monogame.Window.ClientBounds.Width * 0.04);

            Button replayButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), buttonSizeX, buttonSizeY)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.5 - (buttonSizeX / 2)), (int)(_monogame.Window.ClientBounds.Height * 0.50)),
                Text = "Nouvelle partie",
            };
            Button quitButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), buttonSizeX, buttonSizeY
)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.5 - buttonSizeX / 2), (int)(_monogame.Window.ClientBounds.Height * 0.60)),
                Text = "Quitter",
            };

            replayButton.Click += ReplayButton_Click;
            quitButton.Click += QuitButton_Click;

            _components.Add(replayButton);
            _components.Add(quitButton);
        }

        /// <summary>
        /// Update the end menu content
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F2))
                _monogame.ActiveScreen = MonoGame.ScreenType.MainGame;

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the end menu and set the correct end dialogue
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // background
            spriteBatch.Begin();
            spriteBatch.Draw(_background, new Microsoft.Xna.Framework.Rectangle(new Microsoft.Xna.Framework.Point(0, 0),
                new Microsoft.Xna.Framework.Point(_monogame.Window.ClientBounds.Width, _monogame.Window.ClientBounds.Height)), Color.White);

            switch (_monogame.ActiveKingState)
            {
                case MonoGame.KingState.Alive:
                    endText = "Vous êtes vivant... Ce n'est pas normal, roi-sorcier, hérétique, au bucher !";

                    break;
                case MonoGame.KingState.DeathFromOldAge:
                    _background = _monogame.Content.Load<Texture2D>("menus/victoryBackground");
                    endText = "Vous avez gagné. Vous vous éteignez paisiblement après " + Card.YearsLived + " ans de règne prospère. \nVotre peuple se souviendra de vous comme un roi juste, fort et bon !";

                    break;
                case MonoGame.KingState.DeathFromRevolution:
                    endText = "Vous avez perdu. Vous êtes mort à " + Card.YearsLived + " ans de règne, assassiné par votre propre peuple.\nVotre cruauté face à vos citoyens a été l'étincelle décisive dans l'élaboration d'une révolution.";

                    break;
                case MonoGame.KingState.DeathFromBankruptcy:
                    endText = "Vous avez perdu. Vous êtes mort à " + Card.YearsLived + " ans de règne, mort de faim, dans un caniveau.\nVotre vie de débauche vous a mené à une faillite du royaume, vous avez perdu toute votre fortune.";

                    break;
                case MonoGame.KingState.DeathFromOverthrow:
                    endText = "Vous avez perdu. Vous êtes mort à " + Card.YearsLived + " ans de règne, renversé par l'armée ennemie.\nVotre bonté vous a mené à être trop laxiste avec l'ennemi, ce dernier profitant de votre faiblesse militaire pour vous porter le coup fatal.";

                    break;
                case MonoGame.KingState.DeathFromLoneliness:
                    endText = "Vous avez perdu. Vous êtes mort à " + Card.YearsLived + " ans de règne, seul dans votre grand château en ruines.\nVous n'avez pas su protéger votre peuple bien aimé et tous ces citoyens sont morts un par un.\nPas de peuple, pas de roi.";
                    break;
                default:
                    endText = "Vous êtes vivant... Ce n'est pas normal, roi-sorcier, hérétique, au bucher !";
                    break;
            }

            // button texture used behind the end text to make it more readable
            Vector2 endTextSize = _endText.MeasureString(endText);
            float XEndTextSize = endTextSize.X;
            float YEndTextSize = endTextSize.Y;

            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("button/ButtonCentered"), new Microsoft.Xna.Framework.Rectangle(
                (int)((_monogame.Window.ClientBounds.Width * 0.5) - (XEndTextSize / 2) - (_monogame.Window.ClientBounds.Width * 0.125)), 
                (int)((_monogame.Window.ClientBounds.Height * 0.008) - (YEndTextSize) + (_monogame.Window.ClientBounds.Height * 0.08)),
                (int)(XEndTextSize + _monogame.Window.ClientBounds.Width * 0.25), 
                (int)(YEndTextSize + _monogame.Window.ClientBounds.Height * 0.15)), 
                Color.White);


            MediaPlayer.IsRepeating = true;
            spriteBatch.DrawString(_endText, endText, new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.5 - XEndTextSize/2), (int)(_monogame.Window.ClientBounds.Height * 0.1)), Color.Black);

            base.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        // Private method(s)

        /// <summary>
        /// Restart the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplayButton_Click(object sender, System.EventArgs e)
        {
            Card.YearsLived = 1;

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
    }
}
