using System.Text;

namespace FifteenPuzzle
{
    public class Field
    {
        private int width;
        private int height;
        private int x0;
        private int y0;
        private int[,] array;

        public Field(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            string[] dimensions = lines[0].Split(' ');

            width = int.Parse(dimensions[0]);
            height = int.Parse(dimensions[1]);
            array = new int[width, height];

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(' ');

                for (int j = 0; j < values.Length; j++)
                {
                    int value = int.Parse(values[j]);
                    Set(j, i - 1, value);
                }
            }
        }

        public Field(int width, int height)
        {
            this.width = width;
            this.height = height;
            array = new int[width, height];
        }

        public void Set(int x, int y, int value)
        {
            if (value == 0)
            {
                x0 = x;
                y0 = y;
            }
            array[x, y] = value;
        }

        public int GetCell(int x, int y)
        {
            return array[x, y];
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public bool Move(char direction)
        {
            direction = Char.ToUpper(direction);
            int temp;
            switch (direction)
            {
                case 'U':
                    if (y0 == 0) return false;
                    temp = array[x0, y0 - 1];
                    array[x0, y0] = temp;
                    array[x0, y0 - 1] = 0;
                    y0--;
                    return true;
                case 'D':
                    if (y0 == height - 1) return false;
                    temp = array[x0, y0 + 1];
                    array[x0, y0] = temp;
                    array[x0, y0 + 1] = 0;
                    y0++;
                    return true;
                case 'L':
                    if (x0 == 0) return false;
                    temp = array[x0 - 1, y0];
                    array[x0, y0] = temp;
                    array[x0 - 1, y0] = 0;
                    x0--;
                    return true;
                case 'R':
                    if (x0 == width - 1) return false;
                    temp = array[x0 + 1, y0];
                    array[x0, y0] = temp;
                    array[x0 + 1, y0] = 0;
                    x0++;
                    return true;

            }
            return false;
        }

        public bool IsSolved()
        {
            int correctValue = 1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == height - 1 && j == width - 1) correctValue = 0;
                    if (array[j, i] != correctValue) return false;
                    correctValue++;
                }
            }
            return true;
        }

        public int GetHash()
        {
            int hash = 17;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    hash = hash * 31 + array[i, j];
                }
            }
            return hash;
        }

        public Field Clone()
        {
            Field temp = new(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    temp.Set(i, j, array[i, j]);
                }
            }
            return temp;
        }

        public Field GenerateSolvedField()
        {
            Field temp = new(width, height);
            int value = 1;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    temp.Set(j, i, value);
                    value++;
                }
            }
            temp.Set(height - 1, width - 1, 0);

            return temp;
        }
    }
}
