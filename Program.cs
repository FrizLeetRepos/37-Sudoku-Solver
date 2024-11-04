namespace _37_Sudoku_Solver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[][] board = [['.', '.', '.', '2', '.', '.', '.', '6', '3'],
                            ['3', '.', '.', '.', '.', '5', '4', '.', '1'],
                            ['.', '.', '1', '.', '.', '3', '9', '8', '.'],
                            ['.', '.', '.', '.', '.', '.', '.', '9', '.'],
                            ['.', '.', '.', '5', '3', '8', '.', '.', '.'],
                            ['.', '3', '.', '.', '.', '.', '.', '.', '.'],
                            ['.', '2', '6', '3', '.', '.', '5', '.', '.'],
                            ['5', '.', '3', '7', '.', '.', '.', '.', '8'],
                            ['4', '7', '.', '.', '.', '1', '.', '.', '.']];
            DisplayBoard(board);
            
            var solution = new Solution();
            solution.SolveSudoku(board);
            DisplayBoard(board);
        }

        private static void DisplayBoard(char[][] board)
        {
            foreach (var row in board)
            {
                foreach (var col in row)
                {
                    Console.Write(col + "|");
                }
                Console.WriteLine();
            }
        }
    }
}
