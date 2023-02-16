using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EternalKingdoom
{
    class Card : Component
    {
        // Private attribute(s)

        private bool _isCardMoving = false;
        private bool _isLeft = false;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private SpriteFont _font;

        private Texture2D _texture;

        private MonoGame _monogame;

        private Vector2 strSize;
        private Vector2 txtPos;
        private Vector2 txtNamePos;

        private Rectangle picturePos;

        private SoundEffect sound;

        private List<Scenario> _scenario = null;    // list of all game card -> Scenario object

        // Public attribute(s)

        public Scenario _activeScenario = null;

        public static bool NoMoreCards = false;     // variable used to end the game when all scenario have been deleted from the List<Scenario>

        public int PopularityGauge { get; set; }
        public int DefenseGauge { get; set; }
        public int MoneyGauge { get; set; }
        public int PopulationGauge { get; set; }

        public static int YearsLived { get; set; }
        public Color PenColour { get; set; }

        public Rectangle RectangleCardPosition
        {
            get
            {
                return new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.35), (int)(_monogame.Window.ClientBounds.Height * 0.2), (int)(_monogame.Window.ClientBounds.Width * 0.3), (int)(_monogame.Window.ClientBounds.Height * 0.75));
            }
        }

        // Public constructor(s)

        public Card()
        {

        }

        public Card(Texture2D texture, SpriteFont font,  MonoGame monogame)
        {
            _texture = texture;

            _font = font;

            PenColour = Color.Black;

            _monogame = monogame;

            sound = _monogame.Content.Load<SoundEffect>("music/Card-flip-sound-effect");

            // Load Scenario with json
            string jsonString = File.ReadAllText("Content/json/card.json");

            List<JsonCard> jsonCards = JsonSerializer.Deserialize<List<JsonCard>>(jsonString);
            _scenario = new List<Scenario>();
            foreach (JsonCard card in jsonCards)
            {
                Scenario scenario = new Scenario();
                scenario.Id = card.Id;
                scenario.Name = card.Name;
                scenario.Text = _monogame.BreakWordString(card.Text, _font, (int)(_monogame.Window.ClientBounds.Width * 0.2));
                scenario.Choices = card.Choices;
                scenario.FirstChoice = card.firstChoice;
                scenario.SecondChoice = card.secondChoice;
                scenario.Picture = _monogame.Content.Load<Texture2D>(card.Picture);
                scenario.cardType = card.cardType;
                _scenario.Add(scenario);
            }
            
            _activeScenario = _scenario[new Random().Next(0, _scenario.Count)];

            strSize = _font.MeasureString(_activeScenario.Text.ToString());
            txtPos = new Vector2((int)((_monogame.Window.ClientBounds.Width) * 0.4), (int)(_monogame.Window.ClientBounds.Height * 0.62));
            txtNamePos = new Vector2((int)((_monogame.Window.ClientBounds.Width) * 0.37), (int)(_monogame.Window.ClientBounds.Height * 0.23 + (_monogame.Window.ClientBounds.Height * 0.35)));
            picturePos = new Rectangle((int)(_monogame.Window.ClientBounds.Width  * 0.43), (int)(_monogame.Window.ClientBounds.Height * 0.23), (int)(_monogame.Window.ClientBounds.Width * 0.17), (int)(_monogame.Window.ClientBounds.Height * 0.35));

            _scenario.Remove(_activeScenario);  // remove scenario from List<Scenario> when it has been played once

        }

        // Public virtual method(s)

        /// <summary>
        /// Check the state of the current scenario, then the position of the card and the choices made to link the minigames, the gauges impact or the next card drawn.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (_activeScenario != null)
            {
                _previousMouseState = _currentMouseState;
                _currentMouseState = Mouse.GetState();

                Rectangle mouseRectangle = new Rectangle(_currentMouseState.X, _currentMouseState.Y, 1, 1);

                _isCardMoving = true;


                if (mouseRectangle.Intersects(RectangleCardPosition))
                {
                    _isCardMoving = false;
                }
                else
                {
                    if (mouseRectangle.Y >= RectangleCardPosition.Y && mouseRectangle.Y <= RectangleCardPosition.Y + RectangleCardPosition.Height)
                    {
                        if (mouseRectangle.X <= (_monogame.Window.ClientBounds.Width * 0.5))
                        {
                            _isLeft = true;     // player choose the left action on previous card

                            if (_scenario.Count != 0)   // if there's some scenario left in the List<Scenario> -> play the next card
                            {
                                if (_currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed)
                                {
                                    YearsLived++;
                                    sound.Play(volume: 1f, pitch: 0.0f, pan: 0.0f);
                                    switch (_activeScenario.cardType) // switch for special card that activate mini-games
                                    {
                                        case 0:
                                            break;
                                        case 1:
                                            _monogame.fightFinished = false;
                                            MediaPlayer.Play(_monogame.Content.Load<Song>("music/fight_Battle"));
                                            MediaPlayer.IsRepeating = true;
                                            _monogame.ActiveScreen = MonoGame.ScreenType.Combat;
                                            break;
                                        case 2:
                                            _monogame.riddleFinished = false;
                                            MediaPlayer.Play(_monogame.Content.Load<Song>("music/riddle_Fireside-Tales-MP3"));
                                            MediaPlayer.IsRepeating = true;
                                            _monogame.ActiveScreen = MonoGame.ScreenType.Riddle;
                                            break;
                                        case 3:
                                            _monogame.shifumiFinished = false;
                                            MediaPlayer.Play(_monogame.Content.Load<Song>("music/shifumi_CHIPTUNE_Minstrel_Dance"));
                                            MediaPlayer.IsRepeating = true;
                                            _monogame.ActiveScreen = MonoGame.ScreenType.Shifumi;
                                            break;
                                        default:
                                            break;
                                    }
                                    gaugesImpact(_activeScenario.FirstChoice);
                                    _activeScenario = _scenario[new Random().Next(0, _scenario.Count)];
                                    _scenario.Remove(_activeScenario);
                                }
                            } else
                            {
                                NoMoreCards = true;     // if there's no more cards you lose
                                sound.Play(volume: 1f, pitch: 0.0f, pan: 0.0f);

                                YearsLived++;
                            }

                        }
                        else
                        {
                            _isLeft = false;    // player choose the right action on previous card

                            if (_scenario.Count != 0)
                            {
                                if (_currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed)
                                {
                                    YearsLived++;
                                    sound.Play(volume: 1f, pitch: 0.0f, pan: 0.0f);
                                    gaugesImpact(_activeScenario.SecondChoice);
                                    _activeScenario = _scenario[new Random().Next(0, _scenario.Count)];
                                    _scenario.Remove(_activeScenario);
                                }
                            } 
                            else
                            {
                                NoMoreCards = true;
                            }

                        }
                    }
                    else
                    {
                        _isCardMoving = false;
                    }

                }

                if (_monogame.fightFinished)
                {
                    if (_monogame.fightWon)
                    {
                        gaugesImpact(_activeScenario.FirstChoice);

                        _monogame.fightWon = false;
                        _monogame.fightFinished = false;
                    }
                    else
                    {
                        gaugesImpact(_activeScenario.SecondChoice);

                        _monogame.fightWon = false;
                        _monogame.fightFinished = false;
                    }
                    _activeScenario = _scenario[new Random().Next(0, _scenario.Count)];
                }


                if (_monogame.riddleFinished)
                {
                    if (_monogame.riddleWon)
                    {
                        gaugesImpact(_activeScenario.FirstChoice);

                        _monogame.riddleWon = false;
                        _monogame.riddleFinished = false;
                    }
                    else
                    {
                        gaugesImpact(_activeScenario.SecondChoice);

                        _monogame.riddleWon = false;
                        _monogame.riddleFinished = false;
                    }

                    _activeScenario = _scenario[new Random().Next(0, _scenario.Count)];
                }

                if (_monogame.shifumiFinished)
                {
                    if (_monogame.shifumiWon)
                    {
                        gaugesImpact(_activeScenario.FirstChoice);

                        _monogame.shifumiWon = false;
                        _monogame.shifumiFinished = false;
                    }
                    else
                    {
                        gaugesImpact(_activeScenario.SecondChoice);

                        _monogame.shifumiWon = false;
                        _monogame.shifumiFinished = false;
                    }
                    _activeScenario = _scenario[new Random().Next(0, _scenario.Count)];
                }
            }
        }

        /// <summary>
        /// Check the position of the card and the choices made to draw correctly the gauges and the cards
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_isCardMoving)
            {
                if (_isLeft)
                {
                    spriteBatch.Draw(_texture, 
                        new Rectangle((int)(RectangleCardPosition.X + _monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.2), 
                        (int)(_monogame.Window.ClientBounds.Width * 0.3), (int)(_monogame.Window.ClientBounds.Height * 0.75)), Color.White);
                    spriteBatch.DrawString(_font, _activeScenario.Text, new Vector2((int)(txtPos.X + _monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.62)), Color.Black);
                    spriteBatch.DrawString(_font, _activeScenario.Name, new Vector2((int)(txtNamePos.X + _monogame.Window.ClientBounds.Width * 0.1), (int)(int)(_monogame.Window.ClientBounds.Height * 0.23 + (_monogame.Window.ClientBounds.Height * 0.35))), Color.DarkSlateGray);
                    spriteBatch.Draw(_activeScenario.Picture, new Rectangle((int)(picturePos.X + _monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.23), (int)(_monogame.Window.ClientBounds.Width * 0.17), (int)(_monogame.Window.ClientBounds.Height * 0.35)), Color.White);
                    CombatScreen.enemyTexture = _activeScenario.Picture;
                    spriteBatch.DrawString(_font, _monogame.BreakWordString(_activeScenario.Choices[0], _font, (int)(_monogame.Window.ClientBounds.Width * 0.25)), new Vector2((int)((_monogame.Window.ClientBounds.Width) * 0.15), (int)((_monogame.Window.ClientBounds.Height - strSize.X) * 0.7)), Color.White);

                    for(int i = 0; i < 4; i++)
                    {
                        Rectangle arrowsLocation =
                            new Rectangle((int)(_monogame.Window.ClientBounds.Width * (0.35 + 0.08 * i)), (int)(_monogame.Window.ClientBounds.Height * 0.07),
                                (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05));

                        if (_activeScenario.FirstChoice[i] > 0 && _activeScenario.FirstChoice[i] <= 2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/smallArrowGauge"),
                                arrowsLocation, Color.White);

                        }
                        else if (_activeScenario.FirstChoice[i] > 2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/bigArrowGauge"),
                                arrowsLocation, Color.White);
                        }
                        else if (_activeScenario.FirstChoice[i] < 0 && _activeScenario.FirstChoice[i] >= -2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/smallArrowDownGauge"),
                                arrowsLocation, Color.White);

                        }
                        else if (_activeScenario.FirstChoice[i] < 0 && _activeScenario.FirstChoice[i] < -2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/bigArrowDownGauge"),
                                arrowsLocation, Color.White);
                        }
                    }
                }
                else
                {
                    spriteBatch.Draw(_texture, new Rectangle((int)(RectangleCardPosition.X - _monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.2), (int)(_monogame.Window.ClientBounds.Width * 0.3), (int)(_monogame.Window.ClientBounds.Height * 0.75)), Color.White);
                    spriteBatch.DrawString(_font, _activeScenario.Text, new Vector2((int)(txtPos.X - _monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.62)), Color.Black);
                    spriteBatch.DrawString(_font, _activeScenario.Name, new Vector2((int)(txtNamePos.X -_monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.23 + (_monogame.Window.ClientBounds.Height * 0.35))), Color.DarkSlateGray);
                    spriteBatch.Draw(_activeScenario.Picture, new Rectangle((int)(picturePos.X - _monogame.Window.ClientBounds.Width * 0.1), (int)(_monogame.Window.ClientBounds.Height * 0.23), (int)(_monogame.Window.ClientBounds.Width * 0.17), (int)(_monogame.Window.ClientBounds.Height * 0.35)), Color.White);

                    spriteBatch.DrawString(_font, _monogame.BreakWordString(_activeScenario.Choices[1], _font, (int)(_monogame.Window.ClientBounds.Width * 0.25)), new Vector2((int)((_monogame.Window.ClientBounds.Width) * 0.6), (int)((_monogame.Window.ClientBounds.Height - strSize.X) * 0.7)), Color.White);

                    for (int i = 0; i < 4; i++)
                    {
                        Rectangle arrowsLocation =
                            new Rectangle((int)(_monogame.Window.ClientBounds.Width * (0.35 + 0.08 * i)), (int)(_monogame.Window.ClientBounds.Height * 0.07),
                                (int)(_monogame.Window.ClientBounds.Width * 0.05), (int)(_monogame.Window.ClientBounds.Height * 0.05));

                        if (_activeScenario.SecondChoice[i] > 0 && _activeScenario.SecondChoice[i] <= 2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/smallArrowGauge"),
                                arrowsLocation, Color.White);

                        }
                        else if (_activeScenario.SecondChoice[i] > 2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/bigArrowGauge"),
                                arrowsLocation, Color.White);
                        }
                        else if (_activeScenario.SecondChoice[i] < 0 && _activeScenario.SecondChoice[i] >= -2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/smallArrowDownGauge"),
                                arrowsLocation, Color.White);

                        }
                        else if (_activeScenario.SecondChoice[i] < 0 && _activeScenario.SecondChoice[i] < -2)
                        {
                            spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/bigArrowDownGauge"),
                                arrowsLocation, Color.White);
                        }
                    }
                }
            }
            else
            {
                spriteBatch.Draw(_texture, RectangleCardPosition, Color.White);
                spriteBatch.Draw(_activeScenario.Picture, picturePos, Color.White);

                spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/RightArrow"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.67), (int)(_monogame.Window.ClientBounds.Height * 0.5), (int)(_monogame.Window.ClientBounds.Width * 0.03), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.White);
                spriteBatch.Draw(_monogame.Content.Load<Texture2D>("maingame/LeftArrow"), new Rectangle((int)(_monogame.Window.ClientBounds.Width * 0.3), (int)(_monogame.Window.ClientBounds.Height * 0.5), (int)(_monogame.Window.ClientBounds.Width * 0.03), (int)(_monogame.Window.ClientBounds.Height * 0.05)), Color.White);

                spriteBatch.DrawString(_font, _activeScenario.Name, txtNamePos, Color.DarkSlateGray);
                spriteBatch.DrawString(_font, _activeScenario.Text, txtPos, Color.Black);
            }
        }

        // Private method(s)

        /// <summary>
        /// Changes the values of the gauges regarding the list of impacts sent by the action of the previous card
        /// </summary>
        /// <param name="GaugeImpacted"></param>
        private void gaugesImpact(List<int> GaugeImpacted)
        {
            if (PopularityGauge + GaugeImpacted[0] < 32)
            {
                PopularityGauge += GaugeImpacted[0];
            }
            else
            {
                PopularityGauge = 32;
            }

            if (MoneyGauge + GaugeImpacted[1] < 32)
            {

                MoneyGauge += GaugeImpacted[1];
            }
            else
            {
                MoneyGauge = 32;
            }

            if (DefenseGauge + GaugeImpacted[2] < 32)
            {

                DefenseGauge += GaugeImpacted[2];
            }
            else
            {
                DefenseGauge = 32;
            }

            if (PopulationGauge + GaugeImpacted[3] < 32)
            {

                PopulationGauge += GaugeImpacted[3];
            }
            else
            {
                PopulationGauge = 32;
            }
        }
    }
}
