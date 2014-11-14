using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Board
    {
        #region MVars

        public Square[,] squares;
        Vector2 position;
        public Vector2 Position { get { return position; } }
        public readonly int width, height, squareSize, squareRim;
        Player owner;
        Texture2D square;

        public Vector2 TopLeft { get { return (position == Vector2.Zero ? position - new Vector2(squareRim, 0) : position) / (squareSize + squareRim); } }
        public Vector2 BottomRight { get { return (position + new Vector2(width * (squareSize + squareRim), height * (squareSize + squareRim))) / (squareSize + squareRim); } }

        #endregion

        #region Make

        public Board(int width, int height, int squareSize, int squareRim, Vector2 position, Texture2D square)
        {
            this.position = position;
            this.width = width;
            this.height = height;
            this.squareSize = squareSize;
            this.squareRim = squareRim;
            this.square = square;

            squares = new Square[height, width];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    squares[h, w] = new Square(new Vector2(w, h), squareSize, squareRim, square);
                }
            }
        }

        public void RegisterPlayer(Player owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Battleship

        public Square GetSquare(Vector2 position)
        {
            //subtract board position somehow.
            if (position.X > width)
                position.X -= width;

            if (position.Y > height)
                position.Y -= height;

            return squares[(int)position.Y - 1, (int)position.X - 1];
        }

        public Vector2 GetSize()
        {
            return new Vector2(width * (squareSize + squareRim) + squareRim, height * (squareSize + squareRim) + squareRim);
        }
        
        #endregion

        #region U&D

        public void Update(GameTime gameTime)
        {
            foreach (Square s in squares)
            {
                int i = 0;
                if (s.Ship != null)
                foreach (Vector2 v in s.Ship.CoveredSquares)
                {
                    squares[(int)(v.Y - 1), (int)(v.X - 1)].Open = false;
                    squares[(int)(v.Y - 1), (int)(v.X - 1)].PlaceShip(s.Ship, i++);
                }
            }
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Draw(square, new Rectangle((int)position.X, (int)position.Y, (int)GetSize().X, (int)GetSize().Y), Color.Lerp(owner.Color, Color.DimGray, 0.5f));
            foreach (Square s in squares)
            {
                s.Update(position, gameTime);
                s.Draw(sb, gameTime);
            }
        }

        #endregion
    }
}
