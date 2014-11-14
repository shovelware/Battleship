using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleship
{
    class Player
    {
        #region MVars
        
        Board gameBoard;
        public Ship[] ships;
        int shipsI;
        public Ship NextShip { get { return ships[shipsI]; } set { ships[shipsI] = value; } }
        bool shipsPlaced = false;
        public bool ShipsPlaced { get { return shipsPlaced; } }

        string name;
        public string Name { get { return name; } set { name = value; } }
        Color col;
        public Color Color { get { return col; } set { col = value; } }

        bool turnTaken;
        public bool TurnTaken { get { return turnTaken; } set { turnTaken = value; } }

        bool human;
        public bool Human { get { return human; } }

        Texture2D good, hurt;

        #endregion

        #region Make
        
        public Player(string playerName, bool control, Color playerCol, Board gameBoard, Texture2D good, Texture2D hurt)
        {
            name = playerName;
            human = control;
            col = playerCol;
            this.gameBoard = gameBoard;
            this.good = good;
            this.hurt = hurt;

            //This is the player's fleet, changing these variables will change the ships
            CreateFleet(new int[5] { 5, 4, 3, 3, 2 });
        }

        #endregion

        #region Battleship

        public bool PlaceShip(Square square, bool horizontal)
        {
            if (shipsI < ships.Length)
            {
                if (horizontal)
                {
                    NextShip.Position = square.Position + new Vector2(1, 1);
                    NextShip.Horizontal = horizontal;
                    NextShip.Placed = true;
                    square.PlaceShip(NextShip, 0);

                    shipsI++;
                    return true;
                }

                else
                {
                    NextShip.Position = square.Position + new Vector2(1, 1);
                    NextShip.Horizontal = horizontal;
                    NextShip.Placed = true;
                    square.PlaceShip(NextShip, 0);

                    shipsI++;
                    return true;
                }
            }
            return false;
        }

        public bool AllShipsSunk()
        {
            bool sunk = true;
            foreach (Ship s in ships)
            {
                if (!s.Sunk)
                    sunk = false;
            }
            return sunk;
        }

        private void CreateFleet(int[] lengths)
        {
            ships = new Ship[lengths.Length];

            for (int i = 0; i < lengths.Length; i++)
            {
                ships[i] = new Ship(lengths[i], gameBoard.squareSize, gameBoard.squareRim, good, hurt, this);
            }
        }

        #endregion

        #region Customisation

        public void ChangeName(string newName)
        {
            name = newName;
        }

        public void ChangeColour(Color newCol)
        {
            col = newCol;
        }

        #endregion

        #region U&D

        public void Update(GameTime gameTime)
        {
            foreach (Ship s in ships)
                s.Update(gameTime);

            if (shipsI >= ships.Length)
                shipsPlaced = true;
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            foreach (Ship s in ships)
                s.Draw(sb, gameBoard.Position, gameTime);
        }

        #endregion
    }
}
