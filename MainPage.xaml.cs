using FloppyBird.Assets;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Point = Windows.Foundation.Point;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FloppyBird
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        DispatcherTimer timer = new DispatcherTimer();
        #region Data members
        string key;//string key to read input
        Player player1, opponent, snatcher, wingLeft, wingRight;//objects that move in diffrent logic
        Pipes pipeUp1, pipeDown1, pipeUp2, pipeDown2, pipeUp3, pipeDown3, pipeUp4, pipeDown4;
        double gravityCounter;//gravity counter for smooth falling animation
        double hieghtPipeUp, hieghtPipeDown, oppJump, oppFall;
        double speed, angleWing;
        bool isPressed, isOverPipeDown1, isOverPipeDown2, isOverPipeDown3, isOverPipeDown4, isBegining, isEnd, isEndOpponent, isSnatcherUp, isSnatcherDown, isSnatcherAlive, isWingUp, isWingDown;
        int score, difficultSetter, opponentRandomCounter, timeCounter, snatcherCounter;
        #endregion
        public MainPage()
        {
            this.InitializeComponent();
            #region size displaying settings
            ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size((double)1280, (double)720);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            #endregion
            initValues();
            playerSettings();
            opponentSettings();
            pipeSettings();
            snatcherSettings();
            #region keys
            Window.Current.CoreWindow.KeyDown += KeyDown;
            Window.Current.CoreWindow.KeyUp += KeyUp;
            #endregion
            #region timer
            timer.Tick += GameLoop;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();
            #endregion

        }
        public void initValues()
        {
            gravityCounter = oppJump = oppFall = 0;
            speed = 10;
            score = difficultSetter = opponentRandomCounter = timeCounter = snatcherCounter = 0;
            isEnd = false;
            isEndOpponent = false;
            isSnatcherAlive = false;

        }
        public void GameLoop(object sender, object e)
        {
            PlayerMovement();
            OpponentMovment();
            PipeMovment();
            Crashing();
            SwitchingPipes();
            snatcherSwitching();
            ScoreCounting();
            if (isSnatcherAlive == true) snatcherMovment();
            timeCounter++;
            snatcherCounter++;
            if (timeCounter % 100 == 0)
            { opponentRandomCounter++; }
            if (snatcherCounter % 450 == 0)
            { snatcherCounter = 0; isSnatcherAlive = true; }

        }
        public void playerSettings()
        {
            #region player settings
            player1 = new Player(100, 200, "ms-appx:///Assets/harryPotter.png", 100, 150, 0.5, 0.5);
            player1.Addtocanvas(borad);
            Canvas.SetZIndex(player1._img, 3);
            #endregion
        }
        public void opponentSettings()
        {
            #region opponent settings
            opponent = new Player(360, 200, "ms-appx:///Assets/ron.png.png", 100, 150, 0.5, 0.5);
            opponent.Addtocanvas(borad);
            if (Settings.Difficulty == 4) borad.Children.Remove(opponent._img);
            #endregion
        }
        public void pipeSettings()
        {
            isBegining = true;
            #region Pipe settings
            RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
            pipeUp1 = new Pipes(0, 1260, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
            pipeDown1 = new Pipes(700 - hieghtPipeDown, 1260, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
            pipeDown1.Addtocanvas(borad);
            pipeUp1.Addtocanvas(borad);
            RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
            pipeUp2 = new Pipes(0, 1760, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
            pipeDown2 = new Pipes(700 - hieghtPipeDown, 1760, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
            pipeDown2.Addtocanvas(borad);
            pipeUp2.Addtocanvas(borad);
            RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
            pipeUp3 = new Pipes(0, 2260, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
            pipeDown3 = new Pipes(700 - hieghtPipeDown, 2260, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
            pipeDown3.Addtocanvas(borad);
            pipeUp3.Addtocanvas(borad);
            RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
            pipeUp4 = new Pipes(0, 2760, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
            pipeDown4 = new Pipes(700 - hieghtPipeDown, 2760, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
            pipeDown4.Addtocanvas(borad);
            pipeUp4.Addtocanvas(borad);
            #endregion            
        }
        public void SwitchingPipes()
        {
            //checks if each pipe is out of the screen and resets it with diffrent highet

            if (Canvas.GetLeft(pipeDown1._img) < -100)
            {
                borad.Children.Remove(pipeDown1._img);
                borad.Children.Remove(pipeUp1._img);
                RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
                pipeUp1 = new Pipes(0, 1960, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
                pipeDown1 = new Pipes(700 - hieghtPipeDown, 1960, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
                pipeDown1.Addtocanvas(borad);
                pipeUp1.Addtocanvas(borad);
                isOverPipeDown1 = false;
            }
            if (Canvas.GetLeft(pipeDown1._img) < -100)
            {
                borad.Children.Remove(pipeDown1._img);
                borad.Children.Remove(pipeUp1._img);
                RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
                pipeUp1 = new Pipes(0, 1960, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
                pipeDown1 = new Pipes(700 - hieghtPipeDown, 1960, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
                pipeDown1.Addtocanvas(borad);
                pipeUp1.Addtocanvas(borad);
                isOverPipeDown1 = false;
            }
            if (Canvas.GetLeft(pipeDown2._img) < -100)
            {
                borad.Children.Remove(pipeDown2._img);
                borad.Children.Remove(pipeUp2._img);
                RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
                pipeUp2 = new Pipes(0, 1960, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
                pipeDown2 = new Pipes(700 - hieghtPipeDown, 1960, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
                pipeDown2.Addtocanvas(borad);
                pipeUp2.Addtocanvas(borad);
                isOverPipeDown2 = false;

            }
            if (Canvas.GetLeft(pipeDown3._img) < -100)
            {
                borad.Children.Remove(pipeDown3._img);
                borad.Children.Remove(pipeUp3._img);
                RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
                pipeUp3 = new Pipes(0, 1960, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
                pipeDown3 = new Pipes(700 - hieghtPipeDown, 1960, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
                pipeDown3.Addtocanvas(borad);
                pipeUp3.Addtocanvas(borad);
                isOverPipeDown3 = false;

            }
            if (Canvas.GetLeft(pipeDown4._img) < -100)
            {
                borad.Children.Remove(pipeDown4._img);
                borad.Children.Remove(pipeUp4._img);
                RandomHieghtPipes(out hieghtPipeDown, out hieghtPipeUp);
                pipeUp4 = new Pipes(0, 1960, "ms-appx:///Assets/pipeUP.png", hieghtPipeUp, 100);
                pipeDown4 = new Pipes(700 - hieghtPipeDown, 1960, "ms-appx:///Assets/pipeDown.png", hieghtPipeDown, 100);
                pipeDown4.Addtocanvas(borad);
                pipeUp4.Addtocanvas(borad);
                isOverPipeDown4 = false;

            }
        }
        public void PipeMovment()
        {

            Canvas.SetLeft(pipeDown1._img, Canvas.GetLeft(pipeDown1._img) - speed);
            Canvas.SetLeft(pipeUp1._img, Canvas.GetLeft(pipeUp1._img) - speed);
            Canvas.SetLeft(pipeDown2._img, Canvas.GetLeft(pipeDown2._img) - speed);
            Canvas.SetLeft(pipeUp2._img, Canvas.GetLeft(pipeUp2._img) - speed);
            Canvas.SetLeft(pipeDown3._img, Canvas.GetLeft(pipeDown3._img) - speed);
            Canvas.SetLeft(pipeUp3._img, Canvas.GetLeft(pipeUp3._img) - speed);
            Canvas.SetLeft(pipeDown4._img, Canvas.GetLeft(pipeDown4._img) - speed);
            Canvas.SetLeft(pipeUp4._img, Canvas.GetLeft(pipeUp4._img) - speed);
            speed += 0.02;
        }
        public void PlayerMovement()
        {
            if (key == "Space" && isPressed == false)
            {
                Canvas.SetTop(player1._img, Canvas.GetTop(player1._img) - 35);
                gravityCounter = 0;//resets the gravity to crate smoove animation
                isPressed = true;//cenceling pressing
                player1.rotateJump(330);
                if (Settings.jumpSound == 0) jump.Play();
            }
            else
            {
                gravityCounter++;//if player dont jump gravitiy gets faster
                Canvas.SetTop(player1._img, Canvas.GetTop(player1._img) + gravityCounter * 0.987);
                isPressed = false;
                //changing the angle according the gravity counter to crate falling animation
                if (gravityCounter > 12) player1.rotateJump(30);
                if (gravityCounter < 5) player1.rotateJump(330);
                if (gravityCounter > 5 && gravityCounter < 12) player1.rotateJump(0);
            }
        }
        public void KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            key = args.VirtualKey.ToString();
            if (key == "Enter" && isEnd) restart();
            if (key == "Enter" && isEndOpponent) restart();
            if (key == "Escape" && isEnd)
            {

                Frame.Navigate(typeof(StartingMenu));
                if (Settings.keySound == 0) keySound.Play();
            }
            if (key == "Escape" && isEndOpponent)
            {

                Frame.Navigate(typeof(StartingMenu));
                if (Settings.keySound == 0) keySound.Play();
            }
        }
        public void KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            key = "";

        }
        public void RandomHieghtPipes(out double hpd, out double hpu)
        {
            Random rand1 = new Random(), rand2 = new Random();
            do
            {
                hpu = rand1.Next(100, 500);
                hpd = rand2.Next(100, 500);
            } while (hpu + hpd != 450);

        }
        public void Crashing()
        {
            #region craeting rectangle
            Rectangle playerRectangle = new Rectangle((int)Canvas.GetLeft(player1._img), (int)Canvas.GetTop(player1._img), (int)player1._width - 50, (int)player1._highet - 50);
            Rectangle snatcherRectangle = new Rectangle((int)Canvas.GetLeft(snatcher._img), (int)Canvas.GetTop(snatcher._img), (int)snatcher._width - 20, (int)snatcher._highet - 40);
            Rectangle opponentRectangle = new Rectangle((int)Canvas.GetLeft(opponent._img), (int)Canvas.GetTop(opponent._img), (int)opponent._width - 30, (int)opponent._highet - 30);
            Rectangle pipeDown1Rectangle = new Rectangle((int)Canvas.GetLeft(pipeDown1._img), (int)Canvas.GetTop(pipeDown1._img), (int)pipeDown1._width, (int)pipeDown1._highet);
            Rectangle pipeDown2Rectangle = new Rectangle((int)Canvas.GetLeft(pipeDown2._img), (int)Canvas.GetTop(pipeDown2._img), (int)pipeDown2._width, (int)pipeDown2._highet);
            Rectangle pipeDown3Rectangle = new Rectangle((int)Canvas.GetLeft(pipeDown3._img), (int)Canvas.GetTop(pipeDown3._img), (int)pipeDown3._width, (int)pipeDown3._highet);
            Rectangle pipeDown4Rectangle = new Rectangle((int)Canvas.GetLeft(pipeDown4._img), (int)Canvas.GetTop(pipeDown4._img), (int)pipeDown4._width, (int)pipeDown4._highet);
            Rectangle pipeUp1Rectangle = new Rectangle((int)Canvas.GetLeft(pipeUp1._img), (int)Canvas.GetTop(pipeUp1._img), (int)pipeUp1._width, (int)pipeUp1._highet);
            Rectangle pipeUp2Rectangle = new Rectangle((int)Canvas.GetLeft(pipeUp2._img), (int)Canvas.GetTop(pipeUp2._img), (int)pipeUp2._width, (int)pipeUp2._highet);
            Rectangle pipeUp3Rectangle = new Rectangle((int)Canvas.GetLeft(pipeUp3._img), (int)Canvas.GetTop(pipeUp3._img), (int)pipeUp3._width, (int)pipeUp3._highet);
            Rectangle pipeUp4Rectangle = new Rectangle((int)Canvas.GetLeft(pipeUp4._img), (int)Canvas.GetTop(pipeUp4._img), (int)pipeUp4._width, (int)pipeUp4._highet);
            #endregion
            #region player interactions
            if (playerRectangle.IntersectsWith(pipeDown1Rectangle) == true || playerRectangle.IntersectsWith(pipeUp1Rectangle) ||
                 playerRectangle.IntersectsWith(pipeDown2Rectangle) == true || playerRectangle.IntersectsWith(pipeUp2Rectangle) ||
                 playerRectangle.IntersectsWith(pipeDown3Rectangle) == true || playerRectangle.IntersectsWith(pipeUp3Rectangle) ||
                 playerRectangle.IntersectsWith(pipeDown4Rectangle) == true || playerRectangle.IntersectsWith(pipeUp4Rectangle) ||
                 playerRectangle.Top <= 0 || playerRectangle.Top >= borad.ActualHeight)
            { EndGame(); }
            #endregion
            #region opponent interactions
            if (Settings.Difficulty != 4) if (opponentRectangle.IntersectsWith(pipeDown1Rectangle) == true || opponentRectangle.IntersectsWith(pipeUp1Rectangle) ||
                opponentRectangle.IntersectsWith(pipeDown2Rectangle) == true || opponentRectangle.IntersectsWith(pipeUp2Rectangle) ||
                opponentRectangle.IntersectsWith(pipeDown3Rectangle) == true || opponentRectangle.IntersectsWith(pipeUp3Rectangle) ||
                opponentRectangle.IntersectsWith(pipeDown4Rectangle) == true || opponentRectangle.IntersectsWith(pipeUp4Rectangle) ||
                opponentRectangle.Top <= 0 || opponentRectangle.Top >= borad.ActualHeight)
                { EndGameOpponent(); }
            #endregion
            #region snatcher interactions
            if (Settings.Difficulty == 4) if (playerRectangle.IntersectsWith(snatcherRectangle))
                {
                    score += 5;
                    if (Settings.snatcherSound == 0) snatcherSound.Play();
                    borad.Children.Remove(snatcher._img);
                    borad.Children.Remove(wingRight._img);
                    borad.Children.Remove(wingLeft._img);
                    isSnatcherAlive = false;
                    snatcherSettings();
                }
            #endregion
        }
        public void EndGame()
        {
            if (Settings.winLoseSound == 0) lose.Play();
            timer.Stop();
            Loser_txt.Foreground = new SolidColorBrush(Colors.Red);
            Loser_txt.Text = $"YOU LOST\nYOURE SCORE IS {score}\nPRESS ENTER TO PLAY\nPRESS ESCAPE TO GO BACK TO MENU";
            Loser_txt.Visibility = Visibility.Visible;
            isEnd = true;
            if (score > Settings.score) Settings.score = score;//cheks if you score higher then before
        }
        public void EndGameOpponent()
        {
            if (Settings.winLoseSound == 0) win.Play();
            timer.Stop();
            Loser_txt.Foreground = new SolidColorBrush(Colors.Green);
            Loser_txt.Text = $"YOU WON\nYOURE SCORE IS {score}\nPRESS ENTER TO PLAY\nPRESS ESCAPE TO GO BACK TO MENU";
            Loser_txt.Visibility = Visibility.Visible;
            isEndOpponent = true;
            if (score > Settings.score) Settings.score = score;//cheks if you score higher then before
        }
        public void ScoreCounting()
        {
            #region checks if the pipe is in line with player to crate score
            if (Canvas.GetLeft(pipeDown1._img) < player1._left && isOverPipeDown1 == false)
            {
                score++;
                isOverPipeDown1 = true;
                isBegining = false;
            }
            if (Canvas.GetLeft(pipeDown2._img) < player1._left && isOverPipeDown2 == false)
            {
                score++;
                isOverPipeDown2 = true;
            }
            if (Canvas.GetLeft(pipeDown3._img) < player1._left && isOverPipeDown3 == false)
            {
                score++;
                isOverPipeDown3 = true;
            }
            if (Canvas.GetLeft(pipeDown4._img) < player1._left && isOverPipeDown4 == false)
            {
                score++;
                isOverPipeDown4 = true;
            }
            #endregion

            number_txt.Text = $"{score}";//score showing

            if (Settings.score > 0)//how to represent high score
            {
                numberHighScore_txt.Text = $"{Settings.score}";
                numberHighScore_txt.Visibility = Visibility.Visible;
                highscore_txt.Visibility = Visibility.Visible;
            }
        }
        public void OpponentMovment()
        {
            #region crates the diffirntioal from the prfect place according to difficulty
            Random rand = new Random();
            int difficulty = 0;
            if (Settings.Difficulty == 1 && opponentRandomCounter > 0)//easy
            {
                difficultSetter += 30;

                opponentRandomCounter = 0;
            }
            if (Settings.Difficulty == 2 && opponentRandomCounter > 0)//medium
            {
                difficultSetter += 20;

                opponentRandomCounter = 0;
            }
            if (Settings.Difficulty == 3 && opponentRandomCounter > 0)//hard
            {
                difficultSetter += 10;

                opponentRandomCounter = 0;
            }
            difficulty = rand.Next(-difficultSetter, difficultSetter);
            #endregion
            #region checks which pipe is closest to opponent and adding the value according to the difficulty
            double n1, n2, n3, n4;
            n1 = Canvas.GetLeft(pipeDown1._img) - opponent._left;
            n2 = Canvas.GetLeft(pipeDown2._img) - opponent._left;
            n3 = Canvas.GetLeft(pipeDown3._img) - opponent._left;
            n4 = Canvas.GetLeft(pipeDown4._img) - opponent._left;
            //if pipe is behind opponent gets big value so it wont be the closest
            if (n1 < -75) n1 = 1500;
            if (n2 < -75) n2 = 1500;
            if (n3 < -75) n3 = 1500;
            if (n4 < -75) n4 = 1500;
            //each if is for a case opponent is closest to certain pipe,checks if opponent need to go up an down and addig the difficulty value
            if (Math.Min(Math.Min(n1, n2), Math.Min(n3, n4)) == n1)
            {
                if (Canvas.GetTop(opponent._img) > pipeUp1._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) - oppJump);
                    oppJump++;
                    oppFall = 0;

                }
                else if (Canvas.GetTop(opponent._img) < pipeUp1._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) + oppFall);
                    oppFall++;
                    oppJump = 0;

                }

            }
            if (Math.Min(Math.Min(n1, n2), Math.Min(n3, n4)) == n2)
            {
                if (Canvas.GetTop(opponent._img) > pipeUp2._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) - oppJump);
                    oppJump++;
                    oppFall = 0;
                }
                else if (Canvas.GetTop(opponent._img) < pipeUp2._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) + oppFall);
                    oppFall++;
                    oppJump = 0;
                }
            }
            if (Math.Min(Math.Min(n1, n2), Math.Min(n3, n4)) == n3)
            {
                if (Canvas.GetTop(opponent._img) > pipeUp3._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) - oppJump);
                    oppJump++;
                    oppFall = 0;
                }
                else if (Canvas.GetTop(opponent._img) < pipeUp3._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) + oppFall);
                    oppFall++;
                    oppJump = 0;
                }
            }
            if (Math.Min(Math.Min(n1, n2), Math.Min(n3, n4)) == n4)
            {
                if (Canvas.GetTop(opponent._img) > pipeUp4._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) - oppJump);
                    oppJump++;
                    oppFall = 0;
                }
                else if (Canvas.GetTop(opponent._img) < pipeUp4._highet + 75 + difficulty)
                {
                    Canvas.SetTop(opponent._img, Canvas.GetTop(opponent._img) + oppFall);
                    oppFall++;
                    oppJump = 0;
                }

            }

            #endregion
        }
        public void restart()
        {
            if (Settings.keySound == 0) keySound.Play();
            Loser_txt.Visibility = Visibility.Collapsed;
            #region remove objects
            borad.Children.Remove(player1._img);
            borad.Children.Remove(opponent._img);
            borad.Children.Remove(pipeDown1._img);
            borad.Children.Remove(pipeUp1._img);
            borad.Children.Remove(pipeDown2._img);
            borad.Children.Remove(pipeUp2._img);
            borad.Children.Remove(pipeDown3._img);
            borad.Children.Remove(pipeUp3._img);
            borad.Children.Remove(pipeDown4._img);
            borad.Children.Remove(pipeUp4._img);
            borad.Children.Remove(snatcher._img);
            borad.Children.Remove(wingRight._img);
            borad.Children.Remove(wingLeft._img);
            #endregion
            initValues();
            playerSettings();
            opponentSettings();
            pipeSettings();
            snatcherSettings();
            timer.Start();

        }
        public void snatcherSettings()
        {
            snatcher = new Player(300, 1430, "ms-appx:///Assets/snatcherBody.png", 80, 80, 0.5, 0.5);
            wingRight = new Player(328, 1484, "ms-appx:///Assets/wingRight.png", 40, 80, 0, 1);
            wingLeft = new Player(328, 1384, "ms-appx:///Assets/wingLeft.png", 40, 80, 1, 1);
            snatcher.Addtocanvas(borad);
            wingLeft.Addtocanvas(borad);
            wingRight.Addtocanvas(borad);
            Canvas.SetZIndex(snatcher._img, 2);
            Canvas.SetZIndex(wingLeft._img, 1);
            Canvas.SetZIndex(wingRight._img, 1);
            snatcher._img.Stretch = Stretch.UniformToFill;
            wingLeft._img.Stretch = Stretch.UniformToFill;
            wingRight._img.Stretch = Stretch.UniformToFill;
            angleWing = 30;
            isWingUp = true;
            isSnatcherDown = true;
            if (Settings.Difficulty != 4)
            {
                borad.Children.Remove(snatcher._img);
                borad.Children.Remove(wingRight._img);
                borad.Children.Remove(wingLeft._img);
            }
        }
        public void snatcherMovment()
        {
            int speedLeft = 10, speedTop = 20;
            //snatcher move as one object but contain 3
            #region snatcher moving right
            Canvas.SetLeft(snatcher._img, Canvas.GetLeft(snatcher._img) - speedLeft);
            Canvas.SetLeft(wingRight._img, Canvas.GetLeft(wingRight._img) - speedLeft);
            Canvas.SetLeft(wingLeft._img, Canvas.GetLeft(wingLeft._img) - speedLeft);
            #endregion
            //sets bounderies for the snatcher movment and make him move zigzag
            #region snatcher up & down
            if (Canvas.GetTop(snatcher._img) < 100)
            {
                isSnatcherDown = false;
                isSnatcherUp = true;
            }
            if (Canvas.GetTop(snatcher._img) > 500)
            {
                isSnatcherDown = true;
                isSnatcherUp = false;
            }
            if (isSnatcherDown == true)
            {
                Canvas.SetTop(snatcher._img, Canvas.GetTop(snatcher._img) - speedTop);
                Canvas.SetTop(wingRight._img, Canvas.GetTop(wingRight._img) - speedTop);
                Canvas.SetTop(wingLeft._img, Canvas.GetTop(wingLeft._img) - speedTop);
            }
            if (isSnatcherUp == true)
            {
                Canvas.SetTop(snatcher._img, Canvas.GetTop(snatcher._img) + speedTop);
                Canvas.SetTop(wingRight._img, Canvas.GetTop(wingRight._img) + speedTop);
                Canvas.SetTop(wingLeft._img, Canvas.GetTop(wingLeft._img) + speedTop);
            }
            #endregion
            //makes the wing fliping effect by changing angles
            #region wing flipping
            wingLeft.rotateJump(-angleWing);
            wingRight.rotateJump(angleWing);
            if (angleWing == -30) { isWingDown = true; isWingUp = false; }
            if (angleWing == 30) { isWingDown = false; isWingUp = true; }
            if (isWingUp == true) angleWing -= 30;
            if (isWingDown == true) angleWing += 30;
            #endregion
        }
        public void snatcherSwitching()
        {
            if (Canvas.GetLeft(snatcher._img) < -100)//if the snatcher pass the screen line it resets 
            {
                borad.Children.Remove(snatcher._img);
                borad.Children.Remove(wingRight._img);
                borad.Children.Remove(wingLeft._img);
                isSnatcherAlive = false;
                snatcherSettings();
            }
        }
    }

}
