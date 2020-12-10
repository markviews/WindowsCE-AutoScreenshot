namespace Project1
{
    using System;

    internal class Dot
    {
        public int x;
        public int y;
        public int color;

        public Dot(string line)
        {
            string[] separator = new string[] { "," };
            string[] strArray2 = line.Split(separator, Convert.ToChar(5), StringSplitOptions.None);
            this.x = int.Parse(strArray2[0]);
            this.y = int.Parse(strArray2[1]);
            if (strArray2[2].Equals("white"))
            {
                this.color = 0xffffff;
            }
            else if (strArray2[2].Equals("black"))
            {
                this.color = 0;
            }
            else
            {
                this.color = int.Parse(strArray2[2]);
            }
        }
    }
}
