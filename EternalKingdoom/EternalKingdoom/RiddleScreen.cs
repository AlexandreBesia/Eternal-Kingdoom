using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Media;

namespace EternalKingdoom
{
    public class RiddleScreen : Screen
    {
        // Private member(s)

        private const float AnimationDuration = 3f;

        private SpriteFont _font;
        private Texture2D _buttonTexture;
        private Texture2D _backgroundTexture;

        private List<Riddle> _riddles;
        private Riddle _activeRiddle;

        private PointLight _pointLight;
        private Color _transitionColor;

        private Button _correctChoice;
        private Button _helpButton;
        private Button _backButton;
        private Vector2 _backButtonStartPos;
        private Vector2 _backButtonEndPos;

        private bool _choiceClicked;
        private float _timeElapsed;

        // Public constructor(s)
        public RiddleScreen(MonoGame monogame) : base(monogame)
        {
        }

        // Public virtual method(s)
        public override void Initialize()
        {
            // Light properties
            _pointLight = new PointLight(_monogame);
            _pointLight.AmbientColor = new Color(0, 0, 0, 200);

            LoadRiddles();

            Reset();

            base.Initialize();
        }

        /// <summary>
        /// Updates the point light's position to follow the mouse and plays the transition once a choice is made.
        /// </summary>
        /// <param name="gameTime">The delta time between the previous frame and the current one</param>
        public override void Update(GameTime gameTime)
        {
            if (_activeRiddle == null)
            {
                Reset();
                CreateChoices();
            }
            else
            {
                if (!_choiceClicked)
                    _pointLight.Position = Mouse.GetState().Position.ToVector2();
                else if ((_timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds) < AnimationDuration)
                {
                    PlayEndAnimation();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the scene objects which are revealed by the point light.
        /// </summary>
        /// <param name="gameTime">The delta time between the previous frame and the current one</param>
        /// <param name="spriteBatch">The batch used to draw the screen objects in one draw call</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_activeRiddle != null)
            {
                _pointLight.OffscreenDraw(spriteBatch);

                spriteBatch.Begin();
                spriteBatch.Draw(_backgroundTexture, new Rectangle(new Point(0, 0), new Point(_monogame.Window.ClientBounds.Width, _monogame.Window.ClientBounds.Height)), new Color(80, 80, 80));

                _activeRiddle.Draw(gameTime, spriteBatch);

                base.Draw(gameTime, spriteBatch);
                _pointLight.Draw(spriteBatch);

                spriteBatch.End();
            }
        }

        // Protected virtual method(s)
        protected override void LoadContent()
        {
            _font = _monogame.Content.Load<SpriteFont>("fonts/Arial24");
            _buttonTexture = _monogame.Content.Load<Texture2D>("button/Button");
            _backgroundTexture = _monogame.Content.Load<Texture2D>("riddle/pergament-papier-alt-vintage");

            // Back button
            string backButtonText = "Retour";
            Vector2 backButtonSize = _font.MeasureString(backButtonText);
            backButtonSize.X *= 2f;
            backButtonSize.Y += 10f;
            _backButtonStartPos = new Vector2(_monogame.Window.ClientBounds.Width, 10f);
            _backButtonEndPos = new Vector2(_backButtonStartPos.X - backButtonSize.X - 10f, _backButtonStartPos.Y);
            _backButton = new Button(_buttonTexture, _font, (int)backButtonSize.X, (int)backButtonSize.Y)
            {
                Position = _backButtonStartPos,
                Text = backButtonText
            };
            _backButton.Click += BackButton_Click;
            _components.Add(_backButton);

            // Help button
            string helpButtonText = "?";
            float helpButtonSize = _font.MeasureString(helpButtonText).X * 2.5f;
            _helpButton = new Button(_monogame.Content.Load<Texture2D>("button/buttonCircle"), _font, (int)helpButtonSize, (int)helpButtonSize)
            {
                Position = new Vector2(_backButtonEndPos.X - helpButtonSize - 50f, _backButtonStartPos.Y),
                Text = helpButtonText,
            };

            MediaPlayer.Play(_monogame.Content.Load<Song>("music/maingame_CHIPTUNE_The_Old_Tower_Inn"));
            MediaPlayer.IsRepeating = true;

            _monogame.ActiveScreen = MonoGame.ScreenType.MainGame;

            _helpButton.Click += HelpButton_Click;
            _components.Add(_helpButton);

            CreateChoices();
        }

        // Private method(s)

        private void PlayEndAnimation()
        {

            // Scale up and center light, transition to different color based on choice
            float factor = _timeElapsed / AnimationDuration;
            _pointLight.Scale = MathHelper.SmoothStep(_pointLight.Scale, 0.006f * _monogame.Window.ClientBounds.Width, factor / 1.55f);
            _pointLight.Position = new Vector2(MathHelper.SmoothStep(_pointLight.Position.X, _monogame.Window.ClientBounds.Width / 2f, factor / 1.55f),
                                               MathHelper.SmoothStep(_pointLight.Position.Y, _monogame.Window.ClientBounds.Height / 2f, factor / 1.55f));
            _pointLight.Color = new Color((byte)MathHelper.SmoothStep(_pointLight.Color.R, _transitionColor.R, factor),
                                          (byte)MathHelper.SmoothStep(_pointLight.Color.G, _transitionColor.G, factor),
                                          (byte)MathHelper.SmoothStep(_pointLight.Color.B, _transitionColor.B, factor));

            // Translate back button into view using an EaseOutCubic animation
            float outCubicFactor = 1f - (float)Math.Pow(1.0 - factor, 3.0);
            _backButton.Position = new Vector2(MathHelper.Lerp(_backButtonStartPos.X, _backButtonEndPos.X, outCubicFactor), _backButtonStartPos.Y);
        }

        private void Reset()
        {
            // General
            if (_riddles.Count == 0)
                LoadRiddles();

            _activeRiddle = _riddles[new Random().Next(0, _riddles.Count)];
            _choiceClicked = false;
            _timeElapsed = 0f;
            if (_backButton != null)
            {
                _backButton.Position = _backButtonStartPos;
                _components.Add(_backButton);
                _components.Add(_helpButton);
            }

            // Light properties
            _pointLight.Scale = 0.0016f * _monogame.Window.ClientBounds.Width;
            _pointLight.Color = new Color(156, 102, 61);
        }

        private void CreateChoices()
        {
            // Calculate buttons' size based on longest choice string
            Vector2 maxRiddleSize = new Vector2(0f, 0f);
            foreach (string choice in _activeRiddle.Choices)
            {
                Vector2 strSize = _font.MeasureString(choice);
                if (strSize.X > maxRiddleSize.X) maxRiddleSize.X = strSize.X;
                if (strSize.Y > maxRiddleSize.Y) maxRiddleSize.Y = strSize.Y;
            }
            maxRiddleSize.X *= 2f;
            maxRiddleSize.Y += 20f;

            Vector2 buttonMargin = new Vector2(maxRiddleSize.X * 0.5f, maxRiddleSize.Y * 0.75f);
            int buttonsPerLine = _activeRiddle.Choices.Count / 2;

            // Container containing all buttons used for calculations
            Vector2 containerSize = new Vector2(maxRiddleSize.X * buttonsPerLine + buttonMargin.X * (buttonsPerLine - 1), maxRiddleSize.Y * buttonsPerLine + buttonMargin.Y * (buttonsPerLine - 1));
            Vector2 containerPos = new Vector2(_monogame.Window.ClientBounds.Width / 2f - containerSize.X / 2f, _monogame.Window.ClientBounds.Height - containerSize.Y - buttonMargin.Y * 2f);

            // Create the buttons
            for (int i = 0; i < _activeRiddle.Choices.Count; i++)
            {
                Vector2 localPos = new Vector2((maxRiddleSize.X + buttonMargin.X) * (i % buttonsPerLine), i < buttonsPerLine ? 0f : maxRiddleSize.Y + buttonMargin.Y);
                Button choice = new Button(_buttonTexture, _font, (int)maxRiddleSize.X, (int)maxRiddleSize.Y)
                {
                    Position = new Vector2(containerPos.X + localPos.X, containerPos.Y + localPos.Y),
                    Text = _activeRiddle.Choices[i]
                };

                // Associate click event to appropriate method
                if (i == _activeRiddle.CorrectIndex)
                {
                    choice.Click += CorrectChoice_Click;
                    _correctChoice = choice;
                }
                else
                {
                    choice.Click += WrongChoice_Click;
                }
                choice.Click += Choice_Click;

                _components.Add(choice);
            }
        }

        private void Choice_Click(object sender, EventArgs e)
        {
            _correctChoice.PenColour = new Color(51, 194, 21);
            _choiceClicked = true;
        }

        private void CorrectChoice_Click(object sender, EventArgs e)
        {
            if (!_choiceClicked)
            {
                _monogame.riddleWon = true;
                _transitionColor = _pointLight.Color;
            }
        }

        private void WrongChoice_Click(object sender, EventArgs e)
        {
            if (!_choiceClicked)
            {
                _monogame.riddleWon = false;
                _transitionColor = new Color(107, 22, 19);
            }
        }

        private void BackButton_Click(object sender, System.EventArgs e)
        {
            _riddles.Remove(_activeRiddle);
            _activeRiddle = null;
            _components = new List<Component>();

            MediaPlayer.Play(_monogame.Content.Load<Song>("music/maingame_CHIPTUNE_The_Old_Tower_Inn"));
            MediaPlayer.IsRepeating = true;

            _monogame.ActiveScreen = MonoGame.ScreenType.MainGame;
        }

        private void HelpButton_Click(object sender, System.EventArgs e)
        {
            DialogResult messageBoxAnswer = MessageBox.Show("Lisez l'énigme et choisissez la réponse qui vous semble correcte.\nLes jauges sont positivement impactées en choisissant la bonne réponse et négativement en choisissant une des mauvaises." +
                "\nLa lumière devient rouge pour montrer qu'une des mauvaises réponses a été choisie.", "Aide", MessageBoxButtons.OK);
        }

        private void LoadRiddles()
        {
            // Load in riddle data
            string jsonString = File.ReadAllText("Content/json/riddles.json");
            List<RiddleData> riddleData = JsonSerializer.Deserialize<List<RiddleData>>(jsonString);

            // Create riddles
            int nbChoices = 4;
            Random random = new Random();
            _riddles = new List<Riddle>(riddleData.Count);
            for (int i = 0; i < riddleData.Count; i++)
            {
                int randomIndex = random.Next(0, nbChoices);
                Riddle riddle = new Riddle(riddleData[i], randomIndex, nbChoices, _pointLight, _monogame);
                _riddles.Add(riddle);
            }

            // Add the incorrect choices
            foreach (Riddle riddle in _riddles)
                riddle.setupIncorrectChoices(_riddles);
        }
    }
}
