using System;
using System.Xml;
using System.Collections.Generic;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace Runner
{
    class GameManager
    {
        enum State
        {
            Menu, Play
        }

        //TODO: add obstacles & score
        public int score = 0;
        // I feel like something can be done about all those variables, I just don't know what...
        private Texture[] playerRunTextures = new Texture[6]; // 6 run sprites in the spritesheet
        private Texture[] playerDuckTextures = new Texture[2];
        private RenderWindow window;
        private Image spriteSheet;
        private Dictionary<String, IntRect> spriteSheetDict; // dictionary to read coordinates of sprites in spritesheet from xml file
        private Player player;
        private Clock clock; // FPS clock
        private Level level;
        private State state;

        public GameManager(RenderWindow window, string spriteSheetFp)
        {
            this.window = window;
            spriteSheet = new Image(spriteSheetFp);
            window.SetFramerateLimit(60);
            // Register events
            window.Closed += new EventHandler(OnClose);
            window.KeyPressed += OnKeyPressed;

            LoadSpriteCoords();

            Texture sprites = new Texture(spriteSheetFp);

            level = new Level(this, ref sprites, ref spriteSheetDict);
            player = new Player(window, level, ref sprites, ref spriteSheetDict, new Vector2f(100f, 300f));

            clock = new Clock();

            state = State.Play;
            //state = State.Menu;

            GameLoop();
        }

        void GameLoop ()
        {
            while (window.IsOpen)
            {
                float deltaTime = clock.Restart().AsSeconds();
                window.DispatchEvents();
                switch (state)
                {
                    case State.Menu:
                        DrawMenu();
                        break;
                    case State.Play:
                        window.Clear(Color.White);
                        Update(deltaTime);
                        Draw();
                        if (Runner.debug)
                            DrawDebugStuff();
                        break;
                }
                window.Display();
            }
        }

        void OnClose(object sender, EventArgs e)
        {
            window.Close();
        }

        void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                window.Close();
        }

        void Update(float deltaTime)
        {
            player.Update(deltaTime);
            level.Update(deltaTime);
        }

        void DrawMenu()
        {

        }

        void Draw()
        {
            switch (state)
            {
                case State.Menu:
                    
                    break;
                case State.Play:
                    window.Draw(level.tileOne);
                    window.Draw(level.tileTwo);
                    foreach (Obstacle obstacle in level.obstacles)
                        window.Draw(obstacle.sprite);
                    window.Draw(player.sprite);
                    break;
            }

        }

        void DrawDebugStuff()
        {
            RectangleShape pRect = new RectangleShape(player.collider.Size);
            pRect.Position = player.collider.Position;
            pRect.FillColor = Color.Transparent;
            pRect.OutlineColor = Color.Magenta;
            pRect.OutlineThickness = 1f;
            window.Draw(pRect);
            window.Draw(level.collider);
            foreach (Obstacle obstacle in level.obstacles)
            {
                obstacle.collider.Position = obstacle.sprite.Position;
                window.Draw(obstacle.collider);
            }
        }

        void LoadSpriteCoords()
        {
            XmlDocument spriteXml = new XmlDocument();
            spriteXml.Load("sprite\\sheet.xml");

            XmlNodeList sprites = spriteXml.SelectNodes("//sheet/sprite");
            spriteSheetDict = new Dictionary<string, IntRect>();

            foreach (XmlNode node in sprites)
            {
                int x = 0;
                int y = 0;
                int width = 0;
                int height = 0;
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name == "x")
                        x = int.Parse(attr.Value);
                    else if (attr.Name == "y")
                        y = int.Parse(attr.Value);
                    else if (attr.Name == "width")
                        width = int.Parse(attr.Value);
                    else if (attr.Name == "height")
                        height = int.Parse(attr.Value);
                }

                spriteSheetDict.Add(node.InnerText, new IntRect(x, y, width, height));
            }
        }
    }
}
