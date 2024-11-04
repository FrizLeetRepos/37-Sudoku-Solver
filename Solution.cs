public class Solution {
    public bool SolveSudoku(char[][] board) {

        for (int row = 0; row < board.Length; row++) {
            for (int col = 0; col < board[row].Length; col++) {
                if (board[row][col] == '.') {
                    for (int i = 1; i <= 9; i++) {
                        if (CheckRow(row, col, board, i) &&
                            CheckCol(row, col, board, i) &&
                            CheckLocalBox(row, col, board, i)) {
                            board[row][col] = (char)(i + 48);
                            if (!SolveSudoku(board)) {
                                board[row][col] = '.';
                            } else {
                                return true;
                            }
                        }
                        if(i == 9)
                            return false;
                    }
                }
            }
        }
        return true;
    }

    private bool CheckRow(int row, int col, char[][] board, int num) {
        for (int j = 0; j < board[row].Length; j++) {
            if (j == col)
                continue;
            if (board[row][j] - 48 == num)
                return false;
        }
        return true;
    }
    private bool CheckCol(int row, int col, char[][] board, int num) {
        for (int i = 0; i < board.Length; i++) {
            if (i == row)
                continue;
            if (board[i][col] - 48 == num)
                return false;
        }
        return true;
    }
    private bool CheckLocalBox(int row, int col, char[][] board, int num) {
        int boxRow = row / 3;
        int boxCol = col / 3;

        int boxNum = (boxRow * 3) + boxCol;
        for (int i = boxRow * 3; i < (boxRow * 3) + 3; i++) {
            for (int j = boxCol * 3; j < (boxCol * 3) + 3; j++) {
                if (i == row && j == col)
                    continue;
                if (board[i][j] - 48 == num)
                    return false;
            }
        }
        return true;
    }
}