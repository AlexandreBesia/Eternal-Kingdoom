using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace EternalKingdoom
{
    public class MonoGame : Game
    {
        // public attribute(s)

        public enum ScreenType { Menu, MainGame, Riddle, Combat, EndGameScreen, Shifumi, Credits }
        public enum KingState { Alive, DeathFromOldAge, DeathFromRevolution, DeathFromBankruptcy, DeathFromOverthrow, DeathFromLoneliness }

        public bool fightFinished = false;
        public bool riddleFinished = false;
        public bool shifumiFinished = false;
        public bool fightWon = false; // to know if fight has been won
        public bool riddleWon = false;
        public bool shifumiWon = false;

        // private attribute(s)

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Screen> _screens;

        public ScreenType ActiveScreen { get; set; }
        public KingState ActiveKingState { get; set; }


        // Public constructor(s)

        public MonoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        // Public method(s)

        /// <summary>
        /// Converts a string to a break-word string by adding newline characters
        /// at the closest space character available once the size provided is reached.
        /// </summary>
        /// <param name="str">The original string</param>
        /// <param name="font">The spritefont used to draw the string</param>
        /// <param name="containerSize">The max horizontal size available</param>
        /// <returns>The break-word string</returns>
        public string BreakWordString(string str, SpriteFont font, float containerSize)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                float size = font.MeasureString(str.Substring(0, i + 1)).X;
                if (size >= containerSize)
                {
                    // Attempt to find the last space character prior to the current character
                    int breakpoint = str.LastIndexOf(' ', i);
                    if (breakpoint != -1)
                    {
                        str = str.Substring(0, breakpoint) + '\n' + str.Substring(breakpoint + 1);
                    }
                }
            }

            return str;
        }


        // Protected virtual method(s)

        /// <summary>
        /// Initialize the game
        /// </summary>
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            
            _graphics.ApplyChanges();

            // The order in which the screens are added must be same as the enum's
            _screens = new List<Screen>()
            {
                new MenuScreen(this),
                new MainGameScreen(this),
                new RiddleScreen(this),
                new CombatScreen(this),
                new EndGameScreen(this),
                new ShifumiScreen(this),
                new CreditsScreen(this)
            };

            MediaPlayer.Play(Content.Load<Song>("music/menu_CHIPTUNE_The_Bards_Tale"));
            MediaPlayer.IsRepeating = true;

            ActiveScreen = ScreenType.Menu;

            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// Load the game content
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();

            // Création d'un nouveau boutton
            Button exempleButton = new Button(Content.Load<Texture2D>("button/Button"), Content.Load<SpriteFont>("fonts/Arial24"), 1, 1)
            {
                Position = new Vector2(this.Window.ClientBounds.Width / 2, this.Window.ClientBounds.Height / 2),
            };

            // Lien entre l'événement clik du bouton avec la méthode qui va gérer ce qu'il se passe à ce click

            // Remplissage de la liste des composants avec les boutons, les labels etc...
        }

        /// <summary>
        /// Update the content of the game
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            _screens[(int)ActiveScreen].Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the content of the game
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.PeachPuff);

            _screens[(int)ActiveScreen].Draw(gameTime, _spriteBatch);
            base.Draw(gameTime);
        }
    }
}
