namespace Extensions
{
    public static class MatrixExtension
    {
        public static int[,] CalcMatrix(this string array)
        {
            var line = array.Split('\n');
            var elem = line[0].Split(',');
            var y = line.Length;
            var x = elem.Length;
            int[,] matrix = new int[x, y];

            for (int j = 0; j < y; j++)
            {
                elem = line[y - 1 - j].Split(',');
                for (int i = 0; i < x; i++)
                {
                    matrix[i, j] = int.Parse(elem[i]);
                }
            }
            return matrix;
        }

        public static int[,] CreateBorder(this int[,] matrix)
        {
            var x = matrix.GetLength(0);
            var y = matrix.GetLength(1);
            var border = new int[x+2, y+2];
            for (int i = 0; i < x+2; i++)
            {
                for (int j = 0;j < y+2; j++)
                {
                    border[i, j] = 0;
                }
            }


            for(int i = 0; i < x; i++)
            {
                for(int j = 0; j < y; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        border[i+1, j+1] = 1;
                        border[i, j+1] = 1;
                        border[i+1, j] = 1;
                        border[i+1, j+2] = 1;
                        border[i+2, j+1] = 1;
                    }
                }
            }
            return border;
        }
    }
}