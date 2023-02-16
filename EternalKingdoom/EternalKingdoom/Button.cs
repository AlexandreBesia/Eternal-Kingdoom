using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EternalKingdoom
{
    class Button : Component
    {
        // Private attribute(s)

        private bool _isHovering;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private SpriteFont _font;

        // Public attribute(s)

        public Texture2D _texture;

        private int _rectWidth, _rectHeight;

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }

        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _rectWidth, _rectHeight);
            }
        }

        public string Text { get; set; }

        public Button(Texture2D texture, SpriteFont font, int w, int h)
        {
            _texture = texture;

            _font = font;

            PenColour = Color.White;

            _rectWidth = w;
            _rectHeight = h;
        }

        // Public virtual method(s)

        /// <summary>
        /// Called by Monogame update, update button's logic
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            _isHovering = false;           
            if (Rectangle.Contains(Mouse.GetState().Position))
            {
                _isHovering = true;

                // Call click evnet at the click
                if (_currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed && Click != null)
                    Click(this, new EventArgs());
            }
        }

        /// <summary>
        /// Called by Monogame draw, draw button's graphics
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color colour = Color.White;

            if (_isHovering)
                colour = Color.Gray;

            spriteBatch.Draw(_texture, Rectangle, colour);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (int)(_font.MeasureString(Text).Y / 1.4);

                (Vector2, Color)[] textPasses = { (new Vector2(-1.5f, -1.5f), Color.Black), (new Vector2(1.5f, -1.5f), Color.Black), (new Vector2(-1.5f, 1.5f), Color.Black), (new Vector2(1.5f, 1.5f), Color.Black), (new Vector2(), PenColour) };
                foreach ((Vector2, Color) pass in textPasses)
                    spriteBatch.DrawString(_font, Text, new Vector2(x, y) + pass.Item1, pass.Item2);
            }
        }
    }
}
