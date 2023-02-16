using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EternalKingdoom
{
    public class Screen
    {
        // Protected attribute(s)

        protected List<Component> _components;
        protected MonoGame _monogame;

        // Public constructor(s)

        public Screen(MonoGame monogame)
        {
            _monogame = monogame;
            Initialize();
        }

        // Public virtual method(s)

        public virtual void Initialize()
        {
            _components = new List<Component>();
            LoadContent();
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Component component in _components)
                component.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Component component in _components)
                component.Draw(gameTime, spriteBatch);
        }

        protected virtual void LoadContent()
        {
        }
    }
}
