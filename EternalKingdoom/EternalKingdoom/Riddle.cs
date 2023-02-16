using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EternalKingdoom
{
    public class RiddleData
    {
        public string Text { get; set; }
        public string Answer { get; set; }
    }

    /// <summary>Class representing a single riddle with its text, choices available and impact on the main game's gauges.</summary>
    public class Riddle : Component
    {
        // Private attribute(s)

        private SpriteFont _font;
        private PointLight _pointLight;
        private GameWindow _window;
        private Vector2 _riddleSize;

        // Public attribute(s)

        /// <value>Represents the riddle to solve.</value>
        public string Text { get; }
        /// <value>Contains all possible choices including the correct one.</value>
        public List<string> Choices { get; }
        /// <value>The index of the correct choice in the <c>Choices</c> property.</value>
        public int CorrectIndex { get; }

        // Public constructor(s)

        public Riddle(RiddleData riddleData, int correctIndex, int nbChoices, PointLight pointLight, MonoGame monogame)
        {
            _font = monogame.Content.Load<SpriteFont>("fonts/Arial24");
            _pointLight = pointLight;
            _window = monogame.Window;
            Text = monogame.BreakWordString(riddleData.Text, _font, monogame.Window.ClientBounds.Width * 0.65f);
            CorrectIndex = correctIndex;

            Choices = new List<string>(nbChoices);
            for (int i = 0; i < nbChoices; i++)
                Choices.Add((correctIndex == i) ? riddleData.Answer : "");
        }

        // Public method(s)
        public void setupIncorrectChoices(List<Riddle> riddles)
        {
            // The same incorrect choice can't be added twice and the correct can't be added neither
            HashSet<string> incorrectChoices = new HashSet<string>(Choices.Count);
            incorrectChoices.Add(Choices[CorrectIndex]);

            // Modify the empty strings to incorrect choices
            Random random = new Random();
            for (int i = 0; i < Choices.Count; i++)
            {
                if (Choices[i].Length == 0)
                {
                    // Find a choice that has yet to be added
                    string randomChoice;
                    do
                    {
                        int randomIndex = random.Next(0, riddles.Count);
                        randomChoice = riddles[randomIndex].Choices[riddles[randomIndex].CorrectIndex];
                    } while (!incorrectChoices.Add(randomChoice));

                    Choices[i] = randomChoice;
                }
            }
        }

        // Public virtual method(s)
        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _riddleSize = _font.MeasureString(Text);

            Vector2 pos = new Vector2((_window.ClientBounds.Width - _riddleSize.X) / 2f, _window.ClientBounds.Height / 4f);
            Vector2 origin = new Vector2(0, _riddleSize.Y);

            DrawPerCharacterShadow(pos, spriteBatch);

            (Vector2, Color)[] textPasses = { (new Vector2(-1.5f, -1.5f), Color.Black), (new Vector2(1.5f, -1.5f), Color.Black), (new Vector2(-1.5f, 1.5f), Color.Black), (new Vector2(1.5f, 1.5f), Color.Black), (new Vector2(), Color.White) };
            foreach ((Vector2, Color) pass in textPasses)
                spriteBatch.DrawString(_font, Text, pos + pass.Item1, pass.Item2, 0f, origin, 1f, SpriteEffects.None, 1f);
        }

        // Private method(s)

        private void DrawPerCharacterShadow(Vector2 pos, SpriteBatch spriteBatch)
        {
            string stringOffset = "";
            float yOffset = 0f;
            foreach (char character in Text)
            {
                Vector2 offset = _font.MeasureString(stringOffset);
                Vector2 distance = (pos + new Vector2(offset.X, yOffset)) - _pointLight.Position;
                distance.Normalize();
                float direction = (float)Math.Atan2(distance.Y, distance.X);
                spriteBatch.DrawString(_font, character.ToString(), pos + new Vector2(offset.X, yOffset - _riddleSize.Y) + new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction)) * 5.4f, Color.Black, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
                stringOffset += character;
                if (character == '\n')
                {
                    yOffset += _font.MeasureString("\n").Y / 2f;
                    stringOffset = "";
                }
            }
        }
    }
}
