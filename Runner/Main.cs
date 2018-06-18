using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Runner
{
    static class Runner
    {
        public static bool debug = true;

        static void Main()
        {
            RenderWindow window = new RenderWindow(new VideoMode(1200, 400), "Run!", Styles.Close|Styles.Titlebar);
            GameManager manager = new GameManager(window, "sprite\\sheet.png");
            
        }
    }

    public interface IEntity
    {
        void Update(float deltaTime);
        Sprite Sprite
        {
            get;
            set;
        }
    }
}
