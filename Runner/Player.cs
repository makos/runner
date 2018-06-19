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

        // These correspond to indices of textures in textureSheet
        enum Anim
        {
            Idle = 0, Run = 2, Hit = 4, Duck = 6
        }

        public Sprite sprite;

        // I really don't like having so many variables
        private Texture[] textureSheet;
        private float YVelocity = 0f;
        private float gravity = 10f;
        private float jumpImpulse = -300f; // jump "force", has to be negative because 0,0 is top-left
        private State state;
        private Level level;
        private Clock animationClock; // switch animation frames based on this clock
        private float animationSpeed = .15f; // how quickly to switch animation frames
        private int currentAnimFrame = (int)Anim.Idle; // current frame of animation

        public Player(Level level, ref Texture[] textureSheet, Vector2f position)
        {
            this.level = level;
            this.textureSheet = textureSheet;
            sprite = new Sprite(this.textureSheet[(int)Anim.Idle]);
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
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    {
                        state = State.Jump;
                        YVelocity = YVelocity + jumpImpulse;
                        break;
                    }
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        state = State.Duck;
                        currentAnimFrame = (int)Anim.Duck;
                        break;
                    }

                    //TODO pack the animation stuff into a method
                    if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
                    {
                        animationClock.Restart();
                        sprite.Texture = textureSheet[currentAnimFrame];
                        currentAnimFrame = currentAnimFrame + 1;
                        if (currentAnimFrame > (int)Anim.Run + 1)
                            currentAnimFrame = (int)Anim.Run;
                    }
                    break;

                case State.Jump:
                    sprite.Texture = textureSheet[(int)Anim.Idle];
                    break;

                case State.Duck:
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
                        {
                            animationClock.Restart();
                            sprite.Texture = textureSheet[currentAnimFrame];
                            currentAnimFrame = currentAnimFrame + 1;
                            if (currentAnimFrame > (int)Anim.Duck + 1)
                                currentAnimFrame = (int)Anim.Duck;
                        }
                    }
                    else
                    {
                        state = State.Run;
                        currentAnimFrame = (int)Anim.Run;
                        break;
                    } 
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
