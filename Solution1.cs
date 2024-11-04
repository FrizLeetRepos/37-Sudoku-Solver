public class Solution1 {

    private int _itterationCount = 0;
    private int _unfilledCount = 0;//9*9
    private int _noProgressCount = 0;

    private int[,] _board = new int[9, 9];
    private List<int>[,] _options = new List<int>[9, 9];

    private int _testingTheory = 0;
    private bool _unwindTest = false;
    private int _testStartRow = -1;
    private int _testStartCol = -1;
    private List<int> _alreadyTestedStartRow = new List<int>();
    private List<int> _alreadyTestedStartCol = new List<int>();
    private int _backupUnfilledCount;
    private int[,] _backupBoard = new int[9, 9];
    private List<int>[,] _backupOptions = new List<int>[9, 9];

    public void SolveSudoku(char[][] board) {
        _unfilledCount = 81;
        for (int row = 0; row < board.Length; row++) {
            for (int col = 0; col < board[row].Length; col++) {
                if (board[row][col] != '.') {
                    _unfilledCount--;
                }
            }
        }
        ConvertBoardCharsToNums(ref _board, board, _board.GetLength(0), _board.GetLength(1));

        _options = GetOptions(_board);

        _itterationCount = 0;
        _noProgressCount = 0;
        while (_unfilledCount > 0 && _itterationCount < 1000) {
            int lastunfilledCount = _unfilledCount;

            //single option algo
            bool singleOptionUsed = false;
            for (int row = 0; row < _board.GetLength(0); row++) {
                for (int col = 0; col < _board.GetLength(1); col++) {
                    if (_options[row, col].Count == 1) {
                        SetCellValue(row, col, _options[row, col][0]);
                        singleOptionUsed = true;
                        if (_unfilledCount == 0)
                            break;
                    }
                }
                if (singleOptionUsed || _unfilledCount == 0)
                    break;
            }
            if (singleOptionUsed) {
                singleOptionUsed = false;
                _itterationCount++;
                _noProgressCount = 0;
                continue;
            }
            if (_unfilledCount == 0)
                break;

            //exclusive option in row, col, or box algo
            bool exclusiveOption = true;
            bool exclusiveOptionUsed = false;
            for (int row = 0; row < _board.GetLength(0); row++) {
                for (int col = 0; col < _board.GetLength(1); col++) {
                    foreach (var curCellOption in _options[row, col]) {
                        if (exclusiveOption = IsOptionExclusiveOnRow(row, col, curCellOption)) {
                            exclusiveOptionUsed = true;
                            break;
                        }
                        if (exclusiveOption = IsOptionExclusiveOnCol(row, col, curCellOption)) {
                            exclusiveOptionUsed = true;
                            break;
                        }
                        if (exclusiveOption = IsOptionExclusiveOnBox(row, col, curCellOption)) {
                            exclusiveOptionUsed = true;
                            break;
                        }
                    }
                    if (exclusiveOptionUsed)
                        break;
                }
                if (exclusiveOptionUsed)
                    break;
            }
            if (exclusiveOptionUsed) {
                exclusiveOptionUsed = false;
                _itterationCount++;
                _noProgressCount = 0;
                continue;
            }

            //multi option guess algo
            if (_testingTheory == 0) {
                Array.Copy(_board, _backupBoard, _board.Length);
                _backupOptions = Copy(_options);
                _backupUnfilledCount = _unfilledCount;
            } else if (_unwindTest) {
                _testingTheory--;
                _unwindTest = false;
                _alreadyTestedStartRow.Add(_testStartRow);
                _alreadyTestedStartCol.Add(_testStartCol);
                _testStartRow = -1;
                _testStartCol = -1;
                Array.Copy(_backupBoard, _board, _backupBoard.Length);
                _options = Copy(_backupOptions);
                _unfilledCount = _backupUnfilledCount;
            }

            bool optionMatchFound = false;
            for (int row = 0; row < _board.GetLength(0); row++) {
                for (int col = 0; col < _board.GetLength(1); col++) {
                    bool alreadyTested = false;
                    for (int i = 0; i < _alreadyTestedStartRow.Count; i++) {
                        if (_alreadyTestedStartRow[i] == row && _alreadyTestedStartCol[i] == col) {
                            alreadyTested = true;
                            break;
                        }
                    }
                    if (alreadyTested)
                        continue;

                    if (_options[row, col].Count == 2) {
                        if (optionMatchFound = CheckRowForOptionMatch(row, col)) {
                            _testingTheory++;
                            _testStartRow = row;
                            _testStartCol = col;
                            break;
                        }
                        if (optionMatchFound = CheckColForOptionMatch(row, col)) {
                            _testingTheory++;
                            _testStartRow = row;
                            _testStartCol = col;
                            break;
                        }
                        if (optionMatchFound = CheckBoxForOptionMatch(row, col)) {
                            _testingTheory++;
                            _testStartRow = row;
                            _testStartCol = col;
                            break;
                        }
                    }
                }
                if (optionMatchFound) {
                    break;
                }
            }

            _itterationCount++;
            if (lastunfilledCount == _unfilledCount) {
                _noProgressCount++;
                if (_noProgressCount > 1) {
                    if (_testingTheory > 0) {
                        _noProgressCount = 0;
                        _unwindTest = true;
                        //break;//for testing
                    } else {
                        break;
                    }
                }
            } else {
                _noProgressCount = 0;
            }
        }

        ConvertBoardNumsToChars(ref board, _board, _board.GetLength(0), _board.GetLength(1));


    }
    public static List<int>[,] Copy(List<int>[,] array) {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        List<int>[,] copy = new List<int>[width, height];

        for (int w = 0; w < width; w++) {
            for (int h = 0; h < height; h++) {
                copy[w, h] = array[w, h].ToArray().ToList();
            }
        }

        return copy;
    }
    private bool CheckRowForOptionMatch(int row, int col) {
        bool potentialOptionsMatch = false;
        bool optionMatchFound = false;
        for (int j = 0; j < _options.GetLength(1); j++) {
            if (j == col)
                continue;

            foreach (var curCellOption in _options[row, col]) {
                potentialOptionsMatch = false;
                foreach (var option in _options[row, j]) {
                    if (curCellOption == option) {
                        potentialOptionsMatch = true;
                        break;
                    }
                }
                if (!potentialOptionsMatch)
                    break;
            }
            if (potentialOptionsMatch && _options[row, col].Count == _options[row, j].Count) {
                optionMatchFound = true;

                SetCellValue(row, col, _options[row, col][0]);
                SetCellValue(row, j, _options[row, j][0]);
                break;
            }
        }

        return optionMatchFound;
    }

    private bool CheckColForOptionMatch(int row, int col) {
        bool potentialOptionsMatch = false;
        bool optionMatchFound = false;
        for (int i = 0; i < _options.GetLength(0); i++) {
            if (i == row)
                continue;

            foreach (var curCellOption in _options[row, col]) {
                potentialOptionsMatch = false;
                foreach (var option in _options[i, col]) {
                    if (curCellOption == option) {
                        potentialOptionsMatch = true;
                        break;
                    }
                }
                if (!potentialOptionsMatch)
                    break;
            }
            if (potentialOptionsMatch && _options[row, col].Count == _options[i, col].Count) {
                optionMatchFound = true;

                SetCellValue(row, col, _options[row, col][0]);
                SetCellValue(i, col, _options[i, col][0]);
                break;
            }
        }

        return optionMatchFound;
    }

    private bool CheckBoxForOptionMatch(int row, int col) {
        bool potentialOptionsMatch = false;
        bool optionMatchFound = false;
        int boxNum = GetBoxId(row, col);
        for (int i = _boxLimits[boxNum].RowLowerLimit; i < _boxLimits[boxNum].RowUpperLimit; i++) {
            for (int j = _boxLimits[boxNum].ColLowerLimit; j < _boxLimits[boxNum].ColUpperLimit; j++) {
                if (i == row && j == col)
                    continue;

                foreach (var curCellOption in _options[row, col]) {
                    potentialOptionsMatch = false;
                    foreach (var option in _options[i, j]) {
                        if (curCellOption == option) {
                            potentialOptionsMatch = true;
                            break;
                        }
                    }
                    if (!potentialOptionsMatch)
                        break;
                }
                if (potentialOptionsMatch && _options[row, col].Count == _options[i, j].Count) {
                    optionMatchFound = true;

                    SetCellValue(row, col, _options[row, col][0]);
                    SetCellValue(i, j, _options[i, j][0]);
                    break;
                }
            }
            if (optionMatchFound)
                break;
        }

        return optionMatchFound;
    }

    private bool IsOptionExclusiveOnRow(int row, int col, int curCellOption) {
        bool exclusiveOption = true;
        for (int j = 0; j < _options.GetLength(1); j++) {
            if (j == col)
                continue;

            foreach (var option in _options[row, j]) {
                if (curCellOption == option) {
                    exclusiveOption = false;
                    break;
                }
            }
            if (!exclusiveOption)
                break;
        }
        if (exclusiveOption)
            SetCellValue(row, col, curCellOption);

        return exclusiveOption;
    }

    private bool IsOptionExclusiveOnCol(int row, int col, int curCellOption) {
        bool exclusiveOption = true;
        for (int i = 0; i < _options.GetLength(0); i++) {
            if (i == row)
                continue;

            foreach (var option in _options[i, col]) {
                if (curCellOption == option) {
                    exclusiveOption = false;
                    break;
                }
            }
            if (!exclusiveOption)
                break;
        }
        if (exclusiveOption)
            SetCellValue(row, col, curCellOption);

        return exclusiveOption;
    }

    private bool IsOptionExclusiveOnBox(int row, int col, int curCellOption) {
        bool exclusiveOption = true;
        int boxNum = GetBoxId(row, col);
        for (int i = _boxLimits[boxNum].RowLowerLimit; i < _boxLimits[boxNum].RowUpperLimit; i++) {
            for (int j = _boxLimits[boxNum].ColLowerLimit; j < _boxLimits[boxNum].ColUpperLimit; j++) {
                if (i == row && j == col)
                    continue;

                foreach (var option in _options[i, j]) {
                    if (curCellOption == option) {
                        exclusiveOption = false;
                        break;
                    }
                }
                if (!exclusiveOption)
                    break;
            }
            if (!exclusiveOption)
                break;
        }
        if (exclusiveOption)
            SetCellValue(row, col, curCellOption);

        return exclusiveOption;
    }

    private void SetCellValue(int row, int col, int value) {
        _board[row, col] = value;
        _unfilledCount--;
        RemoveValueFromOptions(ref _options, row, col, _board[row, col]);
        _options[row, col].Clear();
    }

    public void ConvertBoardCharsToNums(ref int[,] destBoard, char[][] sourceBoard, int rowUpperLimit = 9, int colUpperLimit = 9) {
        for (int row = 0; row < rowUpperLimit; row++) {
            for (int col = 0; col < colUpperLimit; col++) {
                if (sourceBoard[row][col] == '.') {
                    destBoard[row, col] = 0;
                } else {
                    destBoard[row, col] = sourceBoard[row][col] - 48;
                }
            }
        }
    }

    public void ConvertBoardNumsToChars(ref char[][] destBoard, int[,] sourceBoard, int rowUpperLimit = 9, int colUpperLimit = 9) {
        for (int row = 0; row < rowUpperLimit; row++) {
            for (int col = 0; col < colUpperLimit; col++) {
                if (sourceBoard[row, col] == 0) {
                    destBoard[row][col] = '.';
                } else {
                    destBoard[row][col] = (char)(sourceBoard[row, col] + 48);
                }
            }
        }
    }

    private void RemoveValueFromOptions(ref List<int>[,] options, int row, int col, int value) {

        //check row
        for (int i = 0; i < options.GetLength(1); i++) {
            options[row, i].Remove(value);
        }
        //check col
        for (int i = 0; i < options.GetLength(0); i++) {
            options[i, col].Remove(value);
        }
        //check box
        int boxNum = GetBoxId(row, col);
        for (int i = _boxLimits[boxNum].RowLowerLimit; i < _boxLimits[boxNum].RowUpperLimit; i++) {
            for (int j = _boxLimits[boxNum].ColLowerLimit; j < _boxLimits[boxNum].ColUpperLimit; j++) {
                options[i, j].Remove(value);
            }
        }
    }

    public List<int>[,] GetOptions(int[,] board) {
        List<int>[,] options = new List<int>[9, 9];
        for (int row = 0; row < board.GetLength(0); row++) {
            for (int col = 0; col < board.GetLength(1); col++) {
                options[row, col] = new List<int>();
                if (board[row, col] == 0) {

                    for (int i = 1; i <= 9; i++) {
                        if (CheckRow(row, board, i) && CheckCol(col, board, i) && CheckLocalBox(row, col, board, i)) {
                            options[row, col].Add(i);
                        }
                    }
                }
            }
        }
        return options;
    }

    private bool CheckRow(int row, int[,] board, int num) {
        for (int i = 0; i < board.GetLength(1); i++) {
            if (board[row, i] == num) return false;
        }
        return true;
    }

    private bool CheckCol(int col, int[,] board, int num) {
        for (int i = 0; i < board.GetLength(0); i++) {
            if (board[i, col] == num) return false;
        }
        return true;
    }

    private class BoxLimits {
        public int RowLowerLimit { get; set; }
        public int RowUpperLimit { get; set; }
        public int ColLowerLimit { get; set; }
        public int ColUpperLimit { get; set; }

        public BoxLimits(int rowLowerLimit, int rowUpperLimit, int colLowerLimit, int colUpperLimit) {
            RowLowerLimit = rowLowerLimit;
            RowUpperLimit = rowUpperLimit;
            ColLowerLimit = colLowerLimit;
            ColUpperLimit = colUpperLimit;
        }
    }

    private List<BoxLimits> _boxLimits = new List<BoxLimits>() {
                                                        new(0,3,0,3),
                                                        new(3,6,0,3),
                                                        new(6,9,0,3),
                                                        new(0,3,3,6),
                                                        new(3,6,3,6),
                                                        new(6,9,3,6),
                                                        new(0,3,6,9),
                                                        new(3,6,6,9),
                                                        new(6,9,6,9),
        };
    private int GetBoxId(int row, int col) {
        int boxNum = 0;
        foreach (var box in _boxLimits) {
            if (row >= box.RowLowerLimit && row < box.RowUpperLimit && col >= box.ColLowerLimit && col < box.ColUpperLimit) {
                break;
            }
            boxNum++;
        }
        return boxNum;
    }
    private bool CheckLocalBox(int row, int col, int[,] board, int num) {
        int boxNum = GetBoxId(row, col);
        for (int i = _boxLimits[boxNum].RowLowerLimit; i < _boxLimits[boxNum].RowUpperLimit; i++) {
            for (int j = _boxLimits[boxNum].ColLowerLimit; j < _boxLimits[boxNum].ColUpperLimit; j++) {
                if (board[i, j] == num) return false;
            }
        }
        return true;
    }
}
