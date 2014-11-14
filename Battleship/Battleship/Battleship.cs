using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Battleship
{
    public class Battleship : Microsoft.Xna.Framework.Game
    {
        #region MVars

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random rng;

        bool pregame, drawMenu, gameOver, horizontal;

        Player[] players;
        private int playersI;
        private Player CurrentPlayer { get { return players[playersI]; } }

        Board[] boards;
        private int boardsI;
        private Board CurrentBoard { get { return boards[boardsI]; } }

        public static Texture2D texSquare, texShipGood, texShipHurt, texSelect;

        MouseState mouseCur, mousePrv;
        KeyboardState keysCur, keysPrv;
        
        //These figures affect drawing size for differing resolutions / screen sizes
        private const int boardSize = 10;
        public static int squareSize = 32, squareRim = 2, boardWidth = boardSize, boardHeight = boardSize;

        public Vector2 CurrentSelected { get { return new Vector2((int)mouseCur.X / (squareSize + squareRim) + 1, (int)mouseCur.Y / (squareSize + squareRim) + 1) ; } }
        public Vector2 MousePos { get { return new Vector2(mouseCur.X, mouseCur.Y); } }        

        #endregion

        #region Make/Break

        public Battleship()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            LoadContent();
            graphics.PreferredBackBufferHeight = ((squareSize + squareRim) * boardHeight) + squareRim;
            graphics.PreferredBackBufferWidth = (((squareSize + squareRim) * boardWidth) + squareRim) * 2;
            graphics.ApplyChanges();

            this.IsMouseVisible = true;

            Reset();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texSquare = Content.Load<Texture2D>("square");
            texShipGood = Content.Load<Texture2D>("shipGood");
            texShipHurt = Content.Load<Texture2D>("shipHurt");
            texSelect = Content.Load<Texture2D>("select");
            
        }

        protected override void UnloadContent()
        {
        }

        public void Reset()
        {
            rng = new Random();
            boards = new Board[2];
            players = new Player[2];

            boards[0] = new Board(boardWidth, boardHeight, squareSize, squareRim, Vector2.Zero, texSquare);
            boards[1] = new Board(boardWidth, boardHeight, squareSize, squareRim, new Vector2(boards[0].GetSize().X, 0), texSquare);

            players[0] = new Player("Human", true, Color.Purple, boards[0], texShipGood, texShipHurt);
            players[1] = new Player("CPU", false, Color.Black, boards[1], texShipGood, texShipHurt);

            boards[0].RegisterPlayer(players[0]);
            boards[1].RegisterPlayer(players[1]);

            playersI = 0;
            boardsI = 0;


            pregame = true;
            drawMenu = true;
            gameOver = false;

            horizontal = false;
        }

        #endregion

        #region Tools

        public bool V2LessThanEqual(Vector2 v1, Vector2 v2)
        {
            if (v1.X <= v2.X && v1.Y <= v2.Y)
                return true;
            else return false;
        }

        public bool V2GreaterThanEqual(Vector2 v1, Vector2 v2)
        {
            if (v1.X >= v2.X && v1.Y >= v2.Y)
                return true;
            else return false;
        }

        public bool V2Within(Vector2 lower, Vector2 within, Vector2 upper)
        {
            if (!V2GreaterThanEqual(within, lower))
                return false;
            if (!V2LessThanEqual(within, upper))
                return false;

            else return true;

        }

        public Vector2 V2GridRandom(int min, int max)
        {
            return new Vector2(rng.Next(min, max + 1), rng.Next(min, max + 1));
        }

        public bool CoinFlip()
        {
            if (rng.Next(0, 501) % 2 == 1)
                return true;
            else return false;
        }

        private int CrementIndex(int index, int collectioncount, bool direction)
        {
            int retindex = index;

            if (direction)
            {
                if (++index <= collectioncount - 1)
                    retindex++;
                else retindex = 0;
            }

            else if (!direction)
            {
                if (--index >= 0)
                    retindex--;
                else retindex = collectioncount - 1;
            }

            return retindex;
        }

        private void FlipBoard()
        {
            boardsI = CrementIndex(boardsI, boards.Length, true);
            CurrentPlayer.TurnTaken = false;
        }

        private void FlipPlayer()
        {
            playersI = CrementIndex(playersI, players.Length, true);
            CurrentPlayer.TurnTaken = false;
        }

        #endregion
         
        #region Battleship

        public bool PlaceShip(Vector2 targetSquare, bool horizontal)
        {
            if (!CurrentPlayer.ShipsPlaced) //Check if there's a ship to be placed in current player
                if (CurrentBoard.GetSquare(targetSquare).Open) //Check selected square for ships
                    if (horizontal) //Check overlapping squares for ships & off the board
                    {
                        if (boardWidth - targetSquare.X >= CurrentPlayer.NextShip.Length - 1)
                        {
                            for (int x = (int)targetSquare.X; x < targetSquare.X + CurrentPlayer.NextShip.Length; x++)
                            {
                                if (!CurrentBoard.GetSquare(new Vector2(x, targetSquare.Y)).Open) return false;
                            }
                            return CurrentPlayer.PlaceShip(CurrentBoard.GetSquare(targetSquare), horizontal);
                        }
                    }

                    else if (!horizontal) //Repeat those overlap/ offboard checks for vertical
                    {
                        if (boardHeight - targetSquare.Y >= CurrentPlayer.NextShip.Length - 1)
                        {
                            for (int y = (int)targetSquare.Y; y < targetSquare.Y + CurrentPlayer.NextShip.Length; y++)
                            {
                                if (!CurrentBoard.GetSquare(new Vector2(targetSquare.X, y)).Open) return false;
                            }
                            return CurrentPlayer.PlaceShip(CurrentBoard.GetSquare(targetSquare), horizontal);
                        }
                    }
            return false;
        }

        public bool Fire(Vector2 targetSquare)
        {
            if (playersI != boardsI) //Check if on the right board for current player
            {
                if (!CurrentBoard.GetSquare(targetSquare).Hit) //check if the square hasn't been hit already
                {
                    return CurrentBoard.GetSquare(targetSquare).HitSquare();
                }
            }
            return false;
        }

        public void ChangeTurn()
        {
            FlipPlayer();
            FlipBoard();
        }

        public bool CheckGameOver()
        {
            foreach (Player p in players)
            {
                if (p.AllShipsSunk())
                    return true;
            }

            return false;
        }

        private void ChangeName(Player p, string newName)
        {
            p.ChangeName(newName);
        }

        private void ChangeColour(Player p, Color newCol)
        {
            p.ChangeColour(newCol);
        }

        public void Shutdown()
        {
            this.Exit();
        }

        #endregion

        #region U&D

        private void DrawMenu()
        {
            spriteBatch.Draw(texSquare, new Rectangle(squareSize, squareSize, boardWidth * 2 * squareSize - squareSize, boardHeight * squareSize - squareSize), Color.Lerp(players[0].Color, new Color(255,255,255,128), .5f));
        }

        private void UpdatePregame()
        { 
                //Place Ship
                if (mouseCur.LeftButton == ButtonState.Released && mousePrv.LeftButton ==  ButtonState.Pressed)
                {
                    if (CurrentPlayer.Human && V2Within(CurrentBoard.TopLeft, CurrentSelected, CurrentBoard.BottomRight))
                        {
                            if (PlaceShip(CurrentSelected, horizontal))
                            {
                                ChangeTurn();
                            }
                        }
                }

                //Horizontal switch
                if (mouseCur.RightButton == ButtonState.Pressed && mousePrv.RightButton == ButtonState.Released)
                {
                    horizontal = !horizontal;
                }

                //Switch to real game when all ships are placed
                if (players[0].ShipsPlaced && players[1].ShipsPlaced)
                {
                    pregame = false;
                    playersI = 0;
                    boardsI = 1;

                    //Player will be current, flip coin to decide the first shot
                    if (CoinFlip())
                    {
                        ChangeTurn();
                    }
                }


                if (!CurrentPlayer.Human)
                {
                    if (PlaceShip(V2GridRandom(1, boardSize), CoinFlip()))
                    {
                        ChangeTurn();
                    }
                }
        }

        private void UpdateGame()
        {
            if (mouseCur.LeftButton == ButtonState.Pressed && mousePrv.LeftButton == ButtonState.Released)
            {
                if (CurrentPlayer.Human && V2Within(CurrentBoard.TopLeft, CurrentSelected, CurrentBoard.BottomRight))
                {
                    if (Fire(CurrentSelected))
                    {   
                        CurrentPlayer.TurnTaken = true;
                        FlipPlayer();
                        FlipBoard();
                    }
                }
            }

            if (!CurrentPlayer.Human && !CurrentPlayer.TurnTaken)
            {
                if (Fire(V2GridRandom(1, boardSize)))
                {
                    CurrentPlayer.TurnTaken = true;
                    FlipPlayer();
                    FlipBoard();
                }
            }

            if (CheckGameOver())
            {
                gameOver = true;
                playersI = 0;
                drawMenu = true;
            }
        }

        private void UpdateMenu()
        {
            if (mouseCur.LeftButton == ButtonState.Released && mousePrv.LeftButton == ButtonState.Pressed)
            {
                string[] names = new string[] { "Anna", "Bob", "Charlie", "Dave", "Ed" };

                ChangeName(CurrentPlayer, names[rng.Next(0, names.Length - 1)]);
            }

            if (mouseCur.RightButton == ButtonState.Pressed && mousePrv.RightButton == ButtonState.Released)
            {
                ChangeColour(CurrentPlayer, new Color(rng.Next(0, 255), rng.Next(0, 255), rng.Next(0, 255)));
            }

        }

        protected override void Update(GameTime gameTime)
        {
            keysPrv = keysCur;
            mousePrv = mouseCur;
            keysCur = Keyboard.GetState();
            mouseCur = Mouse.GetState();

            if (keysCur.IsKeyDown(Keys.Escape)) Shutdown();

            //Pregame ship placement 
            if (pregame && !drawMenu)
            {
                UpdatePregame();
            }

            //Game loop
            if (!gameOver && !pregame && !drawMenu)
            {
                UpdateGame();
            }

            //Menu
            if (drawMenu)
            {
                UpdateMenu();
            }

            if ((keysCur.IsKeyDown(Keys.R) && keysPrv.IsKeyUp(Keys.R)) || (keysCur.IsKeyDown(Keys.F5) && keysPrv.IsKeyUp(Keys.F5)))
            {
                Reset();
            }


            if (keysCur.IsKeyDown(Keys.M) && keysPrv.IsKeyUp(Keys.M))
            {
                drawMenu = !drawMenu;
            }

            //TESTING HERE
            if (keysCur.IsKeyDown(Keys.Space) && keysPrv.IsKeyUp(Keys.Space))
            {
            }

            boards[0].Update(gameTime);
            boards[1].Update(gameTime);
            players[0].Update(gameTime);
            players[1].Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aquamarine);

            spriteBatch.Begin();

            //Draw boards
            boards[0].Draw(spriteBatch, gameTime);
            boards[1].Draw(spriteBatch, gameTime);

            //Draw ships with player
            players[0].Draw(spriteBatch, gameTime);
            players[1].Draw(spriteBatch, gameTime);

            if (!drawMenu)
            {
                //Draw selection box
                if (mouseCur.X < boards[0].GetSize().X - squareRim)
                    spriteBatch.Draw(texSelect, new Rectangle((int)(squareRim + (mouseCur.X / (squareSize + squareRim)) * (squareSize + squareRim)), (int)(squareRim + (mouseCur.Y / (squareSize + squareRim)) * (squareSize + squareRim)), squareSize, squareSize), Color.Orange);
                else
                    spriteBatch.Draw(texSelect, new Rectangle((int)(squareRim * 2 + (mouseCur.X / (squareSize + squareRim)) * (squareSize + squareRim)), (int)(squareRim + (mouseCur.Y / (squareSize + squareRim)) * (squareSize + squareRim)), squareSize, squareSize), Color.Orange);

                //Draw next placed ship
                if (CurrentPlayer.Human && !CurrentPlayer.ShipsPlaced)
                {
                    Vector2 drawPos = new Vector2(mouseCur.X, mouseCur.Y);
                    drawPos -= new Vector2(squareSize / 2, squareSize / 2);
                    CurrentPlayer.NextShip.DrawMini(spriteBatch, gameTime, horizontal, drawPos);
                }
            }

            //Draw menu
            else if (drawMenu)
                DrawMenu();

            spriteBatch.End();

            if (drawMenu)
            {
                this.Window.Title = "M: Menu | R: Reset | Left Click: Name | Right Click: Color || Player 0: " + players[0].Name + " | Player 1: " + players[1].Name + "";
            }

            else if (pregame)
            {
                this.Window.Title = "M: Menu | R: Reset | Left Click: Place | Right Click: Rotate";
            }

            else if (!pregame && !gameOver)
            {
                this.Window.Title = "M: Menu | R: Reset | Left Click: Fire";
            }

            else if (!pregame && gameOver)
            {
                this.Window.Title = "M: Menu | R: Reset || GAME OVER";
            }
           
            

        }

        #endregion
    }
}