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
        public float scrollSpeed = -300f;
        public RectangleShape collider;
        //public Sprite sprite;
        public List<Obstacle> obstacles = new List<Obstacle>();

        private Texture spriteSheet;
        private Clock scoreClock = new Clock();
        private Clock obstacleClock = new Clock();
        private List<IntRect> cactusStd;
        private List<IntRect> cactusSmall;
        private IntRect cactusTriple;
        private Random random = new Random();
        private GameManager manager;

        public Level(GameManager manager, ref Texture spriteSheet, ref Dictionary<String, IntRect> spriteSheetDict)
        {
            this.spriteSheet = spriteSheet;
            this.manager = manager;
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

            //PopulateLevelOnStart();
        }

        void SpawnRandomObstacle()
        {
            int randint = random.Next(0, 3);
            if (randint == 0)
            {
                int index = random.Next(0, cactusStd.Count);
                new Obstacle(this, new Sprite(spriteSheet, cactusStd[index]), new Vector2f(1230, 380 - cactusStd[index].Height));
            }
            else if (randint == 1)
            {
                int index = random.Next(0, cactusSmall.Count);
                new Obstacle(this, new Sprite(spriteSheet, cactusSmall[index]), new Vector2f(1230, 380 - cactusSmall[index].Height));
            }
            else
            {
                new Obstacle(this, new Sprite(spriteSheet, cactusTriple), new Vector2f(1230, 380 - cactusTriple.Height));
            }
        }

        void PopulateLevelOnStart()
        {
            int randint = random.Next(2, 5);
            List<int> points = GetRandomPoints(randint);
            for (int i = 0; i < randint; i++)
            {
                int randIndex = random.Next(0, cactusStd.Count);
                Obstacle obstacle = new Obstacle(this, new Sprite(spriteSheet, cactusStd[randIndex]), new Vector2f(points[i], 380 - cactusStd[randIndex].Height));
            }
        }

        List<int> GetRandomPoints(int count)
        {
            //Random random = new Random();
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
            //scrollSpeed = scrollSpeed - 1;
            tileOne.Position = new Vector2f(tileOne.Position.X + scrollSpeed * deltaTime, 360);
            tileTwo.Position = new Vector2f(tileTwo.Position.X + scrollSpeed * deltaTime, 360);

            for (int i = 0; i < obstacles.Count; i++)
            {
                obstacles[i].sprite.Position = new Vector2f(obstacles[i].sprite.Position.X + scrollSpeed * deltaTime, 380 - obstacles[i].sprite.TextureRect.Height);
                if (obstacles[i].sprite.Position.X < -50)
                {
                    obstacles.RemoveAt(i);
                }
            }

            if (tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width < 0)
                tileOne.Position = new Vector2f(tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width, 360);
            if (tileTwo.GetGlobalBounds().Left + tileTwo.TextureRect.Width < 0)
                tileTwo.Position = new Vector2f(tileOne.GetGlobalBounds().Left + tileOne.TextureRect.Width, 360);

            if (scoreClock.ElapsedTime.AsSeconds() > .1f)
            {
                scoreClock.Restart();
                manager.score++;
            }

            if (manager.score % 100 == 0)
                scrollSpeed = scrollSpeed - 10;

            if (obstacleClock.ElapsedTime.AsSeconds() > random.Next(1, 4))
            {
                obstacleClock.Restart();
                SpawnRandomObstacle();
            }
        }
    }
}
