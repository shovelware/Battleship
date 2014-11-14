using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Ship
    {
        #region MVars

        int length;
        public int Length { get { return length; } }

        Vector2 position;
        public Vector2 Position { get { return position; } set { position = value; } }

        bool placed;
        public bool Placed { get { return placed; } set { placed = value; } }

        bool sunk;
        public bool Sunk { get { return sunk; } }

        int[] status;
        const int OKAY = 0, HURT = 1, SUNK = 2;

        bool[] visible;

        bool horizontal;
        public bool Horizontal { get { return horizontal; } set { horizontal = value; } }

        Player owner;
        Texture2D good, hurt;

        public Vector2[] CoveredSquares { get { return GetCoveredSquares(); } }

        int squareSize, squareRim;


        #endregion

        #region Make

        public Ship(int length, int squareSize, int squareRim, Texture2D good, Texture2D hurt, Player owner)
        {
            this.length = length;
            status = new int[length];
            visible = new bool[length];
            this.squareSize = squareSize;
            this.squareRim = squareRim;
            this.good = good;
            this.hurt = hurt;
            this.owner = owner;

            if (owner.Human)
            {
                for (int i = 0; i < length; i++)
                {
                    visible[i] = true;
                }
            }
        }

        #endregion

        #region Battleship

        private Vector2[] GetCoveredSquares()
        {
            Vector2[] squarespos = new Vector2[length];
            squarespos[0] = position;
            Vector2 addVector = new Vector2(0, 1);

            if (horizontal)
                addVector = new Vector2(1, 0);

            for (int i = 1; i < length; i++)
            {
                squarespos[i] = position + addVector * i;
            }

            return squarespos; 
        }

        public void Hit(int location)
        {
            status[location] = HURT;
            visible[location] = true;
        }

        public void Sink()
        {
            for (int i = 0; i < length; i++)
                {
                    status[i] = SUNK;
                }

            sunk = true;
        }

        #endregion

        #region U&D

        public void Update(GameTime gameTime)
        {
            int sink = 0;
            foreach (byte b in status)
            {
                if (b != OKAY)
                {
                    sink++;
                }
            }

            if (sink >= length)
                Sink();
        }

        public void Draw(SpriteBatch sb, Vector2 offset, GameTime gameTime)
        {
            if (placed)
            {
                bool drawBG = true;
                for (int i = 0; i < length; i++)
                {
                    if (visible[i] == false)
                        drawBG = false;
                }

                if (drawBG)
                {
                    //Connecting line
                    if (horizontal)
                        sb.Draw(good, new Rectangle(
                            (int)(offset.X + squareRim + (position.X - 1) * (squareSize + squareRim)),
                            (int)(offset.Y + squareRim + (position.Y - 1) * (squareSize + squareRim)) + squareSize / 4,
                            (squareSize + squareRim) * length - squareRim,
                            squareSize / 2), Color.Lerp(owner.Color, Color.DimGray, 0.5f));

                    else
                        sb.Draw(good, new Rectangle(
                            (int)(offset.X + squareRim + (position.X - 1) * (squareSize + squareRim)) + squareSize / 4,
                            (int)(offset.Y + squareRim + (position.Y - 1) * (squareSize + squareRim)),
                            squareSize / 2,
                            (squareSize + squareRim) * length - squareRim), Color.Lerp(owner.Color, Color.DimGray, 0.5f));
                }

                //Squares
                Vector2 addVector = new Vector2(0, 1);

                if (horizontal)
                    addVector = new Vector2(1, 0);

                for (int i = 0; i < length; i++)
                {
                    if (visible[i] == true)
                    {
                        Rectangle rect = new Rectangle((int)(offset.X + squareRim + (position.X - 1 + (addVector.X * i)) * (squareSize + squareRim)), (int)(offset.Y + squareRim + (position.Y - 1 + (addVector.Y * i)) * (squareSize + squareRim)), squareSize, squareSize);

                        if (status[i] == OKAY)
                            sb.Draw(good, rect, owner.Color);
                        if (status[i] == HURT)
                            sb.Draw(hurt, rect, Color.Lerp(owner.Color, Color.DimGray, 0.5f));
                        if (status[i] == SUNK)                                                                                                                                              
                            sb.Draw(hurt, rect, Color.Lerp(owner.Color, Color.Black, 0.75f));
                    }
                }

            }
        }

        public void DrawMini(SpriteBatch sb, GameTime gameTime, bool horizontal, Vector2 position)
        {
            int mini = 1;

            Vector2 addVector = new Vector2(0, 1);

            if (horizontal)
                addVector = new Vector2(1, 0);

            for (int i = 0; i < length; i++)
            {
                sb.Draw(good, new Rectangle((int)(position.X + ((addVector.X * i)) * (squareSize / mini + squareRim / mini)), (int)(position.Y + ((addVector.Y * i)) * (squareSize / mini + squareRim / mini)), squareSize / mini,squareSize / mini), Color.Lerp(owner.Color, Color.White, 0.5f));
            }
        }

        #endregion
    }
}
