namespace ConsoleApplication1
{
    using System;

    internal class Dot
    {
        public int x;
        public int y;
        public int color;

        public Dot(string line)
        {
            string[] strArray = line.Split(new char[] { ',' });
            this.x = int.Parse(strArray[0]);
            this.y = int.Parse(strArray[1]);
            if (strArray[2].Equals("white"))
            {
                this.color = 0xffffff;
            }
            else if (strArray[2].Equals("black"))
            {
                this.color = 0;
            }
            else
            {
                this.color = int.Parse(strArray[2]);
            }
        }
    }
}
