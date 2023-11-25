using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace airhockey
{
    enum RockType
    {
        Player,
        Enemy,
        Neutral
    }
    class Racket : Puck
    {
        public static Texture2D[] textures;
        public int texId = 0;
        public RockType type;
        public Racket()
        {
            type = RockType.Neutral;
        }
        override public void Move()
        {
            base.Move(); 
            switch (type)
            {
                case RockType.Neutral:
                    {

                        break;
                    }
                case RockType.Player:
                    {
                        if (y < Game1.viewY / 5)
                        {
                            y = Game1.viewY / 5;
                        }
                        break;
                    }
                case RockType.Enemy:
                    {
                        if (y > Game1.viewY / 2)
                        {
                            y = Game1.viewY / 2;
                        }
                        break;
                    }
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            tx = textures[texId];
            base.Draw(sb);
        }
        public virtual void AI_Move(Puck p)
        {
            float e_power = 1f;
            float brake = 0.9f;
            if (x < p.x) vx = vx + e_power;
            if (x > p.x) vx = vx - e_power;
            if (y < 100) vy = vy + e_power;
            if (y > 300) vy = vy - e_power;
            vx = vx * brake;
            vy = vy * brake;

        }
    }
}

