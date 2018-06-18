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
        public List<IEntity> entities = new List<IEntity>();


        private Sprite[] playerSprites = new Sprite[2];
        private RenderWindow window;
        private Image spriteSheet;
        private Dictionary<String, Vector2i> spriteSheetDict;
        private Player player;
        private Vector2i playerSize = new Vector2i(44, 47);
        private Clock clock;
        private Clock speedClock;
        private Level level;

        public GameManager(RenderWindow window, string spriteSheetFp)
        {
            this.window = window;
            spriteSheet = new Image(spriteSheetFp);

            window.SetFramerateLimit(60);
            // Register events
            window.Closed += new EventHandler(OnClose);
            window.KeyPressed += OnKeyPress;

            LoadSpriteCoords();

            for (int i = 0; i < 2; i++)
            {
                playerSprites[i] = new Sprite(new Texture(spriteSheet,
                                   new IntRect(new Vector2i(spriteSheetDict["TREX"].X + (playerSize.X * i), spriteSheetDict["TREX"].Y),
                                   playerSize)));
            }
            Console.WriteLine(playerSprites.Length);
            level = new Level(new Texture(spriteSheet, new IntRect(spriteSheetDict["GROUND"], new Vector2i(1200, 14))));
            player = new Player(level, playerSprites, new Vector2f(100f, 300f));
            
            clock = new Clock();
            speedClock = new Clock();

            GameLoop();
        }

        void GameLoop ()
        {
            while (window.IsOpen)
            {
                float deltaTime = clock.Restart().AsSeconds();
                window.DispatchEvents();
                window.Clear(Color.White);
                Update(deltaTime);
                Draw();
                if (Runner.debug)
                    DrawDebugStuff();
                window.Display();
            }
        }

        void OnClose(object sender, EventArgs e)
        {
            window.Close();
        }

        void OnKeyPress(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    window.Close();
                    break;
            }
        }

        void Update(float deltaTime)
        {
            player.Update(deltaTime);
            level.Update(deltaTime);
        }

        void Draw()
        {
            window.Draw(level.tileOne);
            window.Draw(level.tileTwo);
            window.Draw(player.sprite);
        }

        void DrawDebugStuff()
        {
            RectangleShape pRect = new RectangleShape((Vector2f)player.sprite.Texture.Size);
            pRect.Position = player.sprite.Position;
            pRect.FillColor = Color.Transparent;
            pRect.OutlineColor = Color.Magenta;
            pRect.OutlineThickness = 1f;
            window.Draw(pRect);
            window.Draw(level.collider);
        }

        void LoadSpriteCoords()
        {
            XmlDocument spriteXml = new XmlDocument();
            spriteXml.Load("sprite\\sheet.xml");

            XmlNodeList sprites = spriteXml.SelectNodes("//sheet/sprite");
            spriteSheetDict = new Dictionary<string, Vector2i>();

            foreach (XmlNode node in sprites)
            {
                int x = 0;
                int y = 0;
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name == "x")
                        x = int.Parse(attr.Value);
                    else if (attr.Name == "y")
                        y = int.Parse(attr.Value);
                }

                spriteSheetDict.Add(node.InnerText, new Vector2i(x, y));
            }
        }
    }
}
