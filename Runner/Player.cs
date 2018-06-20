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

        // These correspond to indices of textures in textureSheet
        enum Anim
        {
            Idle = 0, Duck = 0, Run = 2, Hit = 4
        }

        public Sprite sprite;

        // I really don't like having so many variables
        private Texture[] runTextures;
        private Texture[] duckTextures;
        private Texture spriteSheet;
        private IntRect currentFrame;
        private Vector2i firstFrame = new Vector2i(848, 2);
        private Vector2i playerRunSize = new Vector2i(44, 47);
        private Vector2i playerDuckSize = new Vector2i(59, 30);
        private float YVelocity = 0f;
        private float gravity = 10f;
        private float jumpImpulse = -300f; // jump "force", has to be negative because 0,0 is top-left
        private State state;
        private Level level;
        private Clock animationClock; // switch animation frames based on this clock
        private float animationSpeed = .15f; // how quickly to switch animation frames
        private int currentAnimFrame = (int)Anim.Idle; // current frame of animation

        public Player(Level level, ref Texture spriteSheet, Vector2f position)
        {
            this.level = level;
            this.spriteSheet = spriteSheet;
            currentFrame = new IntRect(firstFrame, playerRunSize); // First sprite on the sprite sheet
            //runTextures = textureSheet;
            //duckTextures = duckSprites;
            sprite = new Sprite(this.spriteSheet, currentFrame);
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
                        sprite.TextureRect = new IntRect(0, 0, 59, 30);
                        currentAnimFrame = (int)Anim.Duck;
                        sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y);
                        break;
                    }

                    //TODO pack the animation stuff into a method
                    // why is this so hard
                    // idea: have one texture, and adjust TextureRect (an IntRect) accordingly;
                    if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
                    {
                        animationClock.Restart();
                        currentFrame = new IntRect(new Vector2i(firstFrame.X + playerRunSize.X, firstFrame.Y), playerRunSize);

                        if (currentFrame.Left == firstFrame.X + (playerRunSize.X * 2))
                            currentFrame = new IntRect(firstFrame, playerRunSize);
                        //sprite.TextureRect = new IntRect(new Vector2i(firstFrame.X + playerRunSize.X, firstFrame.Y), playerRunSize);
                        sprite.TextureRect = currentFrame;
                        Console.WriteLine(currentFrame);
                        //if (sprite.TextureRect.Left == firstFrame.X + playerRunSize.X)
                        //    sprite.TextureRect = new IntRect(firstFrame, playerRunSize);
                    }
                    break;

                case State.Jump:
                    sprite.Texture = runTextures[(int)Anim.Idle];
                    break;

                case State.Duck:
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        if (animationClock.ElapsedTime.AsSeconds() > animationSpeed)
                        {
                            animationClock.Restart();
                            sprite.Texture = duckTextures[currentAnimFrame];
                            
                            currentAnimFrame = currentAnimFrame + 1;
                            if (currentAnimFrame > (int)Anim.Duck + 1)
                                currentAnimFrame = (int)Anim.Duck;
                        }
                    }
                    else
                    {
                        state = State.Run;
                        currentAnimFrame = (int)Anim.Run;
                        sprite.Position = new Vector2f(sprite.Position.X, sprite.Position.Y - 17);
                        sprite.TextureRect = new IntRect(0, 0, 44, 47);
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
