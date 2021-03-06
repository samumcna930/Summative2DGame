﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Threading;
using System.Timers;
using System.IO;

namespace Summative2DGame
{
    public partial class GameScreen : UserControl
    {
        //alien list
        public static List<Alien> topSide = new List<Alien>();

        //All bullets 
        public static List<Bullet> bulletList = new List<Bullet>();
        public static List<Bullet> leftBulletList1 = new List<Bullet>();
        public static List<Bullet> leftBulletList2 = new List<Bullet>();
        public static List<Bullet> rightBulletList1 = new List<Bullet>();
        public static List<Bullet> rightBulletList2 = new List<Bullet>();
        public static List<Bullet> alienBullet = new List<Bullet>();

        //brushes
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        SolidBrush starBrush = new SolidBrush(Color.White);
        SolidBrush healthBrush = new SolidBrush(Color.Lime);

        //sounds
        static SoundPlayer hit = new SoundPlayer(Properties.Resources.laserShot);
        SoundPlayer lose = new SoundPlayer(Properties.Resources.DieSound);
        SoundPlayer buttonClick = new SoundPlayer(Properties.Resources.ButtonClick);

        //key press booleans
        Boolean leftArrowDown, rightArrowDown, downArrowDown, upArrowDown, spaceKeyDown, escKeyDown;

        //creating player
        Player hero;

        //creating powerUp
        static PowerUp power;
        int powerCount = 0;

        static Rectangle shipRec;

        //All images used in game
        Image shipImage, alienImage, bulletImage, healthImage, laserImage;

        //alien specs
        int alienWidth, alienHeight, alienSpeed, spawnX;

        static int alienHealth = 3;

        //player specs
        static int playerWidth, playerHeight, playerHealth;

        //bullet specs
        static int bulletWidth, bulletHeight, bulletSpeed;


        int powerUp;
        int powerSpawnX;

        static Boolean powerActive = false;

        //timer

        static int killCount = 0;
        int counter = 0;
        int shotCounter = 21;
        int shotLimit = 8;
        Random randNum = new Random();
        public GameScreen()
        {
            buttonClick.Play();
            InitializeComponent();
            //outputLabel.Visible = false;
            gameOverLabel.Visible = false;
            OnStart();

        }
        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;
                case Keys.Space:
                    spaceKeyDown = true;
                    break;
                case Keys.Escape:
                    escKeyDown = true;
                    Pause();
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;
                case Keys.Space:
                    spaceKeyDown = false;
                    break;
                case Keys.Escape:
                    escKeyDown = false;
                    break;
            }
        }
        public void MakeBullet()
        {
            Bullet bullet = new Bullet(hero.x + 22, hero.y - 10, bulletWidth, bulletHeight, bulletImage);
            bulletList.Add(bullet);
        }
        public void OnStart()
        {
            Refresh();
            topSide.Clear();
            bulletList.Clear();
            leftBulletList1.Clear();
            leftBulletList2.Clear();
            rightBulletList1.Clear();
            rightBulletList1.Clear();

            alienWidth = 60;
            alienHeight = 100;
            alienSpeed = 10;

            playerWidth = 60;
            playerHeight = 80;
            playerHealth = 3;

            bulletWidth = 15;
            bulletHeight = 15;
            bulletSpeed = 20;

            bulletImage = Properties.Resources.BasicBullet;

            gameOverLabel.Visible = false;
            gameTimer.Enabled = true;

            alienImage = Properties.Resources.Monster;

            MakePowerUp();

            if (CharacterScreen.shipSelect == 1)
            {
                shipImage = Properties.Resources.PurpleJetNeutral;
                powerUp = 1;
            }
            else
            {
                shipImage = Properties.Resources.GreenJetHoveringNeutral;
                powerUp = 2;
            }
            hero = new Player(this.Width / 2 - 15, this.Height - 360, playerWidth, playerHeight, shipImage);
        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            shipRec = new Rectangle(hero.x, hero.y, hero.width, hero.height);

            counter++;
            shotCounter++;

            RegBulletCollision();

            PowerUpCollision();

            ShipCollision();

            if (powerActive == false && powerCount < 1)
            {
                MakePowerUp();
                powerCount = 1;
            }

            if (power.y < 400)
            {
                power.MovePower(4);
            }


            killLabel.Text = "KILLS: " + killCount + "/45";

            if (topSide.Count > 0)
            {
                if (topSide[0].y > this.Height - 140)
                {
                    topSide.RemoveAt(0);
                }
            }
            if (counter % 8 == 0)
            {
                if (topSide.Count < 18)
                {
                    SpawnTop();
                }
            }
            #region Move Alien
            foreach (Alien a in topSide)
            {
                a.MoveAlien(alienSpeed);
            }

            #endregion

            #region Moving player and shooting
            if (leftArrowDown == true && hero.x > 0)
            {
                hero.Move("left");
            }
            if (upArrowDown == true && hero.y > 0)
            {
                hero.Move("up");
            }
            if (downArrowDown == true && hero.y + playerHeight < this.Height - 170)
            {
                hero.Move("down");
            }
            if (rightArrowDown == true && hero.x < this.Width - hero.width)
            {
                hero.Move("right");
            }
            if (spaceKeyDown == true && shotCounter > shotLimit && powerActive == false)
            {
                shotCounter = 0;
                MakeBullet();
            }

            else if (spaceKeyDown == true && powerUp == 1 && powerActive)
            {
                MakeLaser();
            }
            else if (spaceKeyDown == true && powerUp == 2 && powerActive)
            {
                MakeMultiBullet();
            }
            else { }
            #endregion

            //move player bullet 
            if (powerActive == true && powerUp == 1)
            {
                foreach (Bullet b in leftBulletList1)
                {
                    b.MoveBullet(bulletSpeed);
                }
                foreach (Bullet b in leftBulletList2)
                {
                    b.MoveBullet(bulletSpeed);
                }
                foreach (Bullet b in rightBulletList1)
                {
                    b.MoveBullet(bulletSpeed);
                }
                foreach (Bullet b in rightBulletList2)
                {
                    b.MoveBullet(bulletSpeed);
                }
            }
            else
            {
                foreach (Bullet b in bulletList)
                {
                    b.MoveBullet(bulletSpeed);
                }
            }


            #region Game Win
            if (killCount > 44)
            {
                GameWin();
            }
            #endregion

            #region Game Over
            //change to if hero gets hit 3 times
            if (playerHealth < 1)
            {
                Refresh();
                GameOver();
            }
            #endregion

            Refresh();
        }
        public void GameWin()
        {
            gameTimer.Enabled = false;

            //outputLabel.Visible = false;
            gameOverLabel.Visible = true;
            gameOverLabel.Text = "You Win! Returning to main menu";
            gameOverLabel.Refresh();

            Thread.Sleep(2000);

            Form f = this.FindForm();
            f.Controls.Remove(this);
            MainScreen ms = new MainScreen();
            f.Controls.Add(ms);

        }
        public void MakeLaser()
        {
            Rectangle laserRec = new Rectangle(hero.x + 22, 0, bulletWidth, hero.y - 10);
        }
        public void MakePowerUp()
        {
            powerSpawnX = randNum.Next(300, 360);
            power = new PowerUp(powerSpawnX, 0, 15);
        }
        public void MakeMultiBullet()
        {

        }
        public static void LaserBeamCollision()
        {

        }
        public static void PowerUpCollision()
        {
            Rectangle powerRec = new Rectangle(power.x, power.y, bulletWidth, bulletWidth);
            if(powerRec.IntersectsWith(shipRec))
            {
                powerActive = true;
            }
        }
        public static void MultiGunCollision()
        {

            List<int> bulletRemove = new List<int>();
            List<int> alienRemove = new List<int>();
            foreach (Bullet b in leftBulletList1)
            {
                foreach (Alien a in topSide)
                {
                    if (a.Collision(b))
                    {
                        if (!bulletRemove.Contains(bulletList.IndexOf(b)))
                        {
                            bulletRemove.Add(bulletList.IndexOf(b));
                        }
                        if (!alienRemove.Contains(topSide.IndexOf(a)))
                        {
                            alienRemove.Add(topSide.IndexOf(a));
                        }
                    }
                }
            }
            bulletRemove.Reverse();
            alienRemove.Reverse();
            foreach (int i in bulletRemove)
            {
                bulletList.RemoveAt(i);
            }
            foreach (int i in alienRemove)
            {
                topSide.RemoveAt(i);
            }

        }
        public static void ShipCollision()
        {
            List<int> alienRemove = new List<int>();
            foreach (Alien a in topSide)
            {
                Rectangle alienRec = new Rectangle(a.x, a.y, a.width, a.height);
                 if (alienRec.IntersectsWith(shipRec))
                {
                    hit.Play();
                    playerHealth--;
                    if (!alienRemove.Contains(topSide.IndexOf(a)))
                    {
                        alienRemove.Add(topSide.IndexOf(a));
                    }
                }
            }
            alienRemove.Reverse();
            foreach (int i in alienRemove)
            {
                topSide.RemoveAt(i);
            }
        }
        public static void RegBulletCollision()
        {
            List<int> bulletRemove = new List<int>();
            List<int> alienRemove = new List<int>();
            foreach (Bullet b in bulletList)
            {
                foreach (Alien a in topSide)
                {
                    if (a.Collision(b))
                    {
                        a.health--;
                        if (!bulletRemove.Contains(bulletList.IndexOf(b)))
                        {
                            bulletRemove.Add(bulletList.IndexOf(b));
                        }
                        if (!alienRemove.Contains(topSide.IndexOf(a)) && a.health < 1)
                        {
                            alienRemove.Add(topSide.IndexOf(a));
                            killCount++;
                        }
                    }
                }
            }
            bulletRemove.Reverse();
            alienRemove.Reverse();
            foreach (int i in bulletRemove)
            {
                bulletList.RemoveAt(i);
            }
            foreach (int i in alienRemove)
            {
                topSide.RemoveAt(i);
            }
        }
        public void SpawnTop()
        {
            spawnX = randNum.Next(50, 800);
            //add box
            Alien newAlien = new Alien(spawnX, 0 - alienHeight, alienWidth, alienHeight, alienImage, alienHealth);
            topSide.Add(newAlien);
        }
        public void Pause()
        {
            if (gameTimer.Enabled == true)
            {

                gameTimer.Enabled = false;

                DialogResult dr = PauseForm.Show();

                if (dr == DialogResult.Cancel)
                {
                    gameTimer.Enabled = true;
                }
                else if (dr == DialogResult.Abort)
                {
                    Form form = this.FindForm();
                    MainScreen ms = new MainScreen();

                    form.Controls.Add(ms);
                    form.Controls.Remove(this);
                }
            }
        }
        public void GameOver()
        {
            gameTimer.Enabled = false;

            lose.Play();

            gameOverLabel.Visible = true;
            gameOverLabel.Text = "Game over, returning to main menu.";
            gameOverLabel.Refresh();

            Thread.Sleep(3000);

            Form f = this.FindForm();
            f.Controls.Remove(this);
            MainScreen ms = new MainScreen();
            f.Controls.Add(ms);

        }
        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            //draw alien
            foreach (Alien a in topSide)
            {
                e.Graphics.DrawImage(a.image, a.x, a.y, a.width, a.height);
            }

            //fills in UI
            e.Graphics.FillRectangle(blackBrush, 0, this.Height - 160, this.Width, this.Height);

            //draw UI line
            e.Graphics.FillRectangle(starBrush, 0, this.Height - 160, this.Width, 12);

            //change health image
            if (playerHealth == 3)
            {
                healthImage = Properties.Resources.HealthFull;
            }
            else if (playerHealth == 2)
            {
                healthImage = Properties.Resources.HealthHalf;
            }
            else if (playerHealth == 1)
            {
                healthImage = Properties.Resources.HealthLow;
            }
            else if (playerHealth < 1)
            {
                healthImage = Properties.Resources.HealthNone;
            }

            //draw status image
            e.Graphics.DrawImage(healthImage, this.Width - 194, 726, 90, 140);

            //drawhero
            e.Graphics.DrawImage(hero.image, hero.x, hero.y, hero.width, hero.height);

            if (powerActive == false)
            {
                //draw bullet
                foreach (Bullet b in bulletList)
                {
                    e.Graphics.DrawImage(b.image, b.x, b.y, b.width, b.height);
                }

                e.Graphics.FillRectangle(starBrush, powerSpawnX, power.y, 15, 15);
            }
            //else if (powerUp == 1 && powerActive)
            //{
            //    e.Graphics.DrawImage(laserImage, hero.x + 22, 0, bulletWidth, hero.y - 10);
            //}
            //else if (powerUp == 2 && powerActive)
            //{
            //    //draw quad gun
            //}
        }
    }
}
