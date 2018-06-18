using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Runner
{
    class Level
    {
        //public RectangleShape collider;
        public Sprite tileOne;
        public Sprite tileTwo;
        public float scrollSpeed = -100f;
        public RectangleShape collider;

        public Sprite sprite;
        //private RectangleShape newCollider;
        //private Sprite newSprite;

        public Level(Texture texture)
        {
            sprite = new Sprite(texture);

            tileOne = new Sprite(texture)
            {
                Position = new Vector2f(0, 360)
            };
            tileTwo = new Sprite(texture)
            {
                Position = new Vector2f(1200, 360)
            };

            collider = new RectangleShape(new Vector2f(1200, 10))
            {
                Position = new Vector2f(0, 380)
            };
            collider.FillColor = Color.Transparent;
            collider.OutlineColor = Color.Magenta;
            collider.OutlineThickness = 1f;
        }

        public void Update(float deltaTime)
        {
            tileOne.Position = new Vector2f(tileOne.Position.X + scrollSpeed * deltaTime, 360);
            tileTwo.Position = new Vector2f(tileTwo.Position.X + scrollSpeed * deltaTime, 360);

            if (tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width < 0)
                tileOne.Position = new Vector2f(tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width, 360);
            if (tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width < 0)
                tileTwo.Position = new Vector2f(tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width, 360);
        }
    }
}
