using SFML.Graphics;
using SFML.System;

namespace Runner
{
    class Level
    {
        public Sprite tileOne;
        public Sprite tileTwo;
        public float scrollSpeed = -100f;
        public RectangleShape collider;
        public Sprite sprite;

        public Level(Texture texture)
        {
            sprite = new Sprite(texture);

            // Fancy schmancy VS2017 intellisense hinted to do this instead of "normal" initialization on separate lines
            //it feels weird for me
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
            // TODO debug stuff should really be nicely packed in a method or something;
            // this applies to all classes with debug drawables, not only here
            collider.FillColor = Color.Transparent;
            collider.OutlineColor = Color.Magenta;
            collider.OutlineThickness = 1f;
        }

        public void Update(float deltaTime)
        {
            // Scroll the background infinitely, swap the position of tiles when they move out of screen
            tileOne.Position = new Vector2f(tileOne.Position.X + scrollSpeed * deltaTime, 360);
            tileTwo.Position = new Vector2f(tileTwo.Position.X + scrollSpeed * deltaTime, 360);

            if (tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width < 0)
                tileOne.Position = new Vector2f(tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width, 360);
            if (tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width < 0)
                tileTwo.Position = new Vector2f(tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width, 360);
        }
    }
}
