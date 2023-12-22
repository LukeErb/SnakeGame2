using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnakeGame
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Body = LoadImage("Body.png");
        public readonly static ImageSource PinkBody = LoadImage("PinkBody.png");
        public readonly static ImageSource BlueBody = LoadImage("BlueBody.png");
        public readonly static ImageSource YellowBody = LoadImage("YellowBody.png");
        public static ImageSource CBody = Body;
        public static ImageSource Head = LoadImage("Head.png");
        public static ImageSource PinkHead = LoadImage("PinkHead.png");
        public static ImageSource BlueHead = LoadImage("BlueHead.png");
        public static ImageSource YellowHead = LoadImage("YellowHead.png");
        public static ImageSource CHead = Head;
        public readonly static ImageSource Food = LoadImage("Food.png");
        public static ImageSource DeadBody = LoadImage("DeadBody.png");
        public static ImageSource PinkDeadBody = LoadImage("PinkDeadBody.png");
        public static ImageSource BlueDeadBody = LoadImage("BlueDeadBody.png");
        public static ImageSource YellowDeadBody = LoadImage("YellowDeadBody.png");
        public static ImageSource CDeadBody = DeadBody;
        public static ImageSource DeadHead = LoadImage("DeadHead.png");
        public static ImageSource PinkDeadHead = LoadImage("PinkDeadHead.png");
        public static ImageSource BlueDeadHead = LoadImage("BlueDeadHead.png");
        public static ImageSource YellowDeadHead = LoadImage("YellowDeadHead.png");
        public static ImageSource CDeadHead = DeadHead;
        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/{filename}", UriKind.Relative));
        }

    }
}
