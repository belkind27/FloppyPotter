using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FloppyBird
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartingMenu : Page
    {
         static MediaPlayer mediaPlayer = new MediaPlayer();
           
        public StartingMenu()
        {
            this.InitializeComponent();
            Uri uri = new Uri("ms-appx:///Assets/theme song.mp3");
            mediaPlayer.Source = MediaSource.CreateFromUri(uri);
            mediaPlayer.IsLoopingEnabled = true;
            mediaPlayer.Volume = 0.5;
            if (Settings.keySound == 0) key_check.IsChecked = true;
            if (Settings.keySound == 1) key_check.IsChecked = false;
            if (Settings.jumpSound == 0) jump_check.IsChecked = true;
            if (Settings.keySound == 1) jump_check.IsChecked = false; 
            if (Settings.winLoseSound == 0) winLose_check.IsChecked = true;
            if (Settings.winLoseSound == 1) winLose_check.IsChecked = false;
            if (Settings.themeSound == 0) theme_check.IsChecked = true;
            if (Settings.themeSound == 1) theme_check.IsChecked = false;
            if (Settings.snatcherSound == 0) snatcher_check.IsChecked = true;
            if (Settings.snatcherSound == 1) snatcher_check.IsChecked = false;
        }

        private void battle_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.keySound == 0) keySound.Play();
            easy_btn.Visibility = Visibility.Visible;
            medium_btn.Visibility = Visibility.Visible;
            hard_btn.Visibility = Visibility.Visible;

        }

        private void solo_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.keySound == 0) keySound.Play();
            Settings.Difficulty = 4;
            Frame.Navigate(typeof(MainPage));
            
        }


        private void easy_btn_Click_1(object sender, RoutedEventArgs e)
        {
            if (Settings.keySound == 0) keySound.Play();
            Settings.Difficulty = 1;
            Frame.Navigate(typeof(MainPage));
        }

        private void medium_btn_Click_1(object sender, RoutedEventArgs e)
        {
            if (Settings.keySound == 0) keySound.Play();
            Settings.Difficulty = 2;
            Frame.Navigate(typeof(MainPage));
        }

        private void hard_btn_Click_1(object sender, RoutedEventArgs e)
        {
            if(Settings.keySound==0)keySound.Play();
            Settings.Difficulty = 3;
            Frame.Navigate(typeof(MainPage));
        }




        private void key_check_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.keySound = 1;
        }

        private void key_check_Checked(object sender, RoutedEventArgs e)
        {
            Settings.keySound = 0;

        }

        private void winLose_check_Checked(object sender, RoutedEventArgs e)
        {
            Settings.winLoseSound = 0;

        }

        private void winLose_check_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.winLoseSound = 1;

        }

        private void jump_check_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.jumpSound = 1;

        }

        private void jump_check_Checked(object sender, RoutedEventArgs e)
        {
            Settings.jumpSound = 0;

        }

        private void settings_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.keySound == 0) keySound.Play();
            if (scroll_cnvs.Visibility == Visibility.Collapsed) scroll_cnvs.Visibility = Visibility.Visible;
            else  scroll_cnvs.Visibility = Visibility.Collapsed;
        }

        private void theme_check_Checked(object sender, RoutedEventArgs e)
        {
             mediaPlayer.Play();
            Settings.themeSound = 0;
            
        }

        private void theme_check_Unchecked(object sender, RoutedEventArgs e)
        {

            mediaPlayer.Pause();
            Settings.themeSound = 1;
            
        }

        private void snatcher_check_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.snatcherSound = 1;

        }

        private void snatcher_check_Checked(object sender, RoutedEventArgs e)
        {
            Settings.snatcherSound = 0;
        }
    }

}
