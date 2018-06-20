using System;
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

        // I really don't like having so many variables
        private RenderWindow window;
        private Texture spriteSheet;
        private IntRect currentFrame;
        private Vector2i idleFrames = new Vector2i(848, 2);
        private Vector2i runFrames = new Vector2i(936, 2);
        private Vector2i playerRunSize = new Vector2i(44, 47);
        private Vector2i playerDuckSize = new Vector2i(59, 30);
        private float YVelocity = 0f;
        private float gravity = 10f;
        private float jumpImpulse = -100f; // jump "force", has to be negative because 0,0 is top-left
        private State state;
        private Level level;
        private Clock animationClock; // switch animation frames based on this clock
        private float animationSpeed = .25f; // how quickly to switch animation frames
        private Clock jumpTime;
        private float jumpKeyPressedAt;

        public Player(RenderWindow window, Level level, ref Texture spriteSheet, Vector2f position)
        {
            this.level = level;
            this.spriteSheet = spriteSheet;
            this.window = window;
            currentFrame = new IntRect(idleFrames, playerRunSize); // First sprite on the sprite sheet
            sprite = new Sprite(this.spriteSheet, currentFrame);
            sprite.Position = position;
            
            state = State.Run;
            animationClock = new Clock();
            jumpTime = new Clock();
        }

        public void Update (float deltaTime)
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
            else
            {
                sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y + YVelocity * deltaTime);
            }

            switch (state)
            {
                case State.Run:
                    //THIS WORKS!
                    //just move the IntRect back and forth over the loaded texture
                    if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
                    {
                        animationClock.Restart();
                        currentFrame = new IntRect(new Vector2i(currentFrame.Left + playerRunSize.X, runFrames.Y), playerRunSize);
                        if (currentFrame.Left == runFrames.X + (playerRunSize.X * 2))
                        {
                            currentFrame = new IntRect(runFrames, playerRunSize);
                        }
                        sprite.TextureRect = currentFrame;
                    }
                    //TODO: add command patterns for key presses for variable jump height
                    // or maybe do it with timers? i dunno
                    break;
                case State.Duck:
                    break;
                case State.Jump:
                    YVelocity = YVelocity + jumpImpulse;
                    if (jumpTime.ElapsedTime.AsSeconds() < jumpKeyPressedAt + 1f)
                    {
                        YVelocity = YVelocity + 50;
                    }
                    break;
            }
        }

        public void KeyPressed(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Code);
            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    window.Close();
                    break;
                case Keyboard.Key.Up:
                    jumpKeyPressedAt = jumpTime.ElapsedTime.AsSeconds();
                    state = State.Jump;
                    break;
            }
        }

        public void KeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Up:
                    state = State.Run;
                    break;
            }
        }

        bool IsColliding (float deltaTime, FloatRect collider)
        {
            // Naively check collisions; we don't need anything fancy (I hope)
            Vector2f oldPosition = sprite.Position;
            sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y + YVelocity * deltaTime);
            FloatRect bounds = sprite.GetGlobalBounds();

            if (bounds.Intersects(collider))
            {
                sprite.Position = oldPosition;
                return true;
            }

            sprite.Position = oldPosition;
            return false;
        }
    }
}
