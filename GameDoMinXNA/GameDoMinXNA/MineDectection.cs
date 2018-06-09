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
using System.Threading;

namespace GameDoMinXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MineDetection : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameState
        {
            MainMenu,
            Playing,
            HowToPlay,
            About,
            Hard,
            Easy,
            Medium,
            Exit,
        }
        enum Winner
        {
            Playing,
            Lose,
            Win,
        }
        enum StateButton
        {
            Hard, Easy, Medium, Playing,
        }
        GameState CurrentGameState = GameState.MainMenu;
        int x, y, fx, fy, count = 0;
        Texture2D textureBackgournd;
        cButton btnPlay, btnHowToPlay, btnAbout, btnBack, btnHard, btnEasy, btnMedium, btnExit, btnLose, btnWin;
        cPlayer player;
        Point StartPoint = new Point(150, 0);
        int[,] bState = new int[12, 12];
        cButton[,] Button = new cButton[11, 11];
        cButton[,] Button1 = new cButton[11, 11];
        Random rand = new Random();
        Winner winner = Winner.Playing;
        bool stopped = false;
        StateButton statebutton = StateButton.Playing;
        cExplosion Explosion;
        public MineDetection()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "Mine Detection";
            graphics.PreferredBackBufferWidth = 950;
            graphics.PreferredBackBufferHeight = 700;
        }
        protected override void Initialize()
        {
            IsMouseVisible = true;
            x = this.Window.ClientBounds.Width;
            y = this.Window.ClientBounds.Height;          
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);
            textureBackgournd = Content.Load<Texture2D>("MainMenu//BackGround");
            btnPlay = new cButton(this, Content.Load<Texture2D>("MainMenu//Play"), new Point(x /2 -75, y/2 - 200), new Vector2(150, 50));
            btnHowToPlay = new cButton(this, Content.Load<Texture2D>("MainMenu//HowToPlay"), new Point(x / 2 - 75, y / 2 - 100), new Vector2(150, 50));
            btnAbout = new cButton(this, Content.Load<Texture2D>("MainMenu//About"), new Point(x / 2 - 75, y / 2), new Vector2(150, 50));
            btnExit = new cButton(this, Content.Load<Texture2D>("MainMenu//Exit"), new Point(x / 2 - 75, y / 2 + 100), new Vector2(150, 50));
            btnBack = new cButton(this, Content.Load<Texture2D>("MainMenu//Back"), new Point(x - 150, y - 50), new Vector2(150, 50));
            btnEasy = new cButton(this, Content.Load<Texture2D>("MainMenu//Easy"), new Point(x / 2 - 75, y / 2 - 150), new Vector2(150, 50));
            btnMedium = new cButton(this, Content.Load<Texture2D>("MainMenu//Medium"), new Point(x / 2 - 75, y / 2 - 50), new Vector2(150, 50));
            btnHard = new cButton(this, Content.Load<Texture2D>("MainMenu//Hard"), new Point(x / 2 - 75, y / 2 + 50), new Vector2(150, 50));
            btnWin = new cButton(this, Content.Load<Texture2D>("MainMenu//Win"), new Point(x / 2 - 150, y / 2 - 75), new Vector2(300, 150));
            btnLose = new cButton(this, Content.Load<Texture2D>("MainMenu//Lose"), new Point(x / 2 - 150, y / 2 - 75), new Vector2(300, 150));
        }
        protected override void UnloadContent()
        {
        }
        private void Start(int NumberOfButton, int NumberOfBoom)
        {
            player = new cPlayer(this, Content.Load<Texture2D>("Sprite//PLayer"), StartPoint, new Point(NumberOfButton * 65 + 150, NumberOfButton * 65));
            fx = fy = NumberOfButton;
            stopped = false;
            CurrentGameState = GameState.MainMenu;
            count = 0;
            winner = Winner.Playing;
            Button = new cButton[11, 11];
            Button1 = new cButton[11, 11];
            bState = new int[12, 12];
            for (int i = 1; i <= NumberOfButton; i++)
            {
                for (int j = 1; j <= NumberOfButton; j++)
                {
                    bState[i, j] = 1;
                }
            }
            for (int i = 0; i < NumberOfButton + 1; i++)
            {
                bState[i, 0] = 10;
                bState[0, i] = 10;
                bState[i, NumberOfButton + 1] = 10;
                bState[NumberOfButton + 1, i] = 10;
            }
            bState[fx, fy] = 100;
            FindAway(fx, fy);
            RandomBoom(NumberOfBoom);
            CheckBoom();
            int by = 0;
            for (int i = 1; i <= NumberOfButton; i++)
            {
                int bx = 150;
                for (int j = 1; j <= NumberOfButton; j++)
                {
                    Button[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_cell"), new Point(bx, by), new Vector2(65, 65));
                    switch (bState[i, j])
                    {
                        case 1:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit1"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 2:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit2"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 3:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit3"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 4:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit4"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 5:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit5"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 6:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit6"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 7:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit7"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 8:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit8"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 0:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_poit9"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case -1:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_bom"), new Point(bx, by), new Vector2(65, 65));
                            break;
                        case 100:
                            Button1[i, j] = new cButton(this, Content.Load<Texture2D>("Buttons//img_finish"), new Point(bx, by), new Vector2(65, 65));
                            break;
                    }
                    bx += 65;
                }
                by += 65;
            }
        }
        private void MainMenu()
        {
            MouseState mouse = Mouse.GetState();

            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    if (btnPlay.IsClicked) CurrentGameState = GameState.Playing;
                    if (btnHowToPlay.IsClicked) CurrentGameState = GameState.HowToPlay;
                    if (btnAbout.IsClicked) CurrentGameState = GameState.About;
                    if (btnExit.IsClicked) CurrentGameState = GameState.Exit;
                    btnPlay.Update(mouse);
                    btnHowToPlay.Update(mouse);
                    btnAbout.Update(mouse);
                    btnExit.Update(mouse);
                    break;
                case GameState.Playing:
                    if (btnBack.IsClicked) CurrentGameState = GameState.MainMenu;
                    if (btnEasy.IsClicked) CurrentGameState = GameState.Easy;
                    if (btnHard.IsClicked) CurrentGameState = GameState.Hard;
                    if (btnMedium.IsClicked) CurrentGameState = GameState.Medium;
                    btnBack.Update(mouse);
                    btnMedium.Update(mouse);
                    btnHard.Update(mouse);
                    btnEasy.Update(mouse);
                    break;
                case GameState.HowToPlay:
                    if (btnBack.IsClicked) CurrentGameState = GameState.MainMenu;
                    btnBack.Update(mouse);
                    break;
                case GameState.About:
                    if (btnBack.IsClicked) CurrentGameState = GameState.MainMenu;
                    btnBack.Update(mouse);
                    break;
                case GameState.Hard:
                    if (statebutton == StateButton.Playing)
                    {
                        Start(10, 33);
                        statebutton = StateButton.Hard;
                    }
                    if (btnBack.IsClicked) statebutton = StateButton.Playing;
                    btnWin.Update(mouse);
                    btnLose.Update(mouse);
                    btnBack.Update(mouse);
                    if (winner == Winner.Playing)
                        player.Update();
                    if (stopped == false)
                        CheckCollision(10);
                    break;
                case GameState.Easy:
                    if (statebutton == StateButton.Playing)
                    {
                        Start(4, 5);
                        statebutton = StateButton.Easy;
                    }
                    if (btnBack.IsClicked) statebutton = StateButton.Playing;
                    btnWin.Update(mouse);
                    btnLose.Update(mouse);
                    btnBack.Update(mouse);
                    if (winner == Winner.Playing)
                        player.Update();
                    if (stopped == false)
                        CheckCollision(4);
                    break;
                case GameState.Medium:
                    if (statebutton == StateButton.Playing)
                    {
                        Start(8, 21);
                        statebutton = StateButton.Medium;
                    }
                    if (btnBack.IsClicked) statebutton = StateButton.Playing;
                    btnWin.Update(mouse);
                    btnLose.Update(mouse);
                    btnBack.Update(mouse);
                    if (winner == Winner.Playing)
                        player.Update();
                    if (stopped == false)
                        CheckCollision(8);
                    break;
                case GameState.Exit:
                    break;
            }
        }
        private void DrawMainMenu()
        {
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    btnPlay.Draw(spriteBatch);
                    btnHowToPlay.Draw(spriteBatch);
                    btnAbout.Draw(spriteBatch);
                    btnExit.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    btnBack.Draw(spriteBatch);
                    btnEasy.Draw(spriteBatch);
                    btnMedium.Draw(spriteBatch);
                    btnHard.Draw(spriteBatch);
                    break;
                case GameState.HowToPlay:
                    btnBack.Draw(spriteBatch);
                    break;
                case GameState.About:
                    btnBack.Draw(spriteBatch);
                    break;
                case GameState.Hard:
                    if (statebutton == StateButton.Hard)
                    {
                        btnBack.Draw(spriteBatch);
                        for (int i = 1; i <= 10; i++)
                        {
                            for (int j = 1; j <= 10; j++)
                            {
                                Button[i, j].Draw(spriteBatch);
                            }
                        }
                        player.Draw(spriteBatch);
                        if (winner == Winner.Lose)
                        {
                            btnLose.Draw(spriteBatch);
                            stopped = true;
                        }
                        else if (winner == Winner.Win)
                        {
                            btnWin.Draw(spriteBatch);
                            stopped = true;
                        }
                    }     
                    break;
                case GameState.Easy:
                    if (statebutton == StateButton.Easy)
                    {
                        btnBack.Draw(spriteBatch);
                        for (int i = 1; i <= 4; i++)
                        {
                            for (int j = 1; j <= 4; j++)
                            {
                                Button[i, j].Draw(spriteBatch);
                            }
                        }
                        player.Draw(spriteBatch);
                        if (winner == Winner.Lose)
                        {
                            btnLose.Draw(spriteBatch);
                            stopped = true;
                        }
                        else if (winner == Winner.Win)
                        {
                            btnWin.Draw(spriteBatch);
                            stopped = true;
                        }
                    }               
                    break;
                case GameState.Medium:
                    if (statebutton == StateButton.Medium)
                    {
                        btnBack.Draw(spriteBatch);
                        for (int i = 1; i <= 8; i++)
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                Button[i, j].Draw(spriteBatch);
                            }
                        }
                        player.Draw(spriteBatch);
                        if (winner == Winner.Lose)
                        {
                            btnLose.Draw(spriteBatch);
                            stopped = true;
                        }
                        else if (winner == Winner.Win)
                        {
                            btnWin.Draw(spriteBatch);
                            stopped = true;
                        }
                    }        
                    break;
                case GameState.Exit:
                    this.Exit();
                    break;
            }
        }
        private void CheckBoom()
        {
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    int count = 0;
                    if (bState[i, j] != -1)
                    {
                        if (i == 10 && j == 10)
                        {
                            break;
                        }
                        else
                        {
                            if (bState[i - 1, j - 1] == -1) count++;
                            if (bState[i - 1, j] == -1) count++;
                            if (bState[i - 1, j + 1] == -1) count++;
                            if (bState[i, j - 1] == -1) count++;
                            if (bState[i, j + 1] == -1) count++;
                            if (bState[i + 1, j - 1] == -1) count++;
                            if (bState[i + 1, j] == -1) count++;
                            if (bState[i + 1, j + 1] == -1) count++;
                            bState[i, j] = count;
                        }

                    }
                }
            }
            bState[fx, fy] = 100;
        }
        private void RandomBoom(int NumberOfBoom)
        {
            while (count < NumberOfBoom) // random boom
            {
                int i = rand.Next(1, 10), j = rand.Next(1, 10);
                if (bState[i, j] == 1)
                {
                    bState[i, j] = -1;
                    count++;
                }
            }
        }
        private void FindAway(int fx, int fy)
        {
            int a = 1;
            int b = 1;
            bState[1, 1] = 20;   
            switch (rand.Next(1, 2)) // tìm đường đi đầu tiên
            {
                case 1:
                    bState[1, 2] = 20;
                    b = 2;
                    break;
                case 2:
                    bState[2, 1] = 20;
                    a = 2;
                    break;
                default:
                    break;
            }
            while (bState[fx, fy] != 20)
            {
                int[] r = new int[5];
                r[1] = 1;
                r[2] = 2;
                r[3] = 3;
                r[4] = 4;
                if (a == 1) r[3] = 0;
                if (a == fx) r[1] = 0;
                if (b == 1) r[4] = 0;
                if (b == fy) r[2] = 0;

                int t = rand.Next(1, 4);
                while (r[t] == 0)
                    t = rand.Next(1, 4);
                switch (t)
                {
                    case 1:
                        bState[a + 1, b] = 20;
                        //  ButtonTexture[a + 1, b] = tetu;
                        a = a + 1;
                        break;
                    case 2:
                        bState[a, b + 1] = 20;

                        //   ButtonTexture[a, b + 1] = tetu;
                        b = b + 1;
                        break;
                    case 3:
                        bState[a - 1, b] = 20;
                        //   ButtonTexture[a - 1, b] = tetu;
                        a = a - 1;
                        break;
                    case 4:
                        bState[a, b - 1] = 20;
                        // ButtonTexture[a, b - 1] = tetu;
                        b = b - 1;
                        break;
                    default:
                        break;
                }
            }
        }
        private void CheckCollision(int NumberOfButtons)
        {
            for (int i = 1; i <= NumberOfButtons; i++)
            {
                for (int j = 1; j <= NumberOfButtons; j++)
                {
                    if (player.getbound(Button[i, j].Getbounds()))
                    {
                        if (i == fx && j == fy)
                        {
                            Button[i, j] = Button1[fx, fy];
                            for (int x = 1; x <= 10; x++)
                            {
                                for (int y = 1; y <= 10; y++)
                                {
                                    Button[x, y] = Button1[x, y];
                                }                             
                            }
                            winner = Winner.Win;
                        }
                        else
                            Button[i, j] = Button1[i, j];


                        if (bState[i, j] == -1)
                        {
                            Explosion = new cExplosion(this, new Point(150 + (j - 1) * 65, (i - 1) * 65), Content.Load<Texture2D>("Sprite//Explosion"));
                            Components.Add(Explosion);
                            Button[i, j] = Button1[i, j];
                            winner = Winner.Lose;
                        }

                    }

                }
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            MainMenu();

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(textureBackgournd, new Rectangle(0, 0, x, y), Color.White);
            DrawMainMenu();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
