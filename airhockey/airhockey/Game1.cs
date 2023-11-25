using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace airhockey
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 


    public class Puck
    {
        public float x, y, vx, vy, size;
        public float radius;
        public Texture2D tx;
        public static Rectangle BounceRect;
        public float brake;
        virtual public void Move()
        {
            x = x + vx;
            y = y + vy;
            vx = vx * brake;
            vy = vy * brake;
            if (x < BounceRect.Left)
            {
                x = BounceRect.Left;
                vx = -vx;
            }
            if (x + size > BounceRect.Left + BounceRect.Width)
            {
                x = BounceRect.Left + BounceRect.Width - size;
                vx = -vx;
            }
            if (y + size > BounceRect.Top + BounceRect.Height)
            {
                y = BounceRect.Top + BounceRect.Height - size;
                vy = -vy;
            }
            if (y < BounceRect.Top)
            {
                y = BounceRect.Top;
                vy = -vy;
            }
        }
        virtual public bool Collide(Puck p)
        {
            double xc = x + size / 2;
            double yc = y + size / 2;
            double rc = radius;
            double xc2 = p.x + p.size / 2;
            double yc2 = p.y + p.size / 2;
            double rc2 = p.radius;
            double xr = Math.Abs(xc2 - xc);
            double yr = Math.Abs(yc2 - yc);
            double Rc = Math.Sqrt(xr * xr + yr * yr);
            if (Rc < rc + rc2)
            {
                double c = 10; //Регулятор силы отскока
                double D = Rc - (rc + rc2);
                double xr_s = xc2 - xc;
                double yr_s = (yc2 - yc);
                double ex = xr_s / Rc;
                double ey = yr_s / Rc;
                double Fx = ex * c * D;
                double Fy = ey * c * D;
                double Fx2 = -Fx;
                double Fy2 = -Fy;
                vx = (float)(vx + Fx / rc);
                vy = (float)(vy + Fy / rc);
                p.vx = (float)(p.vx + Fx2 / rc2);
                p.vy = (float)(p.vy + Fy2 / rc2);
                return true;
            }

            return false;

        }
        virtual public bool isInRect(Rectangle r)
        {
            double rc = radius;
            double xc = x + size / 2;
            double yc = y + size / 2;
            if ((xc - rc > r.Left) && (yc - rc > r.Top) && (xc + rc < r.Left + r.Width) && (yc + rc < r.Top + r.Height))
            {
                return true;
            }
            return false;
        }
        virtual public void Draw(SpriteBatch sb)
        {
            sb.Draw(tx, new Rectangle((int)x, (int)y, (int)size, (int)size), Color.White);
        }

    }
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D fon;
        Texture2D tex;
        public static int viewX;
        public static int viewY;
        Racket r0;
        Rectangle hitRect;
        static List<Puck> lst = new List<Puck>();
        public Game1()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this); // для увеличения экрана
            graphics.PreferredBackBufferWidth =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fon = Content.Load<Texture2D>("tableair1");
            tex = Content.Load<Texture2D>("puck");
            Racket.textures = new Texture2D[2];
            Racket.textures[0] = Content.Load<Texture2D>("racket");
            Racket.textures[1] = Content.Load<Texture2D>("racket1");
            viewX = Window.ClientBounds.Width;
            viewY = Window.ClientBounds.Height;
            // Puck.brake = 0.98f;
            Puck p = new Puck();
            p.tx = tex;
            p.size = 150;
            p.radius = 100;
            p.x = viewX / 2 - p.size / 2;
            p.y = viewY / 2 - p.size / 2;
            p.vx = 15f;
            p.vy = 15f;
            p.brake = 0.995f; 
            Puck.BounceRect = new Rectangle(viewX * 25 / 100, viewY * 5 / 100, viewX * 50 / 100, viewY * 90 / 100);
            r0 = new Racket() { x = 500, y = 500, vx = 0.5f, vy = 0.5f, size = 250, radius = 150, brake = 0.999f};
            r0.type = RockType.Enemy;
            Racket r1 = new Racket() { x = 150, y = 100, vy = 0.5f, vx = 0.5f, size = 250, texId = 1, radius = 150, brake = 0.999f };
            r1.type = RockType.Player;
            lst.Add(r1);
            lst.Add(r0);
            lst.Add(p);
            hitRect = new Rectangle(0, viewY / 3, viewX, viewY / 3);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                // Нажата левая кнопка мыши
                // Берем координаты
                int x = currentMouseState.Position.X; // Координаты мышки
                int y = currentMouseState.Position.Y;
                lst[0].x = x; //Запихиваем в обьект
                lst[0].y = y;
            }
            /* foreach (Puck p in lst)
             {
                 if (p is Racket)
                 {
                     Racket ri = (Racket)p;
                     ri.AI_Move(lst[2]);
                 }
             }*/
            r0.AI_Move(lst[2]);


            // TODO: Add your update logic here
            foreach (Puck p in lst)
            {
                p.Move();
                if (p.isInRect(hitRect) && p as Racket == null)
                {
                    p.x = 0;
                    p.y = 0;

                }
                foreach (Puck d in lst)
                {
                    if (d != p)
                    { p.Collide(d); }

                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(fon, new Rectangle(0, 0, viewX, viewY), Color.White);
            foreach (Puck p in lst)
            {
                p.Draw(spriteBatch);
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
