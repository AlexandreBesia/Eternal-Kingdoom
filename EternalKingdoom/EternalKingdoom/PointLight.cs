using System;
using System.Collections.Generic;
using System.Text;
using MonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EternalKingdoom
{
    /// <summary>
    /// The class used to imitate a point light by playing on opacity using alpha blending.
    /// </summary>
    public class PointLight
    {
        // Private member(s)

        private MonoGame _monogame;
        private Texture2D _texture;
        private RenderTarget2D _brightness;
        private RenderTarget2D _darkness;
        private BlendState _brightnessBlend;
        private BlendState _darknessBlend;

        // Attributes
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Color AmbientColor { get; set; }
        public float Scale { get; set; }

        // Public constructor(s)
        public PointLight(MonoGame monogame)
        {
            _monogame = monogame;

            Initialize();
        }

        // Public method(s)
        public void OffscreenDraw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);

            // Draw on the brightness render target
            _monogame.GraphicsDevice.SetRenderTarget(_brightness);
            _monogame.GraphicsDevice.Clear(AmbientColor);

            spriteBatch.Begin(blendState: _brightnessBlend);
            spriteBatch.Draw(_texture, Position, null, Color, 0f, origin, Scale, SpriteEffects.None, 0f);
            spriteBatch.End();

            // Draw on the darkness render target
            _monogame.GraphicsDevice.SetRenderTarget(_darkness);
            _monogame.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(blendState: _darknessBlend);
            spriteBatch.Draw(_texture, Position, null, Color.Black * 0.6f, 0f, origin, Scale, SpriteEffects.None, 0f);
            spriteBatch.End();

            // Revert to default render target
            _monogame.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw render targets to backbuffer
            spriteBatch.Draw(_darkness, Vector2.Zero, Color.White * 0.8f);
            spriteBatch.Draw(_brightness, Vector2.Zero, Color.White);
        }

        // Private method(s)
        private void Initialize()
        {
            // Setup default properties
            Position = new Vector2(0f, 0f);
            Color = Color.White;
            AmbientColor = Color.Black;
            Scale = 1f;

            // Setup render targets
            PresentationParameters presParams = _monogame.GraphicsDevice.PresentationParameters;
            _darkness = new RenderTarget2D(_monogame.GraphicsDevice, presParams.BackBufferWidth, presParams.BackBufferHeight);
            _brightness = new RenderTarget2D(_monogame.GraphicsDevice, presParams.BackBufferWidth, presParams.BackBufferHeight);

            // Setup blend states
            _brightnessBlend = new BlendState
            {
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,

                ColorBlendFunction = BlendFunction.Add,
                ColorDestinationBlend = Blend.InverseSourceColor
            };
            _darknessBlend = new BlendState
            {
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One
            };

            LoadContent();
        }

        private void LoadContent()
        {
            _texture = _monogame.Content.Load<Texture2D>("riddle/light");
        }
    }
}
