using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace Runner
{
    class Obstacle
    {

        //TODO: add collisions
        public Sprite sprite;
        public int score = 1;
        public float scrollSpeedToAdd = 1f;
        public RectangleShape collider;

        //private Level level;

        public Obstacle(Level level, Sprite sprite, Vector2f position)
        {
            this.sprite = sprite;
            //this.level = level;

            this.sprite.Position = position;

            //Debug
            collider = new RectangleShape(new Vector2f(this.sprite.TextureRect.Width, this.sprite.TextureRect.Height));
            collider.Position = this.sprite.Position;
            collider.FillColor = Color.Transparent;
            collider.OutlineColor = Color.Magenta;
            collider.OutlineThickness = 1f;

            level.obstacles.Add(this);
        }
    }
}
