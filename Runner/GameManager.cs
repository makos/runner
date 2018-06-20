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
        enum GameState
        {
            Menu, Play
        }

        // I feel like something can be done about all those variables, I just don't know what...
        private Texture[] playerRunTextures = new Texture[6]; // 6 run sprites in the spritesheet
        private Texture[] playerDuckTextures = new Texture[2];
        private RenderWindow window;
        private Image spriteSheet;
        private Dictionary<String, Vector2i> spriteSheetDict; // dictionary to read coordinates of sprites in spritesheet from xml file
        private Player player;
        private Vector2i playerSize = new Vector2i(44, 47); // hard-coded player sprite size information
        private Vector2i playerDuckSize = new Vector2i(59, 30);
        private Clock clock; // FPS clock
        private Clock speedClock; // advances the game speed (difficulty)
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

            // Load textures into the array to be passed 
            //for (int i = 0; i < playerRunTextures.Length; i++)
            //{
            //    // TODO
            //    // Because last two (ducking) sprites are different width, we have to check for it
            //    // This doesn't work, thanks google
            //    // Also just make this better
            //    playerRunTextures[i] = new Texture(spriteSheet,
            //                        new IntRect(new Vector2i(spriteSheetDict["TREX"].X + (playerSize.X * i), spriteSheetDict["TREX"].Y),
            //                        playerSize));
            //}

            //int lastTextureUsed = spriteSheetDict["TREX"].X + (playerSize.X * 6);
            //Console.WriteLine(lastTextureUsed + (playerDuckSize.X * 1));

            //for (int i = 0; i < playerDuckTextures.Length; i++)
            //{
            //    playerDuckTextures[i] = new Texture(spriteSheet,
            //                        new IntRect(new Vector2i(lastTextureUsed + (playerDuckSize.X * i), spriteSheetDict["TREX"].Y + 17),
            //                        playerDuckSize));
            //}
            Texture sprites = new Texture("sprite\\sheet.png");

            level = new Level(new Texture(spriteSheet, new IntRect(spriteSheetDict["GROUND"], new Vector2i(1200, 14))));
            player = new Player(level, ref sprites, new Vector2f(100f, 300f));
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
            // TODO
            // Load sprite coordinates from an xml; possibly include more info (width & height of the sprite) in a class instead of just a vector2?
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
