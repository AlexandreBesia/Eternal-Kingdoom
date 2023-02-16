using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Media;

namespace EternalKingdoom
{
    public class Scenario
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public Texture2D Picture { get; set; }
        public List<string> Choices { get; set; }
        public List<int> FirstChoice { get; set; }
        public List<int> SecondChoice { get; set; }

        /// <summary>
        /// popularity , money, defense, population
        /// </summary>
        public List<int> GaugeImpacted { get; set; }

        /// <summary>
        /// 0 = normal, 1 = fight, 2 = riddle
        /// </summary>
        public int cardType { get; set; }
    }

    public class MainGameScreen : Screen
    {

        // Private attribute(s)

        private bool isNewCardDisplayed = false;

        private Button _quitButton, _helpButton;

        private Card card;

        // Public constructor(s)

        public MainGameScreen(MonoGame game) : base(game)
        {

        }

        // Public virtual method(s)

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Load all the main game content
        /// </summary>
        protected override void LoadContent()
        {
            _monogame.ActiveKingState = MonoGame.KingState.Alive;

            _quitButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), (int)(_monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.06))
            {
                Position = new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.7), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.05)),
                Text = "Quitter",
            };

            _helpButton = new Button(_monogame.Content.Load<Texture2D>("button/buttonCircle"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), (int)(_monogame.Window.ClientBounds.Width * 0.03), (int)(_monogame.Window.ClientBounds.Height * 0.06))
            {
                Position = new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.85), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.05)),
                Text = "?",
            };

            _quitButton.Click += _quitButton_Click;
            _helpButton.Click += _helpButton_Click;

        }

        /// <summary>
        /// Check the state of the game to redirect to the end, a minigame or the next card.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            _quitButton.Update(gameTime);
            _helpButton.Update(gameTime);

            if (!isNewCardDisplayed)
            {
                reset();
                Card.YearsLived = 1;
            }

            if(Card.YearsLived >= 99 || Card.NoMoreCards == true)
            {
                _monogame.ActiveKingState = MonoGame.KingState.DeathFromOldAge;
                End();

            }

            card.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Check the state of the gauges and the card to draw the gauges correctly.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/mainGameBackground"), new Rectangle(new Point(0, 0), new Point(_monogame.Window.ClientBounds.Width, _monogame.Window.ClientBounds.Height)), Color.White);

            _quitButton.Draw(gameTime, spriteBatch);
            _helpButton.Draw(gameTime, spriteBatch);

            if (card != null)
            {
                card.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(_monogame.Content.Load<SpriteFont>("fonts/Arial34"), "Vous avez célébré l'année " + Card.YearsLived + " de votre règne.", new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.01), (int)(_monogame.Window.ClientBounds.Height * 0.01)), Color.Black);
                
                if (card.PopularityGauge > 24)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Popularity"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.35), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Green);
                }
                else if (card.PopularityGauge > 16)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Popularity"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.35), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Yellow);
                }
                else if (card.PopularityGauge > 8)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Popularity"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.35), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.DarkOrange);
                }
                else if (card.PopularityGauge > 0)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Popularity"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.35), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Red);
                }
                else
                {
                    _monogame.ActiveKingState = MonoGame.KingState.DeathFromRevolution;
                    End();
                }

                if (card.MoneyGauge > 24)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Money"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.43), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Green);
                }
                else if (card.MoneyGauge > 16)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Money"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.43), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Yellow);
                }
                else if (card.MoneyGauge > 8)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Money"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.43), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.DarkOrange);
                }
                else if (card.MoneyGauge > 0)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Money"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.43), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Red);
                }
                else
                {
                    _monogame.ActiveKingState = MonoGame.KingState.DeathFromBankruptcy;
                    End();
                }

                if (card.DefenseGauge > 24)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Defense"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.51), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Green);
                }
                else if (card.DefenseGauge > 16)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Defense"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.51), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Yellow);
                }
                else if (card.DefenseGauge > 8)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Defense"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.51), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.DarkOrange);
                }
                else if (card.DefenseGauge > 0)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/Defense"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.51), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Red);
                }
                else
                {
                    _monogame.ActiveKingState = MonoGame.KingState.DeathFromOverthrow;
                    End();
                }

                if (card.PopulationGauge > 24)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/People"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.59), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Green);
                }
                else if (card.PopulationGauge > 16)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/People"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.59), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Yellow);
                }
                else if (card.PopulationGauge > 8)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/People"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.59), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.DarkOrange);
                }
                else if (card.PopulationGauge > 0)
                {
                    spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/People"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.59), (int)(_monogame.Window.ClientBounds.Height * 0.13), (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.Red);
                }
                else
                {
                    _monogame.ActiveKingState = MonoGame.KingState.DeathFromLoneliness;
                    End();
                }

                base.Draw(gameTime, spriteBatch);

            }
            else
            {
                reset();
                Card.YearsLived = 1;
            }

            spriteBatch.End();
        }

        // Private method(s)

        /// <summary>
        /// When the quit button is clicked, a messagebox is shown confirm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _quitButton_Click(object sender, System.EventArgs e)
        {
            DialogResult messageBoxAnswer = MessageBox.Show("Êtes-vous sûr de vouloir quitter ?", "Quitter", MessageBoxButtons.YesNo);

            if (messageBoxAnswer == DialogResult.Yes)
            {
                reset();
                Card.YearsLived = 1;
                _monogame.ActiveScreen = MonoGame.ScreenType.Menu;

                MediaPlayer.Play(_monogame.Content.Load<Song>("music/menu_CHIPTUNE_The_Bards_Tale"));
                MediaPlayer.IsRepeating = true;
            }
        }
        private void _helpButton_Click(object sender, System.EventArgs e)
        {
            DialogResult messageBoxAnswer = MessageBox.Show("Lisez le scénario de la carte et choisissez une réponse" +
                " en faisant glisser la carte à gauche ou à droite.\n\nMais, attention aux jauges en dessus de la carte," +
                " si l'une d'entre elles tombe à 0, c'est GAME OVER. \n\nSurvivez jusqu'à mourir d'une mort douce et naturelle, après un long règne prospère !", "Aide", MessageBoxButtons.OK);
        }

        /// <summary>
        /// End the game at the death of the king
        /// </summary>
        private void End()
        {
            reset();

            if (_monogame.ActiveKingState == MonoGame.KingState.DeathFromOldAge)
            {
                MediaPlayer.Play(_monogame.Content.Load<Song>("music/endmenu_Victory"));
                MediaPlayer.IsRepeating = true;
            }
            else
            {
                MediaPlayer.Play(_monogame.Content.Load<Song>("music/endmenu_Defeat"));
                MediaPlayer.IsRepeating = true;
            }
            _monogame.ActiveScreen = MonoGame.ScreenType.EndGameScreen;

        }

        /// <summary>
        /// Resert the values of several attributes
        /// </summary>
        private void reset()
        {
            card = new Card(_monogame.Content.Load<Texture2D>("maingame/cardBackground"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), _monogame);

            Card.NoMoreCards = false;

            card.PopularityGauge = 32;
            card.MoneyGauge = 32;
            card.DefenseGauge = 32;
            card.PopulationGauge = 32;
            isNewCardDisplayed = true;

            MediaPlayer.Play(_monogame.Content.Load<Song>("music/maingame_CHIPTUNE_The_Old_Tower_Inn"));
            MediaPlayer.IsRepeating = true;
        }
    }
}
