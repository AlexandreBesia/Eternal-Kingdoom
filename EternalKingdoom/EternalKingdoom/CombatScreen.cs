using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace EternalKingdoom
{
    class CombatScreen : Screen
    {
        // Public  attribute(s)

        public static Texture2D enemyTexture { get; set; }

        // Private attribute(s)

        private int _enemyHp, _heroHp, _enemyHpMax, _heroHpMax, // hp remaining, and hp total
            _enemyAttack, _heroAttack, // need to be located there to be used moments after the value is set
            _fightStatus, // explained in Initialize, have fun
            _counter, // just a counter used for animation
            _moveWidth, _moveHeight, // animation
            _heroDice, _enemyDice; // result to show

        private Point _heroLocation, _enemyLocation; // animation too

        private string _damageResumeString, _enemyAttackString; // spritefont texts

        private bool _hasWon, _hasAttacked, _showNordVpn, _defenseSuccess;

        private float _heroRotation, _enemyRotation;

        private SpriteFont _fontArial;

        private Button _attackButton, _defenseButton, _rulesButton;

        private Random _rand;

        private Vector2 _attackButtonPos, _defenseButtonPos;

        private Texture2D _heroTexture, _hpGreen, _hpBorder, _nordVpn, _background; // green life bars
        private Texture2D[] _diceTexture; // contains faces of the dice

        // Components list
        private List<Component> _gameComponents; // contains buttons

        // Public constructor(s)

        public CombatScreen(MonoGame _monogame) : base(_monogame)
        {

        }

        // Public virtual method(s)

        /// <summary>
        /// Initialize the fight content
        /// </summary>
        public override void Initialize()
        {
            // Window initialization
            // initialize the game size by the screen size to avoid hell in window's launch
            
            _enemyHpMax = 15;
            _heroHpMax = 15;
            _enemyHp = _enemyHpMax;
            _heroHp = _heroHpMax;
            _enemyAttack = 0;
            _heroAttack = 0;
            _enemyAttackString = "";
            _damageResumeString = "";
            _heroRotation = 0;
            _enemyRotation = 0;
            _fightStatus = 0; // 0 : fight just started -> 1 : enemy attack -> 2 : player's choice, 3 : if damage to set, 4 : if no damage to set
                              // then will return to 1
                              // -1 : game finished
            _heroDice = 0;
            _enemyDice = 0;

            _showNordVpn = false;
            _defenseSuccess = false;

            _moveWidth = (int)(_monogame.Window.ClientBounds.Width * 0.035);
            _moveHeight = (int)(_monogame.Window.ClientBounds.Height * 0);
            _heroLocation = new Point((int)(_monogame.Window.ClientBounds.Width * 0.2125), (int)(_monogame.Window.ClientBounds.Height * 0.65));
            _enemyLocation = new Point((int)(_monogame.Window.ClientBounds.Width * 0.7875), (int)(_monogame.Window.ClientBounds.Height * 0.65));

            _hasWon = false; // true if player wins
            _rand = new Random();
            
            base.Initialize();
        }

        /// <summary>
        /// Load the fight content
        /// </summary>
        protected override void LoadContent()
        {
            // Buttons (attack and defense)
            _attackButtonPos = new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.1), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.88));
            _defenseButtonPos = new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.215), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.88));
            _attackButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"),
                (int)(_monogame.Window.ClientBounds.Width * 0.11), (int)(_monogame.Window.ClientBounds.Height * 0.06))
            {
                Position = _attackButtonPos,
                Text = "Attaquer",
            };
            _defenseButton = new Button(_monogame.Content.Load<Texture2D>("button/Button"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"),
                (int)(_monogame.Window.ClientBounds.Width * 0.11), (int)(_monogame.Window.ClientBounds.Height * 0.06))
            {
                Position = _defenseButtonPos,
                Text = "Défendre",
            };
            _rulesButton = new Button(_monogame.Content.Load<Texture2D>("button/buttonCircle"), _monogame.Content.Load<SpriteFont>("fonts/Arial24"), (int)(_monogame.Window.ClientBounds.Width * 0.03), (int)(_monogame.Window.ClientBounds.Height * 0.06))
            {
                Position = new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.85), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.05)),
                Text = "?",
            };

            // events
            _attackButton.Click += _attackButton_Click;
            _defenseButton.Click += _defenseButton_Click;
            _rulesButton.Click += _rulesButton_Click;

            // other things
            _fontArial = _monogame.Content.Load<SpriteFont>("fonts/Arial24");

            _background = _monogame.Content.Load<Texture2D>("fight/background");
            _heroTexture = _monogame.Content.Load<Texture2D>("fight/heroFight");

            _hpGreen = _monogame.Content.Load<Texture2D>("fight/green");
            _hpBorder = _monogame.Content.Load<Texture2D>("fight/BlackBorder");

            _nordVpn = _monogame.Content.Load<Texture2D>("fight/nordVpn");

            _diceTexture = new Texture2D[]
            {
                _monogame.Content.Load<Texture2D>("fight/dice/one"),
                _monogame.Content.Load<Texture2D>("fight/dice/two"),
                _monogame.Content.Load<Texture2D>("fight/dice/three"),
                _monogame.Content.Load<Texture2D>("fight/dice/four"),
                _monogame.Content.Load<Texture2D>("fight/dice/five"),
                _monogame.Content.Load<Texture2D>("fight/dice/six")
            };

            // Fill list of components
            _gameComponents = new List<Component>()
            {
                _attackButton,
                _defenseButton,
                _rulesButton
            };

        }

        /// <summary>
        /// Check the status of the fight and choice of action to update the life and status of characters
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Update components logic
            foreach (var component in _gameComponents)
                component.Update(gameTime);

            switch(_fightStatus)
            {
                case 0: _fightStatus++; // game starts, can be useful later
                    break;
                case 1: // opponent's attack is decided
                    opponentAttack();
                    _fightStatus++;
                    _attackButton.Position = _attackButtonPos;
                    _defenseButton.Position = _defenseButtonPos;
                    
                    // buttons visible a true
                    break;
                case 2: // player's turn, chooses the attack

                    _damageResumeString = "Choisissez votre type d'attaque";
                    _enemyDice = 0;
                    _heroDice = 0;
                    _enemyAttackString = "";
                    if (_enemyAttack >= 1 && _enemyAttack <= 3)
                    {
                        _enemyAttackString = "Petite attaque";
                    }
                    else if (_enemyAttack != 0)
                    {
                        _enemyAttackString = "Grosse attaque";
                    }
                    // wait for the user to click
                    break;
                case 3: // if defense or attack

                    if(_hasAttacked == true)
                    {
                        damageSetter();
                    }
                    else
                    {
                        defenseCheck();
                    }

                    break;
                case -1: // end of the game

                    _counter++;
                    if(_counter == 29)
                    {
                        _showNordVpn = false;
                    }
                    else if(_counter > 30 && _counter <= 45) // Amazing death animation, part 1
                    {
                        if(_hasWon == true) // check who lost and falls down
                        {
                            _enemyRotation+= (float)Math.PI / 15;

                            _monogame.Content.Load<SoundEffect>("music/Ta Da-SoundBible.com-1884170640").CreateInstance().Play();
                        }
                        else
                        {
                            _heroRotation+=(float)Math.PI / 15;

                            _monogame.Content.Load<SoundEffect>("music/Male Grunt-SoundBible.com-68178715").CreateInstance().Play();
                        }
                    }
                    else if(_counter > 45 && _counter <= 105) // Amazing death animation part 2
                    {
                        if(_hasWon == true)
                        {
                            _enemyLocation.Y += (int)(_monogame.Window.ClientBounds.Height * 0.01);
                        }
                        else
                        {
                            _heroLocation.Y += (int)(_monogame.Window.ClientBounds.Height * 0.01);
                        }
                    }
                    else if (_counter == 106) // Set result of the fight to the main game
                    {
                        if (_hasWon == true)
                        {
                            _monogame.fightWon = true;
                            _monogame.fightFinished = true;
                            _damageResumeString = "Vous avez gagné !";
                        }
                        else
                        {
                            _monogame.fightWon = false;
                            _monogame.fightFinished = true;
                            _damageResumeString = "Vous avez perdu !";
                        }
                    }
                    else if(_counter >= 110 && _counter < 190) // victory animation
                    {
                        if((_counter) % 40 < 20)
                        {
                            if(_hasWon == true)
                            {
                                _heroLocation.X -= (int)(_monogame.Window.ClientBounds.Width * 0.005);
                            }
                            else
                            {
                                _enemyLocation.X -= (int)(_monogame.Window.ClientBounds.Width * 0.005);
                            }
                        }
                        else
                        {
                            if (_hasWon == true)
                            {
                                _heroLocation.X += (int)(_monogame.Window.ClientBounds.Width * 0.005);
                            }
                            else
                            {
                                _enemyLocation.X += (int)(_monogame.Window.ClientBounds.Width * 0.005);
                            }
                        }
                    }
                    else if(_counter == 250) // back to the main game
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
        /// Check the status of the fight and the actions to draw the life and state of the characters correcly
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();
            spriteBatch.Draw(_background, new Rectangle(new Point(0, 0), new Point(_monogame.Window.ClientBounds.Width, _monogame.Window.ClientBounds.Height)), Color.White);

            // components graphics update
            foreach (var component in _gameComponents)
                component.Draw(gameTime, spriteBatch);

            // hp / hp max
            spriteBatch.DrawString(_fontArial, _heroHp + "/" + _heroHpMax, 
                new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.325), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.4)), Color.White);

            spriteBatch.DrawString(_fontArial, _enemyHp + "/" + _enemyHpMax, 
                new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.9), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.4)), Color.White);

            // enemy's attack (big or little)
            spriteBatch.DrawString(_fontArial, _enemyAttackString,
                new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.7875 - _fontArial.MeasureString(_enemyAttackString).X / 2), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.3)), Color.White);    
            // main comment line, important
            spriteBatch.DrawString(_fontArial, _damageResumeString, new Vector2(Convert.ToInt32(_monogame.Window.ClientBounds.Width * 0.5 - _fontArial.MeasureString(_damageResumeString).X / 2), Convert.ToInt32(_monogame.Window.ClientBounds.Height * 0.83)), Color.White);
           
            // life gauges and their borders
            spriteBatch.Draw(_hpGreen, new Rectangle(
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.4)),
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.225 / _heroHpMax * _heroHp), (int)(_monogame.Window.ClientBounds.Height * 0.03))), Color.White);
            spriteBatch.Draw(_hpGreen, new Rectangle(
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.675), (int)(_monogame.Window.ClientBounds.Height * 0.4)),
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.225 / _enemyHpMax * _enemyHp), (int)(_monogame.Window.ClientBounds.Height * 0.03))), Color.White);
            spriteBatch.Draw(_hpBorder, new Rectangle(
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.4)),
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.225), (int)(_monogame.Window.ClientBounds.Height * 0.03))), Color.White);
            spriteBatch.Draw(_hpBorder, new Rectangle(
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.675), (int)(_monogame.Window.ClientBounds.Height * 0.4)),
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.225), (int)(_monogame.Window.ClientBounds.Height * 0.03))), Color.White);

            // player's picture
            spriteBatch.Draw(_heroTexture, new Rectangle(_heroLocation,
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.225), (int)(_monogame.Window.ClientBounds.Height * 0.4))), 
                null, Color.White, _heroRotation, new Vector2(_heroTexture.Width/2, _heroTexture.Height/2), SpriteEffects.None, 0);

            // shield
            if (_showNordVpn == true)
            {
                spriteBatch.Draw(_nordVpn,
                    new Rectangle(new Point((int)(_monogame.Window.ClientBounds.Width * 0.25), (int)(_monogame.Window.ClientBounds.Height * 0.45)),
                    new Point((int)(_monogame.Window.ClientBounds.Width * 0.225), (int)(_monogame.Window.ClientBounds.Height * 0.4))), Color.White);
            }

            // enemy's picture, in front of the shield when it attacks, that's why shield is drawn above
            spriteBatch.Draw(enemyTexture, new Rectangle(_enemyLocation,
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.225), (int)(_monogame.Window.ClientBounds.Height * 0.4))),
                null, Color.White, _enemyRotation, new Vector2(enemyTexture.Width/2, enemyTexture.Height/2), SpriteEffects.None, 0);

            if(_heroDice > 0)
            {
                spriteBatch.Draw(_diceTexture[_heroDice-1], new Rectangle(
                    new Point((int)(_monogame.Window.ClientBounds.Width * 0.2125), (int)(_monogame.Window.ClientBounds.Height * 0.25)),
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.1125), (int)(_monogame.Window.ClientBounds.Height * 0.2))),
                null, Color.White, 0, new Vector2(_diceTexture[_heroDice-1].Width / 2, _diceTexture[_heroDice-1].Height / 2), SpriteEffects.None, 0);
            }
            if (_enemyDice > 0)
            {
                spriteBatch.Draw(_diceTexture[_enemyDice - 1], new Rectangle(
                    new Point((int)(_monogame.Window.ClientBounds.Width * 0.7875), (int)(_monogame.Window.ClientBounds.Height * 0.25)),
                new Point((int)(_monogame.Window.ClientBounds.Width * 0.1125), (int)(_monogame.Window.ClientBounds.Height * 0.2))),
                null, Color.White, 0, new Vector2(_diceTexture[_enemyDice-1].Width / 2, _diceTexture[_enemyDice-1].Height / 2), SpriteEffects.None, 0);
            }

            spriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Attack button's event. The player throws the dice and the turn continues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _attackButton_Click(object sender, System.EventArgs e)
        {
            // attack action, dice and call damage function
            _enemyAttackString = "";
            hideButtons();
            _heroAttack = _rand.Next(6) + 1;
            _fightStatus = 3;
            _counter = 0;
            _hasAttacked = true;

        }

        /// <summary>
        /// Defense button's event. Dice's thrown in the defenseCheck function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _defenseButton_Click(object sender, System.EventArgs e)
        {
            // defense action, dice, then check if bigger than enemy's attack, then if enemy's attack bigger call damage function
            _enemyAttackString = "";
            hideButtons();
            _counter = 0;
            _fightStatus = 3;
            _hasAttacked = false;
        }

        /// <summary>
        /// Display the rules when help is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _rulesButton_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("À chaque tour, vous et votre adversaire lancez un dé à six faces\n" +
                "L'adversaire indique s’il effectue une grosse attaque(de 4 à 6)" + 
                " ou une petite attaque(de 1 à 3)\n\nSi vous décidez d'attaquer, vous lancez votre dé et attaquez avant l'adversaire\n" + 
                "Si vous décidez de vous défendre, vous lancez votre dé, et si vous faites un résultat égal ou supérieur à 4," +
                " vous ne prenez pas de dégâts sur ce tour, et votre adversaire ne pourra pas faire de grosse attaque au tour suivant." + 
                " Si la défense échoue, votre adversaire vous fera des dégâts", "Aide"
);
        }
        /// <summary>
        /// Opponent throws the dice
        /// </summary>
        private void opponentAttack()
        {
            int nbResultMax = 6;
            if(_defenseSuccess == true)
            {
                nbResultMax = 3;
            }
            _enemyAttack = _rand.Next(nbResultMax) + 1;
        }

        /// <summary>
        /// Set both player's damage and makes a cool animation
        /// </summary>
        private void damageSetter()
        {
            _counter++;
            if (_counter == 50)
            {
                _damageResumeString = "Vous avez fait " + _heroAttack + " dégâts";
                _heroDice = _heroAttack;
            }
            else if (_counter >= 75 && _counter < 85)
            {
                _heroLocation.X += _moveWidth;
                _heroLocation.Y -= _moveHeight;

                _monogame.Content.Load<SoundEffect>("music/clang").CreateInstance().Play();
            }
            else if (_counter >= 85 && _counter < 95)
            {
                _heroLocation.X -= _moveWidth;
                _heroLocation.Y += _moveHeight;
            }
            else if (_counter >= 95 && _counter < 125)
            {
                _enemyRotation += (float)Math.PI / 15;
            }

            else if (_counter == 125)
            {
                _enemyRotation = 0f;
                _enemyHp -= _heroAttack;
                if (_enemyHp <= 0)
                {
                    _enemyHp = 0;
                    _hasWon = true;
                    _counter = 0;
                    _fightStatus = -1;

                }
            }
            else if (_counter == 200)
            {
                _damageResumeString = "L'adversaire a fait " + _enemyAttack + " dégâts";
                _enemyDice = _enemyAttack;
            }
            else if (_counter >= 225 && _counter < 235)
            {
                _enemyLocation.X -= _moveWidth;
                _enemyLocation.Y += _moveHeight;

                _monogame.Content.Load<SoundEffect>("music/clang").CreateInstance().Play();
            }
            else if (_counter >= 235 && _counter < 245)
            {
                _enemyLocation.X += _moveWidth;
                _enemyLocation.Y -= _moveHeight;
            }
            else if(_counter >= 245 && _counter < 275)
            {
                if(_enemyAttack > 0)
                {
                    _heroRotation += (float)Math.PI / 15;
                }
            }
            else if(_counter == 275)
            {
                _heroHp -= _enemyAttack;
                if (_heroHp <= 0)
                {
                    _heroHp = 0;
                    _counter = 0;
                    _fightStatus = -1;
                }
            }
            else if (_counter == 350)
            {
                _showNordVpn = false;
                _fightStatus = 1;
            }
        }

        // Private method(s)

        /// <summary>
        /// Throws defense dice and check if defense is high enough
        /// </summary>
        private void defenseCheck()
        {
            _counter++;
            if(_counter == 50)
            {
                int playerDefense = _rand.Next(6) + 1;
                _heroDice = playerDefense;
                if(playerDefense >= 4) // if dice's result is bigger or equal than 4, then no damage taken
                {
                    _damageResumeString = "Défense réussie !";
                    _enemyAttack = 0;
                    _defenseSuccess = true;
                }
                else
                {
                    _damageResumeString = "Défense ratée...";
                }
                if (_enemyAttack > 0)
                {
                    // wait for more time
                }
            }
            if(_counter == 180)
            {
                _showNordVpn = true;
                _hasAttacked = true;
            }
        }

        /// <summary>
        /// Hide buttons to not be used
        /// </summary>
        private void hideButtons()
        {
            _attackButton.Position = new Vector2(-_monogame.Window.ClientBounds.Width, - _monogame.Window.ClientBounds.Height);
            _defenseButton.Position = new Vector2(-_monogame.Window.ClientBounds.Width, - _monogame.Window.ClientBounds.Height);
        }
    }
}
