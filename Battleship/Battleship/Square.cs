using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Square
    {
        #region MVars

        Vector2 boardPos;
        Vector2 position;
        public Vector2 Position { get { return position; } }
        bool hit;
        public bool Hit {get { return hit; } }
        bool open = true;
        public bool Open { get { return open; } set { open = value; } }

        Texture2D square;

        int squareSize, squareRim;

        Ship ship;
        public Ship Ship { get { return ship; } }
        int shipIndex;

        #endregion
        
        #region Make

        public Square(Vector2 position, int squareSize, int squareRim, Texture2D square)
        {
            this.position = position;
            this.squareSize = squareSize;
            this.squareRim = squareRim;
            this.square = square;
        }

        #endregion

        #region Battleship

        public void PlaceShip(Ship placed, int index)
        {
            ship = placed;
            shipIndex = index;
            open = false;
        }

        public bool HitSquare()
        {
            if (!hit)
            {
                hit = true;
                if (!open)
                {
                    Ship.Hit(shipIndex);
                    return true;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region U&D

        public void Update(Vector2 boardpos, GameTime gameTime)
        {
            boardPos = boardpos;
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            if (!hit)
                sb.Draw(square, new Rectangle((int)(boardPos.X + (position.X * (squareSize + squareRim) + squareRim)), (int)(boardPos.Y + (position.Y * (squareSize + squareRim) + squareRim)), squareSize, squareSize), Color.CornflowerBlue);

            else if (hit)
                sb.Draw(square, new Rectangle((int)(boardPos.X + (position.X * (squareSize + squareRim) + squareRim)), (int)(boardPos.Y + (position.Y * (squareSize + squareRim) + squareRim)), squareSize, squareSize), Color.DodgerBlue);
        }

        #endregion
    }
}