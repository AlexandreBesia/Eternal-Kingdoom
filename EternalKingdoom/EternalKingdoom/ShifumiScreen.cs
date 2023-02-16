using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Media;

namespace EternalKingdoom
{
    class ShifumiScreen : Screen
    {
        // Private attribute(s)

        private float _generalRotation;

        private int _playerScore, _enemyScore, _status, _counter, _playerChoice, _enemyChoice, _scoreToWin;

        private string _textWinner;

        private Random _rand;

        private Point _armSize, _punchPos;
        
        private SpriteFont _fontArial;

        private Texture2D[] _arms;
        private Texture2D _background, _playerTexture, _enemyTexture, _punchTexture;

        private Button _btnRock, _btnPaper, _btnScissors, _helpButton;

        // Components list
        private List<Component> _gameComponents; // contains buttons

        // Public constructor(s)

        public ShifumiScreen(MonoGame _monogame) : base(_monogame)
        {

        }

        // Public virtual method(s)

        /// <summary>
        /// Initialize the Shifumi content
        /// </summary>
        public override void Initialize()
        {
            _generalRotation = 0;
            _playerScore = 0;
            _enemyScore = 0;
            _status = 0;
            _scoreToWin = 3;
            _textWinner = "";
            _armSize = new Point((int)(_monogame.Window.ClientBounds.Width * 0.5), (int)(_monogame.Window.ClientBounds.Height * 0.2));
            _rand = new Random();

            base.Initialize();
        }

        /// <summary>
        /// Load the Shifumi content
        /// </summary>
        protected override void LoadContent()
        {
            _background = _monogame.Content.Load<Texture2D>("shifumi/shifumiBackground");

            _arms = new Texture2D[]
            {
                _monogame.Content.Load<Texture2D>("shifumi/rock"),
                _monogame.Content.Load<Texture2D>("shifumi/paper"),
                _monogame.Content.Load<Texture2D>("shifumi/scissors")

            };

            _playerTexture = _arms[0];
            _enemyTexture = _arms[0];
            _punchTexture = _arms[0];
            _fontArial = _monogame.Content.Load<SpriteFont>("fonts/Arial34");

            Vector2 vButtons = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.06));

            _btnRock = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"),
                (int)vButtons.X, (int)vButtons.Y)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.5)),
                Text = "Pierre",
            };
            _btnPaper = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"),
                (int)vButtons.X, (int)vButtons.Y)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.22), (int)(_monogame.Window.ClientBounds.Height * 0.5)),
                Text = "Papier",
            };
            _btnScissors = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"),
                (int)vButtons.X, (int)vButtons.Y)
            {
                Position = new Vector2((int)(_monogame.Window.ClientBounds.Width * 0.34), (int)(_monogame.Window.ClientBounds.Height * 0.5)),
                Text = "Ciseaux",
            };

            _helpButton = new Button(_monogame.Content.Load<Texture2D>("button/buttonCircle"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), (int)(_monogame.Window.ClientBounds.Width * 0.03), (int)(_monogame.Window.ClientBounds.Height * 0.06))
            {
                Position = new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.85), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.05)),
                Text = "?",
            };

            _btnRock.Click += _btnRock_Click;
            _btnPaper.Click += _btnPaper_Click;
            _btnScissors.Click += _btnScissors_Click;
            _helpButton.Click += _helpButton_Click;

            _gameComponents = new List<Component>()
            {
                _btnRock,
                _btnPaper,
                _btnScissors,
                _helpButton
            };
        }

        /// <summary>
        /// Update the position of the characters and check the state of the minigame.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            foreach (var component in _gameComponents)
                component.Update(gameTime);

            switch(_status)
            {
                case 0:
                    break;
                case 1:
                    _counter++;
                    if (_counter % 100 >= 15 && _counter % 100 < 30)
                    {
                        _generalRotation += (float)Math.PI / 80;
                    }
                    else if (_counter % 100 >= 30 && _counter % 100 < 45)
                    {
                        _generalRotation -= (float)Math.PI / 80;
                    }
                    else if(_counter % 100 == 45)
                    {
                        if (_counter > 200)
                        {
                            _playerTexture = _arms[_playerChoice];
                            _enemyTexture = _arms[_enemyChoice];
                            _status = 2;
                            checkHeatWinner();
                        }
                        else
                        {
                            _counter += 68;
                        }
                    }
                    break;
                case -1:
                    _counter++;
                    if (_counter == 200)
                    {
                        Initialize();

                        MediaPlayer.Play(_monogame.Content.Load<Song>("music/maingame_CHIPTUNE_The_Old_Tower_Inn"));
                        MediaPlayer.IsRepeating = true;
                        _monogame.ActiveScreen = MonoGame.ScreenType.MainGame;
                    }

                    break;
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// Check the state of the minigame to draw the characters correctly
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Rectangle(new Point(0, 0), new Point(_monogame.Window.ClientBounds.Width, _monogame.Window.ClientBounds.Height)), Color.White);

            foreach (var component in _gameComponents)
                component.Draw(gameTime, spriteBatch);

            // texts
            spriteBatch.DrawString(_fontArial, _playerScore.ToString(),
                new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.3), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.89)), Color.White);
            spriteBatch.DrawString(_fontArial, _enemyScore.ToString(),
                new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.7), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.89)), Color.White);


            spriteBatch.DrawString(_fontArial, _textWinner, new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.5 - _fontArial.MeasureString(_textWinner).X / 2), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.15)), Color.White);

            // show players' textures
            spriteBatch.Draw(_playerTexture, new Rectangle(
                new Point((int)(_monogame.Window.ClientBounds.Width * -0.1), (int)(_monogame.Window.ClientBounds.Height * 0.6)), _armSize), null, Color.White,
                -_generalRotation, new Vector2(0, 0), SpriteEffects.None, 0);

            spriteBatch.Draw(_enemyTexture, new Rectangle(
                new Point((int)(_monogame.Window.ClientBounds.Width * 1.1), (int)(_monogame.Window.ClientBounds.Height * 0.6)), _armSize), null, Color.Beige,
                _generalRotation, new Vector2((int)(_enemyTexture.Width), 0), SpriteEffects.FlipHorizontally, 0);


            spriteBatch.End();
            base.Draw(gameTime, spriteBatch);
        }

        // Private method(s)

        /// <summary>
        /// At the click of rock, choose rock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnRock_Click(object sender, System.EventArgs e)
        {
            generateChoices(0);
        }

        /// <summary>
        /// At the click of paper, chose paper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnPaper_Click(object sender, System.EventArgs e)
        {
            generateChoices(1);
        }

        /// <summary>
        /// At the click of Scissors, chose Scissors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnScissors_Click(object sender, System.EventArgs e)
        {
            generateChoices(2);

        }

        /// <summary>
        /// Display help when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _helpButton_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Dans ce combat sanglant de négociations politiques, vous devez battre votre adversaire lors d'un shifumi à " + _scoreToWin + " manches gagnantes");
        }

        /// <summary>
        /// Generate the choices of the opponent and the animation
        /// </summary>
        /// <param name="player"></param>
        private void generateChoices(int player)
        {
            // set moves that players will do
            _playerChoice = player;
            _enemyChoice = _rand.Next(3);
            // reduce the chance of getting a draw (1/9 instead of 1/3) because it's boring otherwise
            if(_playerChoice == _enemyChoice)
            {
                _enemyChoice = _rand.Next(3);
            }

            // go to the animation
            _status = 1;
            _counter = 0;
            _playerTexture = _arms[0];
            _enemyTexture = _arms[0];
            hideShowButtons();
        }

        /// <summary>
        /// Check the winner 
        /// </summary>
        private void checkHeatWinner()
        {
            int diff = _playerChoice - _enemyChoice;
            switch(diff)
            {
                case 0:
                    //tie
                    _status = 0;
                    hideShowButtons();
                    break;

                case 1:
                case -2:
                    //won
                    _playerScore++;
                    if(_playerScore == _scoreToWin)
                    {
                        _status = -1;
                        _counter = 0;
                        _textWinner = "Vous avez gagné !";
                        _monogame.shifumiWon = true;
                        _monogame.shifumiFinished = true;
                    }
                    else
                    {
                        _status = 0;
                        hideShowButtons();
                    }
                    break;

                default:
                    //lost
                    _enemyScore++;
                    if(_enemyScore == _scoreToWin)
                    {
                        _status = -1;
                        _counter = 0;
                        _textWinner = "Vous avez perdu !";
                        _monogame.shifumiWon = false;
                        _monogame.shifumiFinished = true;
                    }
                    else
                    {
                        _status = 0;
                        hideShowButtons();
                    }
                    break;
            }
        }

        private void hideShowButtons()
        {
            foreach (Button component in _gameComponents)
                component.Position = -component.Position;
        }

    }

}
