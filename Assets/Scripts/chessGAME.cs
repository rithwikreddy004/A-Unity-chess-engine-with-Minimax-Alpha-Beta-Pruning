using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using TMPro;




public class ChessRules : MonoBehaviour
{

    public int[] board = new int[64];
    public int[] cb = new int[64];
    public List<int[]> boardHistory = new List<int[]>();
    public ChessGUI chessGUI;
    public int currentStateIndex = 0;
    //public Button undoButton;
    //public Button redoButton;
    public GameObject arrowPrefab;
    public bool isWhitePlayer;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameResultText;

    public TextMeshProUGUI mode;
    //public GameObject arrowHeadPrefab;
    void Start()
    {

        //UnityEngine.Debug.Log("startMoveIndex=" + moveIndex);
        isWhitePlayer = PlayerPrefs.GetInt("IsWhitePlayer", 1) == 1;
        isWhitePlayer = !isWhitePlayer;
        UnityEngine.Debug.Log("is white:" + isWhitePlayer);
        //undoButton.GetComponentInChildren<TMP_Text>().text = "Undo";
        //redoButton.GetComponentInChildren<TMP_Text>().text = "Redo";


        InitializeBoard();
        chessGUI.UpdateBoard(board);
        //chessGUI.UpdateBoardB(board,false);

        //gen();

        //int c = CountLegalMoves(0);
        //UnityEngine.Debug.Log("count: " + c);

        //gen();
        // UnityEngine.Debug.Log("white moves: " + string.Join(", ", WF));
        // UnityEngine.Debug.Log("count: " + string.Join(", ", WF.Count/2));

        //UnityEngine.Debug.Log("black moves: " + string.Join(", ", BF));
        //UnityEngine.Debug.Log("count: " + string.Join(", ", BF.Count/2));

        //WF =MoveOrder(WF);
        //BF = MoveOrder(BF,globalBestSource,globalBestTarget);


        //UnityEngine.Debug.Log("move order white moves: " + string.Join(", ", WF));
        //UnityEngine.Debug.Log("count: " + string.Join(", ", WF.Count/2));


        //UnityEngine.Debug.Log("move order black moves: " + string.Join(", ", BF));
        //UnityEngine.Debug.Log("count: " + string.Join(", ", BF.Count/2));
        //Genw(board);
        //gen2();


        //undoButton.onClick.AddListener(() =>UndoMove());
        // redoButton.onClick.AddListener(() => RedoMove());
        SaveState();
        //SaveInitialState();

        // HighlightBestMove(1, 18,60);
        //HighlightBestMove(12, 28);
        //Analysis(true);
        gen();

       
        
       // Analysis(!isWhitePlayer);
        

    }
    public bool analysis = false;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UndoMove();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            RedoMove();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            analysis=true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            analysis = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("menu");  // Change "MainMenu" to your actual main scene name
        }
        DisplayScore();
        UpdateModeText();
        
    }


    /*
     0, 7, 0, 0, 0, 0, 0, 1,
     1, 0, 7, 0, 0, 0, 0, 0,
     0, 0, 0, 0, 0, 0, 0, 0,
     0, 0, 0, 1, 0, 0, 0, 7,
     0, 0, 0, 0, 7, 0, 0, 0,
     0, 0, 0, 0, 0, 0, 0, 0,
     1, 0, 0, 1, 0, 0, 0, 0,
     0, 0, 0, 0, 0, 0, 7, 0

     */


    public void SaveState()
    {
        // If we've undone moves and then a new move is made,
        // remove all states ahead of the current one (clear redo history)
        if (currentStateIndex < boardHistory.Count - 1)
        {
            boardHistory.RemoveRange(currentStateIndex + 1, boardHistory.Count - currentStateIndex - 1);
        }

        // Clone the current board array so that later changes don't affect saved state
        int[] boardState = new int[64];
        System.Array.Copy(board, boardState, board.Length);
        boardHistory.Add(boardState);

        // Update the pointer to point to the latest state
        currentStateIndex = boardHistory.Count - 1;
        UnityEngine.Debug.Log("State saved. CurrentStateIndex: " + currentStateIndex +
                  ", Total States: " + boardHistory.Count);
    }


    public void UpdateModeText()
    {
        // Check if the boolean is false or true
        if (analysis)
        {
            mode.text = "Analysis Mode: ON"; // Set the text to ON if true
        }
        else
        {
            mode.text = "Analysis Mode: OFF"; // Set the text to OFF if false
        }
    }


    public void UndoMove()
    {
        if (currentStateIndex > 0)
        {
            currentStateIndex--; // Go to the previous state
            int[] previousState = boardHistory[currentStateIndex];
            System.Array.Copy(previousState, board, board.Length);
            chessGUI.UpdateBoard(board);
            UnityEngine.Debug.Log("Undo performed. New index: " + currentStateIndex);
        }
        else
        {
            UnityEngine.Debug.Log("No moves to undo!");
        }
    }

    

    public void RedoMove()
    {
        if (currentStateIndex < boardHistory.Count-1)
        {
            currentStateIndex++; // Move forward to the next state
            int[] nextState = boardHistory[currentStateIndex];
            System.Array.Copy(nextState, board, board.Length);
            chessGUI.UpdateBoard(board);
            UnityEngine.Debug.Log("Redo performed. New index: " + currentStateIndex);
        }

        else
        {

            if (currentStateIndex == boardHistory.Count - 1)
            {
                //currentStateIndex++; // Move forward to the next state
                int[] nextState = boardHistory[currentStateIndex];
                System.Array.Copy(nextState, board, board.Length);
                chessGUI.UpdateBoard(board);
                UnityEngine.Debug.Log("Redo performed. New index: " + currentStateIndex);
            }
            else
            {
                UnityEngine.Debug.Log("No moves to redo!");
            }
            
        }
    }





    /*public void Setcb()
    {
        cb[56] = 8; cb[57] = 0; cb[58] = 10; cb[59] = 0; cb[60] = 0; cb[61] = 12; cb[62] = 8; cb[63] = 0;

        cb[48] = 7; cb[49] = 7; cb[50] = 0; cb[51] = 7; cb[52] = 0; cb[53] = 0; cb[54] = 0; cb[55] = 5;

        cb[40] = 9; cb[41] = 0; cb[42] = 0; cb[43] = 0; cb[44] = 0; cb[45] = 4; cb[46] = 0; cb[47] = 0;

        cb[32] = 11; cb[33] = 0; cb[34] = 7; cb[35] = 9; cb[36] = 0; cb[37] = 0; cb[38] = 0; cb[39] = 0;

        cb[24] = 0; cb[25] = 0; cb[26] = 0; cb[27] = 0; cb[28] = 0; cb[29] = 0; cb[30] = 0; cb[31] = 0;

        cb[16] = 1; cb[17] = 0; cb[18] = 0; cb[19] = 0; cb[20] = 0; cb[21] = 0; cb[22] = 0; cb[23] = 0;

        cb[8] = 0; cb[9] = 0; cb[10] = 1; cb[11] = 3; cb[12] = 0; cb[13] = 1; cb[14] = 1; cb[15] = 1;

        cb[0] = 0; cb[1] = 2; cb[2] = 0; cb[3] = 0; cb[4] = 6; cb[5] = 4; cb[6] = 0; cb[7] = 2;

    }*/



     public void Setcb()
     {
         cb[56] = 8; cb[57] = 12; cb[58] = 8; cb[59] = 9; cb[60] = 11; cb[61] = 10; cb[62] = 10; cb[63] = 9;

         cb[48] = 9; cb[49] = 9; cb[50] = 9; cb[51] = 9; cb[52] = 9; cb[53] = 9; cb[54] = 9; cb[55] = 9;

         cb[40] = 0; cb[41] = 0; cb[42] = 0; cb[43] = 0; cb[44] = 0; cb[45] = 0; cb[46] = 0; cb[47] = 0;

         cb[32] = 0; cb[33] = 0; cb[34] = 0; cb[35] = 0; cb[36] = 0; cb[37] = 0; cb[38] = 0; cb[39] = 0;

         cb[24] = 0; cb[25] = 0; cb[26] = 0; cb[27] = 0; cb[28] = 0; cb[29] = 0; cb[30] = 0; cb[31] = 0;

         cb[16] = 0; cb[17] = 0; cb[18] = 0; cb[19] = 0; cb[20] = 0; cb[21] = 0; cb[22] = 0; cb[23] = 0;

         cb[8] = 3; cb[9] = 3; cb[10] = 3; cb[11] = 3; cb[12] = 3; cb[13] = 3; cb[14] = 3; cb[15] = 3;

         cb[0] = 2; cb[1] = 6; cb[2] = 2; cb[3] = 3; cb[4] = 5; cb[5] = 4; cb[6] = 4; cb[7] = 3;

     }


    /*public void Setcb()
    {
        cb[56] = 0; cb[57] = 0; cb[58] = 0; cb[59] = 0; cb[60] = 0; cb[61] = 0; cb[62] = 0; cb[63] = 0;

        cb[48] = 0; cb[49] = 0; cb[50] = 2; cb[51] = 0; cb[52] = 0; cb[53] = 9; cb[54] = 7; cb[55] = 7;

        cb[40] = 0; cb[41] = 5; cb[42] = 0; cb[43] = 0; cb[44] = 8; cb[45] = 12; cb[46] = 0; cb[47] = 0;

        cb[32] = 0; cb[33] = 0; cb[34] = 3; cb[35] = 0; cb[36] = 0; cb[37] = 11; cb[38] = 0; cb[39] = 0;

        cb[24] = 0; cb[25] = 0; cb[26] = 0; cb[27] = 0; cb[28] = 0; cb[29] = 1; cb[30] = 0; cb[31] = 0;

        cb[16] = 0; cb[17] = 0; cb[18] = 0; cb[19] = 1; cb[20] = 0; cb[21] = 0; cb[22] = 0; cb[23] = 0;

        cb[8] = 6; cb[9] = 1; cb[10] = 0; cb[11] = 0; cb[12] = 0; cb[13] = 0; cb[14] = 4; cb[15] = 0;

        cb[0] = 0; cb[1] = 0; cb[2] = 0; cb[3] = 0; cb[4] = 0; cb[5] = 0; cb[6] = 0; cb[7] = 0;

    }*/
    public void SetBoardState(int[] newBoardState)
    {
        UnityEngine.Debug.Log("Board state length: " + newBoardState.Length);


        for (int i = 0; i < 64; i++)
        {
            board[i] = newBoardState[i];
        }
    }



    public void InitializeBoard()
    {

        for (int i = 0; i < 64; i++)
        {
            board[i] = 0;
        }

        // White pieces (using indices for simplicity)
        for (int i = 8; i < 16; i++) board[i] = 1; // White Pawns
        board[0] = board[7] = 2; // White Rooks
        board[1] = board[6] = 3; // White Knights
        //board[1] = 3;
        //board[6] = 0;
        board[2] = board[5] = 4; // White Bishops
        board[3] = 5;            // White Queen
        board[4] = 6;            // White King

        // Black pieces
        for (int i = 48; i < 56; i++) board[i] = 7; // Black Pawns
        board[56] = board[63] = 8;  // Black Rooks
        board[57] = board[62] = 9;  // Black Knights
        board[58] = board[61] = 10; // Black Bishops
        board[59] = 11;             // Black Queen
        board[60] = 12;             // Black King

        // Update the visual board
        //Setcb();
        //SetBoardState(cb);

        chessGUI.UpdateBoard(board);
        //chessGUI.UpdateBoardB(board,false);

        InitializeZobrist();
        //ulong hash = ComputeHash(board);




        //Debug.Log($" 1st Zobrist Hash: {hash}");

        if (isWhitePlayer)
        {
            Rando(isWhitePlayer);
        }
        //Rando(false);
    }








    public void MovePiece(Vector3 startPosition, Vector3 endPosition)
    {



        int startIdx = GetBoardIndexFromPosition(startPosition);
        int endIdx = GetBoardIndexFromPosition(endPosition);


        enPassantTarget = GetEnPassantTarget(startIdx, endIdx, board);

        int piece = board[startIdx];


        board[startIdx] = 0;
        board[endIdx] = piece;





        if ((isWhite(piece) && endIdx >= 56 && piece == 1) || (!isWhite(piece) && endIdx < 8 && piece == 7))
        {

            PromotePawn(endIdx, isWhite(piece));
        }



        chessGUI.UpdateBoard(board);
        //ulong hash = ComputeHash(board);
        //Debug.Log($"Zobrist Hash: {hash}");








        //PrintTranspositionTable();
        //Rando(false);
        //chessGUI.count += 2;

    }



    public void MoveC(int a, int b, int c, int d)
    {


        int piece1 = board[a];
        int piece2 = board[c];

        board[a] = 0;
        board[b] = piece1;


        board[c] = 0;
        board[d] = piece2;

        chessGUI.UpdateBoard(board);
    }




    public void MovePieceI(int startPosition, int endPosition)
    {
        
        int piece = board[startPosition];


        board[startPosition] = 0;
        board[endPosition] = piece;





        

        chessGUI.UpdateBoard(board);
        

    }

    /*public async Task  MovePieceIG(int startPosition, int endPosition)
    {

        DestroyAllArrows();

        SaveState();
        int piece = board[startPosition];


        board[startPosition] = 0;
        board[endPosition] = piece;



        chessGUI.UpdateBoard(board);
        await Task.Delay(100);


        Rando(isWhitePlayer);

        //Analysis(true);
    }*/



    public void  MovePieceIG(int startPosition, int endPosition)
    {

        DestroyAllArrows();

        SaveState();
        int piece = board[startPosition];


        board[startPosition] = 0;
        board[endPosition] = piece;



        chessGUI.UpdateBoard(board);
        //await Task.Delay(100);


        Rando(isWhitePlayer);

        //Analysis(true);
    }



    public void MovePieceIB(int startPosition, int endPosition)
    {



        SaveState();
        int piece = board[startPosition];


        board[startPosition] = 0;
        board[endPosition] = piece;







        chessGUI.UpdateBoard(board);
        if (analysis == true)
        {
            Analysis(!isWhitePlayer);
        }


    }








    public void MoveE(int startPosition, int endPosition)
    {



        int piece = board[startPosition];



        board[startPosition] = 0;





        int captureSquare = isWhite(piece) ? endPosition - 8 : endPosition + 8;
        board[captureSquare] = 0;



        board[endPosition] = piece;


        chessGUI.UpdateBoard(board);
    }

    private bool isWhite(int piece)
    {
        return piece >= 1 && piece <= 6;
    }






    int GetBoardIndexFromPosition(Vector3 position)
    {
        float tileSize = 2.0f;
        float boardOffset = (8 * tileSize) / 2;

        int col = Mathf.RoundToInt((position.x + boardOffset) / tileSize);
        int row = Mathf.RoundToInt((position.y + boardOffset) / tileSize);


        return row * 8 + col;
    }


    Vector3 GetPositionFromBoardIndex(int squareIndex)
    {
        float tileSize = 2.0f; // Size of each square
        int col = squareIndex % 8; // Column (0 to 7)
        int row = squareIndex / 8; // Row (0 to 7)

        // Calculate x and y coordinates
        float x = (col - 4) * tileSize; // Shift x based on column
        float y = (row - 4) * tileSize; // Shift y based on row (positive upwards)

        return new Vector3(x, y, 0); // z is 0
    }







    public List<int> wpm = new List<int>();
    public List<int> bpm = new List<int>();




    public int enPassantTarget = -1;
    public List<int> WEP = new List<int>();
    public List<int> BEP = new List<int>();


    
    public void GeneratePawnMoves(int[] board, int pawnIndex, bool isWhite, List<int> possibleMoves)
    {
        //List<int> lm = new List<int>();




        int direction = isWhite ? 1 : -1;
        int startRank = isWhite ? 1 : 6;

        // 1. Normal Move (Pawn moves 1 square forward)
        int forwardIndex = pawnIndex + (direction * 8);
        if (forwardIndex >= 0 && forwardIndex < 64 && board[forwardIndex] == 0)
        {
            possibleMoves.Add(pawnIndex);
            possibleMoves.Add(forwardIndex);
            //lm.Add(pawnIndex);
            //lm.Add(forwardIndex);

        }

        // 2. Double Move (Pawn moves 2 squares forward, only on the first move)
        if ((isWhite && pawnIndex / 8 == startRank) || (!isWhite && pawnIndex / 8 == 6))
        {
            int doubleForwardIndex = pawnIndex + (direction * 16);
            if (doubleForwardIndex >= 0 && doubleForwardIndex < 64 &&
                board[doubleForwardIndex] == 0 && board[forwardIndex] == 0)
            {
                possibleMoves.Add(pawnIndex);
                possibleMoves.Add(doubleForwardIndex);

                //lm.Add(pawnIndex);
                //lm.Add(doubleForwardIndex);


            }
        }

        // 3. Captures (Diagonal moves)
        int[] captureOffsets = { direction * 7, direction * 9 };
        foreach (int offset in captureOffsets)
        {
            int captureIndex = pawnIndex + offset;

            // Ensure the captureIndex is within bounds and doesn't wrap around the board
            if (captureIndex >= 0 && captureIndex < 64 && IsOnSameDiagonal(pawnIndex, captureIndex, offset))
            {
                if (isWhite && board[captureIndex] >= 7 && board[captureIndex] <= 12) // Capture black piece
                {
                    possibleMoves.Add(pawnIndex);
                    possibleMoves.Add(captureIndex);
                    //lm.Add(pawnIndex);
                    //lm.Add(captureIndex);


                }
                else if (!isWhite && board[captureIndex] >= 1 && board[captureIndex] <= 6) // Capture white piece
                {
                    possibleMoves.Add(pawnIndex);
                    possibleMoves.Add(captureIndex);
                    //lm.Add(pawnIndex);
                    //lm.Add(captureIndex);




                }
            }
        }









    }


    public void PromotePawn(int squareIndex, bool isWhite)
    {
        int piece = isWhite ? 5 : 11;

        board[squareIndex] = piece;
        chessGUI.UpdateBoard(board);

        Console.WriteLine("Pawn promotion triggered at square " + squareIndex);

    }








    public void GeneratePawnEP(int[] board, int pawnIndex, bool isWhite, List<int> possibleMoves)
    {
        int direction = isWhite ? 1 : -1;  // Direction depends on whether it's a white or black pawn
        int[] captureOffsets = { direction * 7, direction * 9 };  // Diagonal capture offsets

        if (enPassantTarget != -1) // Ensure there is a valid en passant target
        {
            foreach (int offset in captureOffsets)
            {
                int enPassantIndex = pawnIndex + offset;

                // Ensure the en passant index is within the bounds of the board
                if (enPassantIndex >= 0 && enPassantIndex < 64)
                {
                    // Check if the en passant index matches the target and if it's on the same diagonal
                    if (enPassantIndex == enPassantTarget && IsOnSameDiagonal(pawnIndex, enPassantIndex, offset))
                    {
                        // En passant can only happen if the opponent's pawn just moved two squares forward
                        if ((isWhite && board[enPassantIndex - 8] == 7) || (!isWhite && board[enPassantIndex + 8] == 1))
                        {
                            // White: If enPassantIndex is a black pawn and white is capturing it
                            // Black: If enPassantIndex is a white pawn and black is capturing it
                            possibleMoves.Add(pawnIndex);
                            possibleMoves.Add(enPassantIndex);
                        }
                    }
                }
            }
        }
    }





    public int GetEnPassantTarget(int startSquare, int endSquare, int[] board)
    {
        int piece = board[startSquare]; // The piece being moved
        UnityEngine.Debug.Log(piece);
        // Check if the piece is a pawn (1 for white, 7 for black)
        if (piece == 1 || piece == 7) // Check if it's a white or black pawn
        {
            // Check if this is a double pawn move
            if (Math.Abs(startSquare - endSquare) == 16) // A double move changes the row by 2 (16 squares)
            {
                return (startSquare + endSquare) / 2; // Set en passant target to the square behind the pawn
            }
        }

        // If not a double pawn move, reset en passant target
        return -1;
    }

    // Helper Method to Ensure Moves Don't Wrap Around the Board
    private bool IsOnSameDiagonal(int start, int target, int offset)
    {
        int startCol = start % 8;
        int targetCol = target % 8;

        // Ensure the move stays within one file difference
        return Math.Abs(startCol - targetCol) == 1;
    }




    public void GenerateRookMoves(int[] board, int rookIndex, bool isWhite, List<int> possibleMoves)
    {
        //List<int> lm = new List<int>();



        // Directions for rook movement: up, down, left, right
        int[] directions = { 8, -8, 1, -1 };

        foreach (int direction in directions)
        {
            int currentIndex = rookIndex;

            while (true)
            {
                currentIndex += direction;

                // Ensure the move stays within the board boundaries
                if (currentIndex < 0 || currentIndex >= 64 || !IsOnSameLine(rookIndex, currentIndex, direction))
                    break;

                // If the square is empty, the rook can move there
                if (board[currentIndex] == 0)
                {
                    possibleMoves.Add(rookIndex);
                    possibleMoves.Add(currentIndex);

                    //lm.Add(rookIndex);
                    //lm.Add(currentIndex);


                }
                else
                {
                    // If the square has an opponent's piece, capture it and stop
                    if (isWhite && board[currentIndex] >= 7 && board[currentIndex] <= 12)
                    {
                        possibleMoves.Add(rookIndex);
                        possibleMoves.Add(currentIndex);
                        //lm.Add(rookIndex);
                        //lm.Add(currentIndex);
                    }
                    else if (!isWhite && board[currentIndex] >= 1 && board[currentIndex] <= 6)
                    {
                        possibleMoves.Add(rookIndex);
                        possibleMoves.Add(currentIndex);
                        //lm.Add(rookIndex);
                        //lm.Add(currentIndex);
                    }
                    break; // Stop after capturing
                }
            }
        }




    }

    // Helper Method to Ensure Moves Stay in the Same Line
    private bool IsOnSameLine(int start, int target, int direction)
    {
        // For vertical moves (up/down), no column checks are needed
        if (Math.Abs(direction) == 8)
            return true;

        // For horizontal moves (left/right), ensure the row stays the same
        int startRow = start / 8;
        int targetRow = target / 8;
        return startRow == targetRow;
    }




    public void GenerateBishopMoves(int[] board, int bishopIndex, bool isWhite, List<int> possibleMoves)
    {


        // List<int> lm = new List<int>();
        // Directions for bishop movement: top-left, top-right, bottom-left, bottom-right
        int[] directions = { 9, 7, -9, -7 };

        foreach (int direction in directions)
        {
            int currentIndex = bishopIndex;

            while (true)
            {
                currentIndex += direction;

                // Ensure the move stays within the board boundaries
                if (currentIndex < 0 || currentIndex >= 64 || !SameDiagonal(bishopIndex, currentIndex, direction))
                    break;

                // If the square is empty, the bishop can move there
                if (board[currentIndex] == 0)
                {
                    possibleMoves.Add(bishopIndex);
                    possibleMoves.Add(currentIndex);
                    //lm.Add(bishopIndex);
                    //lm.Add(currentIndex);
                }
                else
                {
                    // If the square has an opponent's piece, capture it and stop
                    if (isWhite && board[currentIndex] >= 7 && board[currentIndex] <= 12)
                    {
                        possibleMoves.Add(bishopIndex);
                        possibleMoves.Add(currentIndex);

                        //lm.Add(bishopIndex);
                        //lm.Add(currentIndex);
                    }
                    else if (!isWhite && board[currentIndex] >= 1 && board[currentIndex] <= 6)
                    {
                        possibleMoves.Add(bishopIndex);
                        possibleMoves.Add(currentIndex);
                        //lm.Add(bishopIndex);
                        //lm.Add(currentIndex);
                    }
                    break; // Stop after capturing
                }
            }
        }





    }

    // Helper Method to Ensure Moves Stay in the Same Diagonal
    private bool SameDiagonal(int start, int target, int direction)
    {
        // For diagonal moves, ensure the distance between rows and columns matches
        int startRow = start / 8;
        int startCol = start % 8;
        int targetRow = target / 8;
        int targetCol = target % 8;

        // Check the change in rows and columns is the same
        return Math.Abs(startRow - targetRow) == Math.Abs(startCol - targetCol);
    }




    public void GenerateQueenMoves(int[] board, int queenIndex, bool isWhite, List<int> possibleMoves)
    {
        // Generate rook moves (horizontal and vertical)
        GenerateRookMoves(board, queenIndex, isWhite, possibleMoves);

        // Generate bishop moves (diagonal)
        GenerateBishopMoves(board, queenIndex, isWhite, possibleMoves);
    }



    public void GenerateKnightMoves(int[] board, int knightIndex, bool isWhite, List<int> possibleMoves)
    {


        //List<int> lm = new List<int>();
        // Possible knight move directions (L-shaped moves)
        int[] moveOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };

        foreach (int offset in moveOffsets)
        {
            int currentIndex = knightIndex + offset;

            // Ensure the move stays within board boundaries (0 to 63)
            if (currentIndex < 0 || currentIndex >= 64)
                continue;

            // Ensure the move doesn't wrap around rows
            if (!IsValidRowTransition(knightIndex, currentIndex))
                continue;

            // Check if the square is empty or contains an opponent's piece
            if (board[currentIndex] == 0 || (isWhite && board[currentIndex] >= 7 && board[currentIndex] <= 12) || (!isWhite && board[currentIndex] >= 1 && board[currentIndex] <= 6))
            {
                // Add the move in the required format: knightIndex and currentIndex
                possibleMoves.Add(knightIndex);
                possibleMoves.Add(currentIndex);

                //lm.Add(knightIndex);
                //lm.Add(currentIndex);
            }
        }



    }

    // Helper method to validate row transitions for knight moves
    private bool IsValidRowTransition(int start, int target)
    {
        int startRow = start / 8;
        int targetRow = target / 8;

        // Valid row transitions for knight moves
        int columnDifference = Math.Abs((start % 8) - (target % 8));

        // Ensure no horizontal wrap-around (e.g., knight jumping from left edge to right edge)
        return columnDifference <= 2 && Math.Abs(startRow - targetRow) <= 2;
    }








    public void GenerateKingMoves(int[] board, int kingIndex, bool isWhite, List<int> possibleMoves)
    {
        //List<int> lm = new List<int>();

        int[] moveOffsets = { -9, -8, -7, -1, 1, 7, 8, 9 };

        foreach (int offset in moveOffsets)
        {
            int currentIndex = kingIndex + offset;

            // Boundary check: Ensure the move stays within the board
            if (currentIndex < 0 || currentIndex >= 64)
                continue;

            // Row wrapping: Prevent the king from moving horizontally across rows
            if (!IsValidKingMove(kingIndex, currentIndex))
                continue;

            // Skip squares occupied by friendly pieces
            if ((isWhite && board[currentIndex] >= 1 && board[currentIndex] <= 6) || (!isWhite && board[currentIndex] >= 7 && board[currentIndex] <= 12))
                continue;

            // Add king's move to the list
            possibleMoves.Add(kingIndex);
            possibleMoves.Add(currentIndex);
            //lm.Add(kingIndex);
            //lm.Add(currentIndex);
        }



    }

    // Helper Method: Validate King Move (No Wrapping Across Rows)
    private bool IsValidKingMove(int start, int target)
    {
        int startRow = start / 8;
        int targetRow = target / 8;

        // The king can move at most 1 square horizontally or vertically
        return Math.Abs((start % 8) - (target % 8)) <= 1 && Math.Abs(startRow - targetRow) <= 1;
    }


    public List<int> WF = new List<int>();
    public List<int> BF = new List<int>();
    public List<int> GameHistory = new List<int>();





    public void Record(int a)
    {
        GameHistory.Add(a);
        //Debug.Log("Game history: " + string.Join(", ",GameHistory));
    }



    public void RemoveMovesThatCauseCheck(bool isWhite)
    {

        Genw(board);
        // List<int> possibleMoves = isWhite ? wpm : bpm;
        List<int> possibleMoves = new List<int>(isWhite ? wpm : bpm);
        List<int> validMoves = new List<int>();


        //int[] boardBackup = new int[board.Length];
        //Array.Copy(board, boardBackup, board.Length);

        // Validate possibleMoves structure


        // Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves));
        // Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves.Count));
        // Iterate through the possibleMoves array in pairs
        //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves));
        //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves.Count));
        for (int i = 0; i < possibleMoves.Count; i += 2)
        {
            // Debug.Log($"i i+1 {i} -> {i+1}");
            //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves));
            //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves.Count));


            int sourceIndex = possibleMoves[i];
            int targetIndex = possibleMoves[i + 1];

            // Validate indices


            //Debug.Log($"Simulating move {sourceIndex} -> {targetIndex}");

            // Simulate the move
            int originalSourcePiece = board[sourceIndex];
            int originalTargetPiece = board[targetIndex];
            board[targetIndex] = board[sourceIndex];
            board[sourceIndex] = 0;

            // Generate opponent moves after the simulated move
            Genw(board);
            List<int> opponentMoves = new List<int>(isWhite ? bpm : wpm);
            // Debug.Log("Opponent moves after simulation: " + string.Join(", ", opponentMoves));

            // Check if the move causes check
            if (!IsKingInCheck(board, isWhite, opponentMoves))
            {
                validMoves.Add(sourceIndex);
                validMoves.Add(targetIndex);
            }
            /*else
            {
                Debug.Log($"Move {sourceIndex} -> {targetIndex} causes check and is discarded.");
            }*/

            // Undo the move
            board[sourceIndex] = originalSourcePiece;
            board[targetIndex] = originalTargetPiece;
            // Array.Copy(boardBackup, board, board.Length);
            //Debug.Log($"Move {sourceIndex} -> {targetIndex} reverted.");
        }

        // Update the move list with valid moves
        if (isWhite)
            WF = new List<int>(validMoves);
        else
            BF = new List<int>(validMoves);

        //Debug.Log("Valid Moves: " + string.Join(", ", validMoves));
        //Debug.Log("Valid Moves Count: " + validMoves.Count);
    }


















    /* public void RemoveMovesThatCauseCheck(bool isWhite)
     {

         Genw(board);
         // List<int> possibleMoves = isWhite ? wpm : bpm;
         List<int> possibleMoves = new List<int>(isWhite ? wpm : bpm);
         List<int> validMoves = new List<int>();


         int[] boardBackup = new int[board.Length];
         Array.Copy(board, boardBackup, board.Length);

         // Validate possibleMoves structure


         // Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves));
         // Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves.Count));
         // Iterate through the possibleMoves array in pairs
         //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves));
         //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves.Count));
         for (int i = 0; i < possibleMoves.Count; i += 2)
         {
             // Debug.Log($"i i+1 {i} -> {i+1}");
             //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves));
             //Debug.Log("Initial possibleMoves: " + string.Join(", ", possibleMoves.Count));


             int sourceIndex = possibleMoves[i];
             int targetIndex = possibleMoves[i + 1];

             // Validate indices


             //Debug.Log($"Simulating move {sourceIndex} -> {targetIndex}");

             // Simulate the move
             int originalSourcePiece = board[sourceIndex];
             int originalTargetPiece = board[targetIndex];
             board[targetIndex] = board[sourceIndex];
             board[sourceIndex] = 0;

             // Generate opponent moves after the simulated move
             Genw(board);
             List<int> opponentMoves = new List<int>(isWhite ? bpm : wpm);
             // Debug.Log("Opponent moves after simulation: " + string.Join(", ", opponentMoves));

             // Check if the move causes check
             if (!IsKingInCheck(board, isWhite, opponentMoves))
             {
                 validMoves.Add(sourceIndex);
                 validMoves.Add(targetIndex);
             }
             else
             {
                 Debug.Log($"Move {sourceIndex} -> {targetIndex} causes check and is discarded.");
             }

             // Undo the move
             //board[sourceIndex] = originalSourcePiece;
             //board[targetIndex] = originalTargetPiece;
             Array.Copy(boardBackup, board, board.Length);
             //Debug.Log($"Move {sourceIndex} -> {targetIndex} reverted.");
         }

         // Update the move list with valid moves
         if (isWhite)
             WF = new List<int>(validMoves);
         else
             BF = new List<int>(validMoves);

         //Debug.Log("Valid Moves: " + string.Join(", ", validMoves));
         //Debug.Log("Valid Moves Count: " + validMoves.Count);
     }


     */




    // Helper to check if the king is in check
    public bool IsKingInCheck(int[] board, bool isWhite, List<int> opponentMoves)
    {
        int kingPosition = -1;

        // Find the king's position
        for (int i = 0; i < board.Length; i++)
        {
            if ((isWhite && board[i] == 6) || (!isWhite && board[i] == 12))
            {
                kingPosition = i;
                break;
            }
        }

        // Check if any opponent move targets the king's position
        for (int i = 1; i < opponentMoves.Count; i += 2)
        {
            if (opponentMoves[i] == kingPosition)
                return true;
        }

        return false;
    }


    public void DisplayBoard(int[] board)
    {
        for (int rank = 7; rank >= 0; rank--) // Iterate from rank 7 to rank 0
        {
            string row = "";
            for (int file = 0; file < 8; file++) // Iterate through files 0 to 7
            {
                int index = rank * 8 + file; // Calculate the 1D index for the 8x8 board
                row += board[index].ToString().PadLeft(3); // Add the piece value, padded for alignment
            }
            UnityEngine.Debug.Log(row); // Print each rank as a row
        }
    }







    //non-threaded
    public void Genw(int[] board)
    {
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        wpm.Clear();
        bpm.Clear();
        WEP.Clear();
        BEP.Clear();
        // Iterate over all squares on the board (0 to 63)
        for (int i = 0; i < 64; i++)
        {
            int piece = board[i];

            // Check if the piece is a white pawn
            if (IsWhite(piece) && piece == 1) // White pawn
            {
                GeneratePawnMoves(board, i, true, wpm);
                GeneratePawnEP(board, i, true, WEP);// Pass `wpm` to store moves
            }
            // Check if the piece is a black pawn
            if (!IsWhite(piece) && piece == 7) // Black pawn
            {
                GeneratePawnMoves(board, i, false, bpm);
                GeneratePawnEP(board, i, false, BEP);// Pass `bpm` to store moves
            }

            if (IsWhite(piece) && piece == 2) // white rook
            {
                GenerateRookMoves(board, i, true, wpm); // Pass `wpm` to store moves
            }
            if (!IsWhite(piece) && piece == 8) // Black rook
            {
                GenerateRookMoves(board, i, false, bpm); // Pass `bpm` to store moves
            }
            if (IsWhite(piece) && piece == 4) // white bishop
            {
                GenerateBishopMoves(board, i, true, wpm); // Pass `wpm` to store moves
            }
            if (!IsWhite(piece) && piece == 10) // Black bishop
            {
                GenerateBishopMoves(board, i, false, bpm); // Pass `bpm` to store moves
            }
            if (IsWhite(piece) && piece == 5) // white queen
            {
                GenerateQueenMoves(board, i, true, wpm); // Pass `wpm` to store moves
            }
            if (!IsWhite(piece) && piece == 11) // Black queen
            {
                GenerateQueenMoves(board, i, false, bpm); // Pass `bpm` to store moves
            }
            if (IsWhite(piece) && piece == 3) // white knight
            {
                GenerateKnightMoves(board, i, true, wpm); // Pass `wpm` to store moves
            }
            if (!IsWhite(piece) && piece == 9) // Black knight
            {
                GenerateKnightMoves(board, i, false, bpm); // Pass `bpm` to store moves
            }


            if (IsWhite(piece) && piece == 6) // white king
            {
                GenerateKingMoves(board, i, true, wpm); // Pass `wpm` to store moves
            }
            if (!IsWhite(piece) && piece == 12) // Black king
            {
                GenerateKingMoves(board, i, false, bpm); // Pass `bpm` to store moves
            }


        }
        //stopwatch.Stop();
        //UnityEngine.Debug.Log("Move generation took: " + stopwatch.ElapsedMilliseconds + " ms");
        // Debug.Log("White Pawn Moves: " + string.Join(", ", wpm));
        //Debug.Log("Black Pawn Moves: " + string.Join(", ", bpm));
    }













    public void gen()
    {
        WF.Clear();
        BF.Clear();
        // Genw(board);
        // Debug.Log("gen White Pawn Moves: " + string.Join(", ", wpm));
        // Debug.Log("gen White Pawn Moves: " + string.Join(", ", wpm.Count));
        //Debug.Log("Black Pawn Moves: " + string.Join(", ", bpm));
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        RemoveMovesThatCauseCheck(true);
        //Debug.Log("after gen White Pawn Moves: " + string.Join(", ", WF));
        //Debug.Log("after gen White Pawn Moves: " + string.Join(", ", WF.Count));
        // Debug.Log("Black Pawn Moves: " + string.Join(", ", bpm));



        //Debug.Log("gen black Pawn Moves: " + string.Join(", ", bpm));
        // Debug.Log("gen black Pawn Moves: " + string.Join(", ", bpm.Count));
        //Debug.Log("Black Pawn Moves: " + string.Join(", ", bpm));
        RemoveMovesThatCauseCheck(false);
        // Debug.Log("after gen black Pawn Moves: " + string.Join(", ", BF));
        //Debug.Log("after gen black Pawn Moves: " + string.Join(", ", BF.Count));
        stopwatch.Stop();
        //UnityEngine.Debug.Log("Move generation took: " + stopwatch.ElapsedMilliseconds + " ms");

    }




    public void GenerateCastlingMoves(int[] board, bool isWhite, List<int> possibleMoves, bool[] castlingRights, List<int> wpm, List<int> bpm)
    {
        // WCM.Clear();
        //BCM.Clear();
        // Determine key indexes based on color
        int kingIndex = isWhite ? 4 : 60; // E1 for White, E8 for Black
        int kingsideRookIndex = isWhite ? 7 : 63; // H1 for White, H8 for Black
        int queensideRookIndex = isWhite ? 0 : 56; // A1 for White, A8 for Black

        // Castling rights indices
        int kingsideIndex = isWhite ? 0 : 2;
        int queensideIndex = isWhite ? 1 : 3;

        // Get opponent's possible moves from gen()
        List<int> opponentMoves = isWhite ? bpm : wpm;

        // Helper function to check if a square is under attack
        bool IsSquareUnderAttack(int square)
        {
            for (int i = 1; i < opponentMoves.Count; i += 2) // Iterate over targets (second elements in pairs)
            {
                if (opponentMoves[i] == square)
                    return true;
            }
            return false;
        }

        // Kingside Castling
        if (castlingRights[kingsideIndex] &&
            board[kingsideRookIndex] == (isWhite ? 2 : 8) && // Ensure rook is present at H1/H8
            board[kingIndex + 1] == 0 &&                    // F1/F8 is empty
            board[kingIndex + 2] == 0 &&                    // G1/G8 is empty
            !IsSquareUnderAttack(kingIndex) &&              // E1/E8 is not under attack
            !IsSquareUnderAttack(kingIndex + 1) &&          // F1/F8 is not under attack
            !IsSquareUnderAttack(kingIndex + 2))            // G1/G8 is not under attack
        {
            // Add kingside castling move (king and rook)
            possibleMoves.Add(kingIndex);          // King's source position (E1 or E8)
            possibleMoves.Add(kingIndex + 2);      // King's target position (G1 or G8)
            possibleMoves.Add(kingsideRookIndex);  // Rook's source position (H1 or H8)
            possibleMoves.Add(kingsideRookIndex - 2); // Rook's target position (F1 or F8)
        }

        // Queenside Castling
        if (castlingRights[queensideIndex] &&
            board[queensideRookIndex] == (isWhite ? 2 : 8) && // Ensure rook is present at A1/A8
            board[kingIndex - 1] == 0 &&                    // D1/D8 is empty
            board[kingIndex - 2] == 0 &&                    // C1/C8 is empty
            board[kingIndex - 3] == 0 &&                    // B1/B8 is empty
            !IsSquareUnderAttack(kingIndex) &&              // E1/E8 is not under attack
            !IsSquareUnderAttack(kingIndex - 1) &&          // D1/D8 is not under attack
            !IsSquareUnderAttack(kingIndex - 2))            // C1/C8 is not under attack
        {
            // Add queenside castling move (king and rook)
            possibleMoves.Add(kingIndex);          // King's source position (E1 or E8)
            possibleMoves.Add(kingIndex - 2);      // King's target position (C1 or C8)
            possibleMoves.Add(queensideRookIndex); // Rook's source position (A1 or A8)
            possibleMoves.Add(queensideRookIndex + 3); // Rook's target position (D1 or D8)
        }
    }




    public List<int> WCM = new List<int>();
    public List<int> BCM = new List<int>();
    public bool[] castlingRights = { true, true, true, true };
    public void gen2()
    {
        WF.Clear();
        BF.Clear();
        WCM.Clear();
        BCM.Clear();
        Genw(board);
        // Debug.Log("black Pawn Moves: " + string.Join(", ", bpm));
        // Debug.Log("black Pawn Moves: " + string.Join(", ", bpm.Count));
        // Debug.Log("white Pawn Moves: " + string.Join(", ", wpm));
        // Debug.Log("white Pawn Moves: " + string.Join(", ", wpm.Count));
        gen();
        //Debug.Log("after gen White Pawn Moves: " + string.Join(", ", WF));
        // Debug.Log("after gen White Pawn Moves: " + string.Join(", ", WF.Count/2));
        // Debug.Log("after gen black Pawn Moves: " + string.Join(", ", BF));
        // Debug.Log("after gen black Pawn Moves: " + string.Join(", ", BF.Count/2));
        GenerateCastlingMoves(board, true, WCM, castlingRights, WF, BF);


        // Debug.Log("white castling Moves: " + string.Join(", ", WCM));

        GenerateCastlingMoves(board, false, BCM, castlingRights, WF, BF);


        //Debug.Log("black castling Moves: " + string.Join(", ", BCM));






    }

    public bool whiteTurn = true;
    public void Rando(bool iswhite)
    {
        // gen2();
        DestroyAllArrows();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        UnityEngine.Debug.Log("Engine started........");
        gen();
        List<int> wfCopy = new List<int>(WF);
        List<int> bfCopy = new List<int>(BF);
       // WF = MoveOrder(WF);
        //BF = MoveOrder(BF);
        WF = MoveOrder(WF, globalBestSource, globalBestTarget);
        BF = MoveOrder(BF, globalBestSource, globalBestTarget);
        //wfCopy = MoveOrder(wfCopy);
        //bfCopy = MoveOrder(bfCopy);
        wfCopy = MoveOrder(wfCopy, globalBestSource, globalBestTarget);
        bfCopy = MoveOrder(bfCopy, globalBestSource, globalBestTarget);
        

        //List<int>l2= iswhite ? new List<int>(WCM) : new List<int>(BCM);

        // List<int> l3 = iswhite ? new List<int>(WEP) : new List<int>(BEP);

        //  int randomIndex = UnityEngine.Random.Range(0, l1.Count / 2) * 2;

        // int source = l1[randomIndex];
        // int target = l1[randomIndex + 1];


        // Vector3 s = GetPositionFromBoardIndex(source);
        // Vector3 t = GetPositionFromBoardIndex(target);

        // Debug.Log(s.x + " " + s.y + " " + t.x + " " + t.y);

        //Destroy(chessGUI.highlightedSquare);
        /*if (chessGUI.highlightedSquare != null)
        {
            //Debug.Log("destroying...");
            Destroy(chessGUI.highlightedSquare);

            // Remove the previous highlight
        }
        if (chessGUI.highlightedSquare2 != null)
        {
            //Debug.Log("destroying...");
            Destroy(chessGUI.highlightedSquare2);

            // Remove the previous highlight
        }*/

        //Destroy(chessGUI.highlightedSquare);
        //Destroy(chessGUI.highlightedSquare2);
        //chessGUI.count += 1;
        //chessGUI.HighlightSquare(s);

        // MovePieceI(source, target);
        //chessGUI.HighlightSquare2(t);
        //chessGUI.count += 1;

        // chessGUI.HighlightSquare(s);
        int positionCount = 0;
        //var result = Minimax(depth, true, WF, BF, ref positionCount);
        int evaluateCallCount = 0;
        List<int> rootMoveScores = new List<int>();

        //(int bestEval, int bestSource, int bestTarget) = Minimax(3, false, WF, BF,ref positionCount);
        //(int bestEval1, int bestSource1, int bestTarget1) = Minimax(1, false, WF, BF, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue);

        //(int bestEval2, int bestSource2, int bestTarget2) = Minimax3(2, false, WF, BF, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue);
        //positionCount = 0;
        //evaluateCallCount = 0;
        //(int bestEval3, int bestSource3, int bestTarget3) = Minimax(3, false, WF, BF, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue);

        //InitializeZobrist();
        //transpositionTable.Clear();
        //transpositionTable = new Dictionary<ulong, int>();
        //UnityEngine.Debug.Log($"Total keys in transposition table: {transpositionTable.Count}");

        (int bestEval, int bestSource, int bestTarget) = Minimax(2, iswhite, wfCopy, bfCopy, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue);
        //3.2 sec 5626
        //(int bestEval, int bestSource, int bestTarget) = Minimax(2, false, wfCopy, bfCopy, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue);
        //3.3sec 5626
       
        //(int bestEval, int bestSource, int bestTarget) = Minimax8(2, false, wfCopy, bfCopy, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue,rootMoveScores);

        //(int bestEval, int bestSource, int bestTarget) = IterativeDeepening(4, false, wfCopy, bfCopy); 
        //3.6 sec on minimax3        3.6 sec on minimax same

        //MinimaxState state = new MinimaxState();
        //(int bestEval, int bestSource, int bestTarget) = Minimax2(2, false, WF, BF, state, int.MinValue, int.MaxValue);
        stopwatch.Stop();
        UnityEngine.Debug.Log("engine took: " + stopwatch.ElapsedMilliseconds / 1000.0 + " seconds");

        //UnityEngine.Debug.Log("Engine took: " + (stopwatch.ElapsedMilliseconds / 60000.0) + " minutes");

        //1-20

        //2-380

        //3-8457 4 sec no eval


        //4-180858  1 min 1 sec  eval 

        UnityEngine.Debug.Log($"Best move for black: {bestSource} -> {bestTarget} with evaluation: {bestEval}");
        //UnityEngine.Debug.Log($"Total positions evaluated: {positionCount}");

        //UnityEngine.Debug.Log($"Evaluate() was called {evaluateCallCount} times.");
        UnityEngine.Debug.Log($"Total number of keys in transposition table: {transpositionTable.Count}");

        //UnityEngine.Debug.Log($"Position Count: {state.PositionCount}, Evaluation Count: {state.EvaluationCount}");
       // UnityEngine.Debug.Log("Killer Moves: " + string.Join(", ", killerMoves));



        chessGUI.botHighlightSquare(bestSource);
        MovePieceIB(bestSource, bestTarget);
        chessGUI.botHighlightSquare2(bestTarget);

        //PrintRootMoves(rootMoveScores,bfCopy);

        //if (analysis == true)
        //{
            //Analysis(!iswhite);
        //}
        //Analysis(!iswhite);

    }

    public List<int> rootMoveScores = new List<int>();
    public void Analysis(bool iswhite)
    {
        // gen2();
        rootMoveScores.Clear();
        DestroyAllArrows();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        UnityEngine.Debug.Log("Engine started........");
        gen();
        List<int> wfCopy = new List<int>(WF);
        List<int> bfCopy = new List<int>(BF);
        // WF = MoveOrder(WF);
        //BF = MoveOrder(BF);
        WF = MoveOrder(WF, globalBestSource, globalBestTarget);
        BF = MoveOrder(BF, globalBestSource, globalBestTarget);
        //wfCopy = MoveOrder(wfCopy);
        //bfCopy = MoveOrder(bfCopy);
        wfCopy = MoveOrder(wfCopy, globalBestSource, globalBestTarget);
        bfCopy = MoveOrder(bfCopy, globalBestSource, globalBestTarget);


        


        
        int positionCount = 0;
        //var result = Minimax(depth, true, WF, BF, ref positionCount);
        int evaluateCallCount = 0;
        //List<int> rootMoveScores = new List<int>();

       

        (int bestEval, int bestSource, int bestTarget) = Minimax8(4, iswhite, wfCopy, bfCopy, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue, rootMoveScores);

     
        stopwatch.Stop();
        UnityEngine.Debug.Log("engine took: " + stopwatch.ElapsedMilliseconds / 1000.0 + " seconds");

        //UnityEngine.Debug.Log("Engine took: " + (stopwatch.ElapsedMilliseconds / 60000.0) + " minutes");

        //1-20

        //2-380

        //3-8457 4 sec no eval


        //4-180858  1 min 1 sec  eval 

        UnityEngine.Debug.Log($"Best move for black: {bestSource} -> {bestTarget} with evaluation: {bestEval}");
        //UnityEngine.Debug.Log($"Total positions evaluated: {positionCount}");

        //UnityEngine.Debug.Log($"Evaluate() was called {evaluateCallCount} times.");
        UnityEngine.Debug.Log($"Total number of keys in transposition table: {transpositionTable.Count}");

        //UnityEngine.Debug.Log($"Position Count: {state.PositionCount}, Evaluation Count: {state.EvaluationCount}");
        // UnityEngine.Debug.Log("Killer Moves: " + string.Join(", ", killerMoves));



        //chessGUI.botHighlightSquare(bestSource);
        //MovePieceIB(bestSource, bestTarget);
        //chessGUI.botHighlightSquare2(bestTarget);



        
        PrintRootMovesBt(rootMoveScores, bfCopy,iswhite);

    }


   


    public Dictionary<ulong, int> transpositionTable = new Dictionary<ulong, int>();

    // Store position evaluation in transposition table
    public void StoreEvaluation(int[] boardState, int score)
    {
        ulong key = ComputeHash(boardState);
        transpositionTable[key] = score;
    }

    // Retrieve stored evaluation if available
    public bool TryGetEvaluation(int[] boardState, out int score)
    {
        ulong key = ComputeHash(boardState);
        return transpositionTable.TryGetValue(key, out score);
    }






    /*void PrintTranspositionTable()
    {
        Debug.Log("Transposition Table (Key -> Score):");

        // Header
        Debug.Log("--------------------------------------------------------");
        Debug.Log("|    Key (Zobrist Hash)    |   Score   |");
        Debug.Log("--------------------------------------------------------");

        // Loop through each entry and print in a table-like format
        foreach (var entry in transpositionTable)
        {
            Debug.Log($"| {entry.Key,24} | {entry.Value,8} |");
        }

        Debug.Log("--------------------------------------------------------");
    }
    */



    public static ulong[,] zobristTable = new ulong[64, 12]; // 64 squares × 12 piece types
    public static RandomNumberGenerator rng = RandomNumberGenerator.Create();

    // Initialize Zobrist table with random values
    public static void InitializeZobrist()
    {
        byte[] buffer = new byte[8]; // 8 bytes for 64-bit values

        for (int square = 0; square < 64; square++)
        {
            for (int piece = 0; piece < 12; piece++)
            {
                rng.GetBytes(buffer); // Fill buffer with random bytes
                zobristTable[square, piece] = BitConverter.ToUInt64(buffer, 0); // Convert to ulong
            }
        }
    }

    // Compute the Zobrist hash for the given board state (int[64])
    public static ulong ComputeHash(int[] board) // board = int[64], 0 = empty, 1-12 = piece type
    {
        ulong hash = 0;

        for (int square = 0; square < 64; square++)
        {
            int piece = board[square]; // Get piece at this square
            if (piece != 0) // If not empty
            {
                hash ^= zobristTable[square, piece - 1]; // XOR corresponding hash
            }
        }

        return hash; // Return final computed hash
    }










    public (int eval, int source, int target) Minimax(int depth, bool isMaximizingPlayer, List<int> wf, List<int> bf, ref int positionCount, ref int ec, int alpha, int beta)
    {
        // positionCount++;
        //Debug.Log($"Depth: {depth}, PositionCount: {positionCount}, WF: {wf.Count/2}, BF: {bf.Count/2}");
        if (depth == 0)
        {
            positionCount++;
            if (TryGetEvaluation(board, out int storedScore))
            {
                return (storedScore, -1, -1);  // Use stored score to save computation
            }
            else
            {
                ec++;
                int score = Evaluate(false, false);
                StoreEvaluation(board, score);
                return (score, -1, -1);
            }
            //return (Evaluate(), -1, -1); // Base case: return evaluation with no move.
        }

        // Create local copies of the move lists to avoid altering the original lists.
        List<int> wfCopy = new List<int>(wf);
        List<int> bfCopy = new List<int>(bf);
        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            int bestSource = -1;
            int bestTarget = -1;
            // Loop through white's moves.
            for (int i = 0; i < wfCopy.Count; i += 2)
            {
                int source = wfCopy[i];
                int target = wfCopy[i + 1];

                // Make the move for white.
                int spiece = board[source];
                int tpiece = board[target];
                board[source] = 0;
                board[target] = spiece;

                // Call gen2 to generate white's potential moves (this won't modify wfCopy or bfCopy).
                //gen2();
                gen();

                //WF = MoveOrder(WF);
                //BF = MoveOrder(BF);
                WF = MoveOrder(WF,globalBestSource,globalBestTarget);
                BF = MoveOrder(BF, globalBestSource, globalBestTarget);
                //Debug.Log($"Before Minimax Recursion (Maximizing): Depth: {depth}, WF: {WF.Count/2}, BF: {BF.Count/2}, source: {source}, target: {target}");



                if (BF == null || BF.Count == 0)
                {
                    // Either checkmate or stalemate; handle accordingly.
                    /*board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;
                    return (Evaluate(), source, target);*/ // Return evaluation immediately if no moves are left.




                    if (TryGetEvaluation(board, out int storedScore))
                    {
                        board[source] = spiece;
                        board[target] = tpiece;
                        positionCount++;
                        return (storedScore, source, target);
                    }
                    else
                    {
                        ec++;
                        int score = Evaluate(false, true);
                        StoreEvaluation(board, score);
                        board[source] = spiece;
                        board[target] = tpiece;
                        positionCount++;
                        return (score, source, target);
                    }


                }
                // Recurse to minimize for black player.
                var (eval, _, _) = Minimax(depth - 1, false, WF, BF, ref positionCount, ref ec, alpha, beta);
                // Undo the move for white.
                board[source] = spiece;
                board[target] = tpiece;
                // Update maxEval and track the best move.
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestSource = source;
                    bestTarget = target;
                }
                //alpha = Math.Max(alpha, maxEval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;  // Beta cut-off.
                }
            }

            return (maxEval, bestSource, bestTarget); // Return best evaluation and move.
        }
        else
        {
            int minEval = int.MaxValue;
            int bestSource = -1;
            int bestTarget = -1;

            // Loop through black's moves.
            for (int i = 0; i < bfCopy.Count; i += 2)
            {
                int source = bfCopy[i];
                int target = bfCopy[i + 1];

                // Make the move for black.
                int spiece = board[source];
                int tpiece = board[target];
                board[source] = 0;
                board[target] = spiece;

                // Call gen2 to generate black's potential moves (this won't modify wfCopy or bfCopy).
                //gen2();
                gen();

                //Debug.Log($"Before Minimax Recursion (Minimizing): Depth: {depth}, WF: {WF.Count/2}, BF: {BF.Count/2}, source: {source}, target: {target}");
                //WF = MoveOrder(WF);
                //BF = MoveOrder(BF);
                // WF = MoveOrder(WF);
                //BF = MoveOrder(BF);
                // WF = MoveOrder(WF);
                // BF = MoveOrder(BF);
                WF = MoveOrder(WF, globalBestSource, globalBestTarget);
                BF = MoveOrder(BF, globalBestSource, globalBestTarget);

                if (WF == null || WF.Count == 0)
                {
                    // Either checkmate or stalemate; handle accordingly.
                    /*board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;
                    return (Evaluate(), source, target); */// Return evaluation immediately if no moves are left.


                    if (TryGetEvaluation(board, out int storedScore))
                    {
                        board[source] = spiece;
                        board[target] = tpiece;
                        positionCount++;
                        return (storedScore, source, target);
                    }
                    else
                    {
                        ec++;
                        int score = Evaluate(true, false);
                        StoreEvaluation(board, score);
                        board[source] = spiece;
                        board[target] = tpiece;
                        positionCount++;
                        return (score, source, target);
                    }



                }

                // Recurse to maximize for white player.
                var (eval, _, _) = Minimax(depth - 1, true, WF, BF, ref positionCount, ref ec, alpha, beta);

                // Undo the move for black.
                board[source] = spiece;
                board[target] = tpiece;

                // Update minEval and track the best move.
                if (eval < minEval)
                {
                    minEval = eval;
                    bestSource = source;
                    bestTarget = target;
                }




                //beta = Math.Min(beta, minEval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;  // Alpha cut-off.
                }



            }

            return (minEval, bestSource, bestTarget); // Return best evaluation and move.
        }
    }







    public (int eval, int source, int target) Minimax3(int depth, bool isMaximizingPlayer, List<int> wf, List<int> bf, ref int positionCount, ref int ec, int alpha, int beta)
    {

        // positionCount++;

        //Debug.Log($"Depth: {depth}, PositionCount: {positionCount}, WF: {wf.Count/2}, BF: {bf.Count/2}");



        if (depth == 0)
        {
            positionCount++;

           
            
           
            ec++;
            int score = Evaluate(false, false);
            return (score, -1, -1);
            



            //return (Evaluate(), -1, -1); // Base case: return evaluation with no move.
        }

        // Create local copies of the move lists to avoid altering the original lists.
        List<int> wfCopy = new List<int>(wf);
        List<int> bfCopy = new List<int>(bf);

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            int bestSource = -1;
            int bestTarget = -1;

            // Loop through white's moves.
            for (int i = 0; i < wfCopy.Count; i += 2)
            {
                int source = wfCopy[i];
                int target = wfCopy[i + 1];

                // Make the move for white.
                int spiece = board[source];
                int tpiece = board[target];
                board[source] = 0;
                board[target] = spiece;

                // Call gen2 to generate white's potential moves (this won't modify wfCopy or bfCopy).
                //gen2();
                gen();

                //WF = MoveOrder(WF);
                //BF = MoveOrder(BF);
                //WF = MoveOrder(WF);
                //BF = MoveOrder(BF);
                //Debug.Log($"Before Minimax Recursion (Maximizing): Depth: {depth}, WF: {WF.Count/2}, BF: {BF.Count/2}, source: {source}, target: {target}");



                if (BF == null || BF.Count == 0)
                {
                    // Either checkmate or stalemate; handle accordingly.
                    /*board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;
                    return (Evaluate(), source, target);*/ // Return evaluation immediately if no moves are left.




                    
                    
                   
                    ec++;
                    int score = Evaluate(false, true);
                    
                    board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;
                    return (score, source, target);
                    


                }




                // Recurse to minimize for black player.
                var (eval, _, _) = Minimax3(depth - 1, false, WF, BF, ref positionCount, ref ec, alpha, beta);

                // Undo the move for white.
                board[source] = spiece;
                board[target] = tpiece;

                // Update maxEval and track the best move.
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestSource = source;
                    bestTarget = target;
                }


                //alpha = Math.Max(alpha, maxEval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;  // Beta cut-off.
                }



            }

            return (maxEval, bestSource, bestTarget); // Return best evaluation and move.
        }
        else
        {
            int minEval = int.MaxValue;
            int bestSource = -1;
            int bestTarget = -1;

            // Loop through black's moves.
            for (int i = 0; i < bfCopy.Count; i += 2)
            {
                int source = bfCopy[i];
                int target = bfCopy[i + 1];

                // Make the move for black.
                int spiece = board[source];
                int tpiece = board[target];
                board[source] = 0;
                board[target] = spiece;

                // Call gen2 to generate black's potential moves (this won't modify wfCopy or bfCopy).
                //gen2();
                gen();

                //Debug.Log($"Before Minimax Recursion (Minimizing): Depth: {depth}, WF: {WF.Count/2}, BF: {BF.Count/2}, source: {source}, target: {target}");
                //WF = MoveOrder(WF);
                //BF = MoveOrder(BF);
                //WF = MoveOrder(WF);
                //BF = MoveOrder(BF);
               // WF = MoveOrder(WF);
                //BF = MoveOrder(BF);

                if (WF == null || WF.Count == 0)
                {
                    // Either checkmate or stalemate; handle accordingly.
                    /*board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;
                    return (Evaluate(), source, target); */// Return evaluation immediately if no moves are left.


                    
                    
                    
                    ec++;
                    int score = Evaluate(true, false);
                    
                    board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;
                    return (score, source, target);
                    



                }

                // Recurse to maximize for white player.
                var (eval, _, _) = Minimax3(depth - 1, true, WF, BF, ref positionCount, ref ec, alpha, beta);

                // Undo the move for black.
                board[source] = spiece;
                board[target] = tpiece;

                // Update minEval and track the best move.
                if (eval < minEval)
                {
                    minEval = eval;
                    bestSource = source;
                    bestTarget = target;
                }




                //beta = Math.Min(beta, minEval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;  // Alpha cut-off.
                }



            }

            return (minEval, bestSource, bestTarget); // Return best evaluation and move.
        }
    }







    public (int eval, int source, int target) Minimax8(
    int depth,
    bool isMaximizingPlayer,
    List<int> wf,
    List<int> bf,
    ref int positionCount,
    ref int ec,
    int alpha,
    int beta,
    List<int> rootMoves = null,   // Optional list to collect [source, target, score] triples at the root
    int rootDepth = -1)           // Internal parameter to record the original search depth
    {
        // If this is the top‐level call, record the root depth.
        if (rootDepth == -1)
            rootDepth = depth;

        // Base case: if depth is 0, evaluate the position.
        if (depth == 0)
        {
            positionCount++;

            if (TryGetEvaluation(board, out int storedScore))
            {
                return (storedScore, -1, -1);  // Use stored score to save computation
            }
            else
            {
                ec++;
                int score = Evaluate(false, false);
                StoreEvaluation(board, score);
                return (score, -1, -1);
            }
        }

        // Create local copies of the move lists so that the originals remain unchanged.
        List<int> wfCopy = new List<int>(wf);
        List<int> bfCopy = new List<int>(bf);

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            int bestSource = -1;
            int bestTarget = -1;

            // Loop through white's moves.
            for (int i = 0; i < wfCopy.Count; i += 2)
            {
                int source = wfCopy[i];
                int target = wfCopy[i + 1];

                // Make the move for white.
                int spiece = board[source];
                int tpiece = board[target];
                board[source] = 0;
                board[target] = spiece;

                // Generate moves.
                gen();

                // Optionally re-order moves.
                WF = MoveOrder(WF, globalBestSource, globalBestTarget);
                BF = MoveOrder(BF, globalBestSource, globalBestTarget);

                // Terminal check: if opponent (black) has no moves.
                if (BF == null || BF.Count == 0)
                {
                    int termScore;
                    if (TryGetEvaluation(board, out int storedScore))
                    {
                        termScore = storedScore;
                    }
                    else
                    {
                        ec++;
                        termScore = Evaluate(false, true);
                        StoreEvaluation(board, termScore);
                    }
                    // Undo the move.
                    board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;

                    // If at root level, record this move.
                    if (depth == rootDepth && rootMoves != null)
                    {
                        rootMoves.Add(source);
                        rootMoves.Add(target);
                        rootMoves.Add(termScore);
                    }

                    // Return immediately (as per your original logic when no moves are left).
                    return (termScore, source, target);
                }

                // Recurse for the minimizing (black) player.
                var (eval, _, _) = Minimax8(depth - 1, false, WF, BF, ref positionCount, ref ec, alpha, beta, rootMoves, rootDepth);

                // Undo the move.
                board[source] = spiece;
                board[target] = tpiece;

                // If at root level, record this move's evaluation.
                if (depth == rootDepth && rootMoves != null)
                {
                    rootMoves.Add(source);
                    rootMoves.Add(target);
                    rootMoves.Add(eval);
                }

                // Update best evaluation and best move.
                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestSource = source;
                    bestTarget = target;
                }

                // Alpha–beta update.
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;  // Beta cut-off.
                }
            }

            return (maxEval, bestSource, bestTarget);
        }
        else  // Minimizing player
        {
            int minEval = int.MaxValue;
            int bestSource = -1;
            int bestTarget = -1;

            // Loop through black's moves.
            for (int i = 0; i < bfCopy.Count; i += 2)
            {
                int source = bfCopy[i];
                int target = bfCopy[i + 1];

                // Make the move for black.
                int spiece = board[source];
                int tpiece = board[target];
                board[source] = 0;
                board[target] = spiece;

                // Generate moves.
                gen();

                WF = MoveOrder(WF, globalBestSource, globalBestTarget);
                BF = MoveOrder(BF, globalBestSource, globalBestTarget);

                // Terminal check: if opponent (white) has no moves.
                if (WF == null || WF.Count == 0)
                {
                    int termScore;
                    if (TryGetEvaluation(board, out int storedScore))
                    {
                        termScore = storedScore;
                    }
                    else
                    {
                        ec++;
                        termScore = Evaluate(true, false);
                        StoreEvaluation(board, termScore);
                    }
                    // Undo the move.
                    board[source] = spiece;
                    board[target] = tpiece;
                    positionCount++;

                    // If at root level, record this move.
                    if (depth == rootDepth && rootMoves != null)
                    {
                        rootMoves.Add(source);
                        rootMoves.Add(target);
                        rootMoves.Add(termScore);
                    }

                    // Return immediately.
                    return (termScore, source, target);
                }

                // Recurse for the maximizing (white) player.
                var (eval, _, _) = Minimax8(depth - 1, true, WF, BF, ref positionCount, ref ec, alpha, beta, rootMoves, rootDepth);

                // Undo the move.
                board[source] = spiece;
                board[target] = tpiece;

                // If at root level, record this move's evaluation.
                if (depth == rootDepth && rootMoves != null)
                {
                    rootMoves.Add(source);
                    rootMoves.Add(target);
                    rootMoves.Add(eval);
                }

                // Update best evaluation and best move.
                if (eval < minEval)
                {
                    minEval = eval;
                    bestSource = source;
                    bestTarget = target;
                }

                // Alpha–beta update.
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;  // Alpha cut-off.
                }
            }

            return (minEval, bestSource, bestTarget);
        }
    }


























    public List<int> killerMoves = new List<int>();
    public List<int> killerMovescopy = new List<int>();


  












    // int globalBestSource = -1;
    //int globalBestTarget = -1;


    public (int bestEval, int bestSource, int bestTarget) IterativeDeepening(int maxDepth, bool isWhite, List<int> wf, List<int> bf)
    {

        List<int> wfCopy = new List<int>(wf);
        List<int> bfCopy = new List<int>(bf);
        List<int> rms = new List<int>();
        int bestEval = 0;
        int bestSource = -1;
        int bestTarget = -1;
        int positionCount = 0;
        int evaluateCallCount = 0;

        for (int depth = 2; depth <= maxDepth; depth+=2) // Iteratively increasing depth
        {

            //killerMoves.Clear();
            (bestEval, bestSource, bestTarget) = Minimax(depth, isWhite, wfCopy, bfCopy, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue);
            //(bestEval,bestSource,bestTarget) = Minimax8(4, false, wfCopy, bfCopy, ref positionCount, ref evaluateCallCount, int.MinValue, int.MaxValue, rms);


            globalBestSource = bestSource;
            globalBestTarget = bestTarget;
            
            UnityEngine.Debug.Log($"Best move for Black in depth:{depth}: {bestSource} -> {bestTarget} with evaluation: {bestEval}");
            UnityEngine.Debug.Log($"Total positions evaluated: {positionCount}");
            PrintRootMoves(rms, bfCopy);
            //UnityEngine.Debug.Log($"Evaluate() was called {evaluateCallCount} times.");
            //UnityEngine.Debug.Log($"Killer Moves in depth {depth}: " + string.Join(", ", killerMoves));
            positionCount = 0;
            evaluateCallCount = 0;

           
            
        }

        return (bestEval, bestSource, bestTarget);
        //return (-1,-1,-1);// Return the best move found at the highest completed depth
    }





















   














    /*int Evaluate(bool a,bool b)
    {

        //evaluateCallCount++;
        // Material values for each piece.
        int score = 0;

        // Loop through all squares on the board.
        for (int i = 0; i < 64; i++)
        {
            int piece = board[i]; // Get the piece at the current square.

            if (piece == 1) // White Pawn
            {
                score += 1;
            }
            else if (piece == 2) // White Knight
            {
                score += 5;
            }
            else if (piece == 3) // White Bishop
            {
                score += 3;
            }
            else if (piece == 4) // White Rook
            {
                score += 3;
            }
            else if (piece == 5) // White Queen
            {
                score += 9;
            }
            else if (piece == 6) // White King
            {
                score += 1000000000; // A very high value for the king
            }
            else if (piece == 7) // Black Pawn
            {
                score -= 1;
            }
            else if (piece == 8) // Black Knight
            {
                score -= 5;
            }
            else if (piece == 9) // Black Bishop
            {
                score -= 3;
            }
            else if (piece == 10) // Black Rook
            {
                score -= 3;
            }
            else if (piece == 11) // Black Queen
            {
                score -= 9;
            }
            else if (piece == 12) // Black King
            {
                score -= 1000000000; // A very high value for the king
            }
            // If piece is 0, it's an empty square, do nothing (score remains the same)
        }

        return score;
    }
    */


    public bool IsWhite(int piece)
    {
        return piece >= 1 && piece <= 6; // White pieces
    }




   






   /* public List<int> MoveOrder(List<int> moves)
    {


        int[] pieceValues = { 0, 100, 500, 320, 330, 900, 0, -100, -500, -320, -330, -900, 0 };
        List<(int source, int target, int score)> moveList = new List<(int, int, int)>();

        for (int i = 0; i < moves.Count; i += 2)
        {
            int source = moves[i];
            int target = moves[i + 1];

            int srcPiece = board[source];
            int tgtPiece = board[target];

            int srcValue = Math.Abs(pieceValues[srcPiece]);
            int tgtValue = Math.Abs(pieceValues[tgtPiece]);

            // MVV-LVA: prioritize capturing higher-value pieces with lower-value pieces
            int captureScore = (tgtPiece != 0) ? (10 * tgtValue) - srcValue : 0;
            //UnityEngine.Debug.Log("move oder : " + source + " " + target + " " + captureScore);
            moveList.Add((source, target, captureScore));
        }

        // Sort moves in descending order based on captureScore.
        // If there is no capture (score = 0), sort by other criteria (e.g., piece value)
        moveList.Sort((a, b) =>
        {
            if (b.score != a.score) return b.score.CompareTo(a.score);
            // Fallback: If captureScore is the same, prioritize based on piece value
            int srcValueA = Math.Abs(pieceValues[board[a.source]]);
            int srcValueB = Math.Abs(pieceValues[board[b.source]]);
            return srcValueB.CompareTo(srcValueA);
        });

        // Convert back to list format [source, target, source, target, ...]
        List<int> sortedMoves = new List<int>();
        foreach (var move in moveList)
        {
            sortedMoves.Add(move.source);
            sortedMoves.Add(move.target);
        }

        return sortedMoves;
    }
   */




    public int globalBestSource = -1;
    public int globalBestTarget = -1;






    public List<int> MoveOrder(List<int> moves, int prevBestSource, int prevBestTarget)
    {
        int[] pieceValues = { 0, 100, 500, 320, 330, 900, 0, -100, -500, -320, -330, -900, 0 };
        //int[] pieceValues = { 0, 100, 500, 320, 330, 900, 20000, -100, -500, -320, -330, -900, -20000 };
        List<(int source, int target, int score)> moveList = new List<(int, int, int)>();

        for (int i = 0; i < moves.Count; i += 2)
        {
            int source = moves[i];
            int target = moves[i + 1];

            int srcPiece = board[source];
            int tgtPiece = board[target];

            int srcValue = Math.Abs(pieceValues[srcPiece]);
            int tgtValue = Math.Abs(pieceValues[tgtPiece]);

            // MVV-LVA: prioritize capturing higher-value pieces with lower-value pieces
             int captureScore = (tgtPiece != 0) ? (10 * tgtValue) - srcValue : 0;
            //int captureScore = (tgtPiece != 0) ? (7 * tgtValue) - srcValue : 0;

            // Assign a high priority if the move was the best move from the previous search
            //int bestMoveBonus = (source == prevBestSource && target == prevBestTarget) ? 10000 : 0;
            //int bestMoveBonus = (prevBestSource != -1 && prevBestTarget != -1 && source == prevBestSource && target == prevBestTarget) ? 11000 : 0;
            int bestMoveBonus = (prevBestSource != -1 && prevBestTarget != -1 && source == prevBestSource && target == prevBestTarget) ? 11000 : 0;



            






            moveList.Add((source, target, captureScore + bestMoveBonus));
        }

        // Sort moves in descending order based on (best move priority > captureScore > piece value)
        moveList.Sort((a, b) =>
        {
            if (b.score != a.score) return b.score.CompareTo(a.score);
            int srcValueA = Math.Abs(pieceValues[board[a.source]]);
            int srcValueB = Math.Abs(pieceValues[board[b.source]]);
            return srcValueB.CompareTo(srcValueA);
        });

        // Convert back to list format [source, target, source, target, ...]
        List<int> sortedMoves = new List<int>();
        foreach (var move in moveList)
        {
            sortedMoves.Add(move.source);
            sortedMoves.Add(move.target);
        }

        return sortedMoves;
    }





    public void HighlightBestMove(int sourceSquare, int targetSquare)
    {
        Vector3 sourcePosition = SquareToWorldPosition(sourceSquare);
        Vector3 targetPosition = SquareToWorldPosition(targetSquare);

        DrawArrow(sourcePosition, targetPosition, Color.red);
    }


    public List<GameObject> arrows = new List<GameObject>();

    public void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        GameObject arrow = Instantiate(arrowPrefab);
        arrows.Add(arrow);
        LineRenderer lineRenderer = arrow.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 5; // Ensuring closed triangle
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;

            // Arrow direction
            Vector3 direction = (end - start).normalized;

            // Length and width of arrowhead
            float arrowHeadLength = 0.4f;  // Proportional length
            float arrowHeadWidth = 0.2f;   // Proportional width

            // Perfect perpendicular vector for symmetrical arrowhead
            Vector3 perp = Vector3.Cross(direction, Vector3.forward).normalized * arrowHeadWidth;

            // Calculate arrowhead points
            Vector3 leftTip = end - (direction * arrowHeadLength) + perp;
            Vector3 rightTip = end - (direction * arrowHeadLength) - perp;

            // Set positions in LineRenderer ensuring perfect alignment
            lineRenderer.SetPosition(0, start);   // Start of arrow
            lineRenderer.SetPosition(1, end);     // Arrow tip
            lineRenderer.SetPosition(2, leftTip); // Left arrowhead
            lineRenderer.SetPosition(3, rightTip); // Right arrowhead
            lineRenderer.SetPosition(4, end);     // Closing the triangle properly

           
        }
    }



    public void DestroyAllArrows()
    {
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
        arrows.Clear();  // Clear the list after destroying
    }



    public void DisplayScore()
    {
        int whiteScore = 0;
        int blackScore = 0;

        int[] pieceValues = { 0, 1, 5, 3, 3, 9, 0, 1, 5, 3, 3, 9, 0 };
        for (int i = 0; i < board.Length; i++)
        {
            int piece = board[i];

            if (piece != 0) // If there's a piece
            {
                if (piece >= 1 && piece <= 5)
                    whiteScore += pieceValues[piece]; // White piece
                else if (piece >= 7 && piece <= 11)
                    blackScore += pieceValues[piece]; // Black piece
            }
        }

        int score = whiteScore - blackScore; // Positive = White ahead, Negative = Black ahead
        //UnityEngine.Debug.Log("Material Score: " + score);
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            //scoreText.enabled = true;
        }
        if (gameResultText != null)
        {
            if (score > 0)
                gameResultText.text = "White is ahead!";
            else if (score < 0)
                gameResultText.text = "Black is ahead!";
            else
                gameResultText.text = "Scores are equal!";
        }

    }

    public Vector3 SquareToWorldPosition(int square)
    {
        float squareSize = 2.0f;
        float boardOffsetX = -1.0f - (squareSize * 3.5f);
        float boardOffsetY = -1.0f - (squareSize * 3.5f);

        int x = square % 8;
        int y = square / 8;

        return new Vector3(boardOffsetX + x * squareSize, boardOffsetY + y * squareSize, 0);
    }


    public int CountLegalMoves(int square, int piece)
    {
        int count = 0;

        // Count occurrences in WF (White moves)

        if (piece <= 6)
        {
            if (WF != null)
            {
                for (int i = 0; i < WF.Count; i += 2) // stepping by 2 (source, target pairs)
                {
                    if (WF[i] == square)
                    {
                        count++;
                    }
                }
            }
        }


        // Count occurrences in BF (Black moves)

        else
        {
            if (BF != null)
            {
                for (int i = 0; i < BF.Count; i += 2)
                {
                    if (BF[i] == square)
                    {
                        count++;
                    }
                }
            }

            //return count;
        }


        return count;
    }











    void PrintRootMoves(List<int> rootMoves,List<int>d)
    {
        //UnityEngine.Debug.Log("black  Moves: " + string.Join(", ", d));
        //UnityEngine.Debug.Log("length: " + d.Count / 2);
        //UnityEngine.Debug.Log("root Moves and their scores: ");
        //UnityEngine.Debug.Log("rootmoves length="+rootMoves.Count / 3);
        // Ensure the list has a multiple of 3 elements (source, target, score)
        if (rootMoves.Count % 3 != 0)
        {
            UnityEngine.Debug.Log("Invalid rootMoves list. It must contain triplets of (source, target, score).");
            return;
        }

        // Create a list of tuples (source, target, score)
        List<(int source, int target, int score)> moveList = new List<(int, int, int)>();

        for (int i = 0; i < rootMoves.Count; i += 3)
        {
            moveList.Add((rootMoves[i], rootMoves[i + 1], rootMoves[i + 2]));
        }

        // Sort moves by score in ascending order
        moveList.Sort((a, b) => a.score.CompareTo(b.score));



        var top3Moves = moveList.Take(3).ToList();
        // Print each move in the required format
        foreach (var move in moveList)
        {
            UnityEngine.Debug.Log($"{move.source}, {move.target}, {move.score}");
        }
        foreach (var move in top3Moves)
        {
            UnityEngine.Debug.Log($"Best Move: {move.source} -> {move.target} with score: {move.score}");
            HighlightBestMove(move.source, move.target); // Highlighting the move
        }
    }



    void PrintRootMovesB(List<int> rootMoves, List<int> d, bool isWhite)
    {
        if (rootMoves.Count % 3 != 0)
        {
            UnityEngine.Debug.Log("Invalid rootMoves list. It must contain triplets of (source, target, score).");
            return;
        }

        // Create a list of tuples (source, target, score)
        List<(int source, int target, int score)> moveList = new List<(int, int, int)>();

        for (int i = 0; i < rootMoves.Count; i += 3)
        {
            moveList.Add((rootMoves[i], rootMoves[i + 1], rootMoves[i + 2]));
        }

        // Sort based on the player type
        if (isWhite)
        {
            moveList.Sort((a, b) => b.score.CompareTo(a.score)); // Descending for White
        }
        else
        {
            moveList.Sort((a, b) => a.score.CompareTo(b.score)); // Ascending for Black
        }

        // Take the top 3 moves after sorting
        var top3Moves = moveList.Take(3).ToList();

        // Print all moves
        foreach (var move in moveList)
        {
            UnityEngine.Debug.Log($"{move.source}, {move.target}, {move.score}");
        }

        // Print and highlight the top 3 moves
        foreach (var move in top3Moves)
        {
            UnityEngine.Debug.Log($"Best Move: {move.source} -> {move.target} with score: {move.score}");
            HighlightBestMove(move.source, move.target); // Highlighting the move
        }
    }



    void PrintRootMovesBt(List<int> rootMoves, List<int> d, bool isWhite)
    {
        if (rootMoves.Count % 3 != 0)
        {
            UnityEngine.Debug.Log("Invalid rootMoves list. It must contain triplets of (source, target, score).");
            return;
        }

        // Create a list of tuples (source, target, score)
        List<(int source, int target, int score)> moveList = new List<(int, int, int)>();

        for (int i = 0; i < rootMoves.Count; i += 3)
        {
            moveList.Add((rootMoves[i], rootMoves[i + 1], rootMoves[i + 2]));
        }

        // Sort based on player type
        if (isWhite)
        {
            moveList.Sort((a, b) => b.score.CompareTo(a.score)); // Descending for White
        }
        else
        {
            moveList.Sort((a, b) => a.score.CompareTo(b.score)); // Ascending for Black
        }

        // Ensure valid top moves
        List<(int source, int target, int score)> topMoves = new List<(int, int, int)>();

        if (moveList.Count > 0)
        {
            int bestScore = moveList[0].score; // Best move's score
            topMoves.Add(moveList[0]); // Always take the best move

            for (int i = 1; i < moveList.Count && topMoves.Count < 3; i++)
            {
                // Compare with the first move only
                if (Mathf.Abs(bestScore - moveList[i].score) <= 100)
                {
                    topMoves.Add(moveList[i]);
                }
                else
                {
                    break; // Stop if the move is a blunder
                }
            }
        }

        // Print all moves
        foreach (var move in moveList)
        {
            UnityEngine.Debug.Log($"{move.source}, {move.target}, {move.score}");
        }

        // Print and highlight only valid top moves
        foreach (var move in topMoves)
        {
            UnityEngine.Debug.Log($"Best Move: {move.source} -> {move.target} with score: {move.score}");
            HighlightBestMove(move.source, move.target); // Highlighting the move
        }




    }





    int Evaluate(bool a, bool b)
    {
        
        //evaluateCallCount++;
        int score = 0;

        if (a) // White is checkmated
        {
            return -200000; // A very low score for white being checkmated
        }
        if (b) // Black is checkmated
        {
            return 200000; // A very high score for black being checkmated
        }








        // Piece values
       // int[] pieceValues = { 0, 100, 500, 320, 330, 900, 25000, -100, -500, -320, -330, -900, -25000 };
        int[] pieceValues = { 0, 100, 500, 320, 330, 900, 0, -100, -500, -320, -330, -900, 0 };

        // Piece-square tables for positional evaluation
        int[][] pieceSquareTables = {

            // White Pawn (adjusted for your board indexing)
        new int[] {
             0,  0,  0,  0,  0,  0,  0,  0,
             5, 10, 10,-20,-20, 10, 10,  5,
             5, -5,-10,  0,  0,-10, -5,  5,
             0,  0,  0, 20, 20,  0,  0,  0,
             5,  5, 10, 25, 25, 10,  5,  5,
            10, 10, 20, 30, 30, 20, 10, 10,
            50, 50, 50, 50, 50, 50, 50, 50,
             900,  900,  900,  900,  900,  900,  900, 900
        },

        // White Rook
        new int[] {
             0,  0,  0,  5,  5,  0,  0,  0,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
             5, 10, 10, 10, 10, 10, 10,  5,
             0,  0,  0,  0,  0,  0,  0,  0
        },

        // White Knight
        new int[] {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50
        },

        // White Bishop
        new int[] {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10,-10,-10,-10,-10,-20
        },

        // White Queen
        new int[] {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
             -5,  0,  5,  5,  5,  5,  0, -5,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        },

        // White King (Middle Game)
        new int[] {
             20, 30, 10,  0,  0, 10, 30, 20,
             20, 20,  0,  0,  0,  0, 20, 20,
            -10,-20,-20,-20,-20,-20,-20,-10,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30
        }


        };

        // Loop through all squares on the board.
        for (int i = 0; i < 64; i++)
        {
            int piece = board[i]; // Get the piece at the current square.

            if (piece == 0) continue; // Skip empty squares

            // Add material value
            score += pieceValues[piece];

            // Add positional value based on piece-square tables
            if (piece <= 6) // White pieces
            {
                score += pieceSquareTables[piece - 1][i];
            }
            else // Black pieces
            {
                score -= pieceSquareTables[piece - 7][63 - i]; // Flip the table for black pieces
            }


            //int[] mobilityWeights = { 0, 1, 4, 3, 3, 2, 0, -1, -4, -3, -3, -2, 0 };

            //int mobility = CountLegalMoves(i,piece); // Function that returns number of legal moves for this piece
            //score += mobility * mobilityWeights[piece];
            //score += (int)(mobility * mobilityWeights[piece] * 0.5);

        }



        return score;
    }













}
