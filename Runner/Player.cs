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
        enum State
        {
            Run, Jump
        }

        public Sprite sprite;

        private Sprite[] spriteSheet;
        private float YVelocity = 0f;
        private float gravity = 10f;
        private float jumpImpulse = -300f;
        private State state;
        private Level level;
        private Clock absClock;
        private Clock animationClock;
        private int currentAnimIndex = 0;

        public Player(Level level, Sprite[] spriteSheet, Vector2f position)
        {
            this.level = level;
            this.spriteSheet = spriteSheet;
            sprite = this.spriteSheet[currentAnimIndex];
            sprite.Position = position;
            
            state = State.Run;
            animationClock = new Clock();
        }

        public void Update (float deltaTime)
        {
            
            YVelocity = YVelocity + gravity;

            if (IsColliding(deltaTime, level.collider.GetGlobalBounds()))
            {
                YVelocity = 0f;
                state = State.Run;
            }
            else
            {
                sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y + YVelocity * deltaTime);
            }

            switch (state)
            {
                case State.Run:
                    if (animationClock.ElapsedTime.AsSeconds() > .25f)
                    {
                        animationClock.Restart();
                        currentAnimIndex++;
                        sprite.Texture = spriteSheet[currentAnimIndex % 2].Texture;
                    }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    {
                        state = State.Jump;
                        YVelocity = YVelocity + jumpImpulse;
                    }
                    break;
                case State.Jump:
                    //sprite.Texture = spriteSheet[0].Texture;
                    break;
            }
        }

        bool IsColliding (float deltaTime, FloatRect collider)
        {
            
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
