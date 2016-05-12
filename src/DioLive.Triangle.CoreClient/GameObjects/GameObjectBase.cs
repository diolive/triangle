using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DioLive.Triangle.CoreClient.GameObjects
{
    public abstract class GameObjectBase
    {
        public Rectangle Bounds { get; protected set; }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}