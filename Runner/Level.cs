using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Runner
{
    class Level
    {
        public Sprite tileOne;
        public Sprite tileTwo;
        public float scrollSpeed = -100f;
        public RectangleShape collider;
        //public Sprite sprite;
        public List<Obstacle> obstacles = new List<Obstacle>();

        private Texture spriteSheet;
        private Clock levelClock = new Clock();
        private List<IntRect> cactusStd;
        private List<IntRect> cactusSmall;
        private IntRect cactusTriple;

        public Level(ref Texture spriteSheet, ref Dictionary<String, IntRect> spriteSheetDict)
        {
            this.spriteSheet = spriteSheet;
            //sprite = new Sprite(spriteSheet, spriteSheetDict["GROUND"]);

            cactusStd = new List<IntRect> { spriteSheetDict["CACTUS1"], spriteSheetDict["CACTUS2"],
                                            spriteSheetDict["CACTUS3"], spriteSheetDict["CACTUS4"] };
            cactusSmall = new List<IntRect> { spriteSheetDict["CACTUS_SMALL1"], spriteSheetDict["CACTUS_SMALL2"], spriteSheetDict["CACTUS_SMALL3"],
                                              spriteSheetDict["CACTUS_SMALL4"], spriteSheetDict["CACTUS_SMALL5"], spriteSheetDict["CACTUS_SMALL6"] };
            cactusTriple = spriteSheetDict["CACTUS_TRIPLE"];

            tileOne = new Sprite(spriteSheet, spriteSheetDict["GROUND"]);
            tileOne.Position = new Vector2f(0, 360);
            tileTwo = new Sprite(spriteSheet, spriteSheetDict["GROUND"]);
            tileTwo.Position = new Vector2f(1200, 360);

            collider = new RectangleShape(new Vector2f(1200, 10))
            {
                Position = new Vector2f(0, 380)
            };
            // TODO debug stuff should really be nicely packed in a method or something;
            // this applies to all classes with debug drawables, not only here
            collider.FillColor = Color.Transparent;
            collider.OutlineColor = Color.Magenta;
            collider.OutlineThickness = 1f;

            PopulateLevelOnStart();
        }

        void SpawnRandomObstacle()
        {
            int randint = new Random().Next(0, cactusStd.Count);
            Obstacle testCactus = new Obstacle(this, new Sprite(spriteSheet, cactusStd[randint]), new Vector2f(1230, 380 - cactusStd[randint].Height));
        }

        void PopulateLevelOnStart()
        {
            int randint = new Random().Next(2, 5);
            List<int> points = GetRandomPoints(randint);
            for (int i = 0; i < randint; i++)
            {
                int randIndex = new Random().Next(0, cactusStd.Count);
                Obstacle obstacle = new Obstacle(this, new Sprite(spriteSheet, cactusStd[randIndex]), new Vector2f(points[i], 380 - cactusStd[randIndex].Height));
            }
        }

        List<int> GetRandomPoints(int count)
        {
            Random random = new Random();
            List<int> randomPositions = new List<int>();
            int last = 0;

            for (int i = 0; i < count; i++)
            {
                int temp = random.Next(200, 1200);
                if (temp > last + 30 || temp < last - 30)
                    randomPositions.Add(temp);
                else if (temp + 30 < 1200)
                    randomPositions.Add(temp + 30);
                else
                    randomPositions.Add(temp - 30);
                last = temp;
            }

            return randomPositions;
        }

        public void Update(float deltaTime)
        {
            // Scroll the background infinitely, swap the position of tiles when they move out of screen
            scrollSpeed = scrollSpeed - 1;
            tileOne.Position = new Vector2f(tileOne.Position.X + scrollSpeed * deltaTime, 360);
            tileTwo.Position = new Vector2f(tileTwo.Position.X + scrollSpeed * deltaTime, 360);

            foreach (Obstacle obstacle in obstacles)
            {
                obstacle.sprite.Position = new Vector2f(obstacle.sprite.Position.X + scrollSpeed * deltaTime, 380 - obstacle.sprite.TextureRect.Height);
            }

            if (tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width < 0)
                tileOne.Position = new Vector2f(tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width, 360);
            if (tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width < 0)
                tileTwo.Position = new Vector2f(tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width, 360);

            if (levelClock.ElapsedTime.AsSeconds() > 2f)
            {
                levelClock.Restart();
                SpawnRandomObstacle();
            }
        }
    }
}
