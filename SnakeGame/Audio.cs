using System;
using System.Windows.Media;

namespace SnakeGame
{
    public static class Audio
    {
        public readonly static MediaPlayer Game = LoadAudio("Game.wav", 1);
        public readonly static MediaPlayer Gulp = LoadAudio2("Gulp.wav", 1);
        public readonly static MediaPlayer GameOver = LoadAudio("GameOver.wav", 1);
        public readonly static MediaPlayer Shopkeep1 = LoadAudio("Shopkeep1.wav", 1); 
        private static MediaPlayer LoadAudio(string filename, double volume = 1, bool autoReset = true)
        {
            MediaPlayer player = new();
            player.Open(new Uri($"Assets/{filename}", UriKind.Relative));
            player.MediaEnded += PlayerRepeat_MediaEnded;
            return player;
        }

        private static MediaPlayer LoadAudio2(string filename, double volume = 1)
        {
            MediaPlayer player = new();
            player.Open(new Uri($"Assets/{filename}", UriKind.Relative));
            player.MediaEnded += Player_MediaEnded;
            return player;
        }

        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Stop();
            m.Position = new TimeSpan(0);
        }

        private static void PlayerRepeat_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Stop();
            m.Position = new TimeSpan(0);
            m.Play();
        }
    }
}
