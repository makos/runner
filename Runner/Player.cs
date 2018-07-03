using System;
using System.Collections.Generic;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace Runner
{
    class Player
    {
        // Finite state machines, yay!
        enum State
        {
            Run, Jump, Duck
        }

        public Sprite sprite;
        public RectangleShape collider;

        // I really don't like having so many variables
        private RenderWindow window;
        private Texture spriteSheet;
        private float YVelocity = 0f;
        private float gravity = 15f;
        private float jumpImpulse = -320f; // jump "force", has to be negative because 0,0 is top-left
        private State state;
        private Level level;
        private Clock animationClock; // switch animation frames based on this clock
        private float animationSpeed = .1f; // how quickly to switch animation frames
        private List<IntRect> runFrames;
        private IntRect jumpFrame;
        private List<IntRect> duckFrames;
        
        public Player(RenderWindow window, Level level, ref Texture spriteSheet, ref Dictionary<String, IntRect> spriteSheetDict, Vector2f position)
        {
            this.level = level;
            this.spriteSheet = spriteSheet;
            this.window = window;

            // Load animation coordinates
            runFrames = new List<IntRect> { spriteSheetDict["TREX_RUN1"], spriteSheetDict["TREX_RUN2"] };
            duckFrames = new List<IntRect> { spriteSheetDict["TREX_DUCK1"], spriteSheetDict["TREX_DUCK2"] };
            jumpFrame = spriteSheetDict["TREX_JUMP"];

            sprite = new Sprite(this.spriteSheet, runFrames[0]);
            sprite.Position = position;

            collider = new RectangleShape(new Vector2f(sprite.TextureRect.Width - 15, sprite.TextureRect.Height - 4));
            collider.Position = new Vector2f(sprite.Position.X, sprite.Position.Y);

            state = State.Run;
            animationClock = new Clock();
        }

        public void Update(float deltaTime)
        {
            YVelocity = YVelocity + gravity;

            if (IsColliding(deltaTime, level.collider.GetGlobalBounds()))
            {
                YVelocity = 0f;

                if (state != State.Duck)
                    state = State.Run;
                else
                    state = State.Duck;
            }

            // Check if player hits an obstacle
            foreach (Obstacle obstacle in level.obstacles)
            {
                if (IsColliding(deltaTime, obstacle.sprite.GetGlobalBounds()))
                    Console.WriteLine("hit");
            }

            switch(state)
            {
                case State.Run:
                    Animate();

                    if (Keyboard.IsKeyPressed(Keyboard.Key.Up) || Keyboard.IsKeyPressed(Keyboard.Key.Space))
                    {
                        YVelocity = YVelocity + jumpImpulse;
                        state = State.Jump;
                    }
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        sprite.TextureRect = duckFrames[0];
                        sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y + 17);
                        state = State.Duck;
                    }
                    break;

                case State.Jump:
                    Animate();
                    break;

                case State.Duck:
                    Animate();

                    if (!Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y - 17);
                        sprite.TextureRect = runFrames[0];
                        state = State.Run;
                    }
                    break;
            }

            sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y + YVelocity * deltaTime * 1.25f);
            collider.Position = new Vector2f(collider.Position.X, sprite.Position.Y);
        }

        void Animate()
        {
            switch (state)
            {
                case State.Run:
                    if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
                    {
                        animationClock.Restart();
                        if (sprite.TextureRect == runFrames[0])
                            sprite.TextureRect = runFrames[1];
                        else
                            sprite.TextureRect = runFrames[0];
                    }
                    break;

                case State.Jump:
                    sprite.TextureRect = jumpFrame;
                    break;

                case State.Duck:
                    if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
                    {
                        animationClock.Restart();
                        if (sprite.TextureRect == duckFrames[0])
                            sprite.TextureRect = duckFrames[1];
                        else
                            sprite.TextureRect = duckFrames[0];
                    }
                    break;
            }
        }

        bool IsColliding (float deltaTime, FloatRect other)
        {
            // Naively check collisions; we don't need anything fancy (I hope)
            Vector2f oldPosition = collider.Position;
            collider.Position = new Vector2f(collider.Position.X, collider.Position.Y + YVelocity * deltaTime);
            FloatRect bounds = collider.GetGlobalBounds();

            if (bounds.Intersects(other))
            {
                collider.Position = oldPosition;
                return true;
            }

            collider.Position = oldPosition;
            return false;
        }
    }
}
