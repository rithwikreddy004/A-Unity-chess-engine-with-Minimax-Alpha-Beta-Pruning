



using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;

public class ChessGUI : MonoBehaviour
{
    public int boardSize = 8;


    public Material moveDotMaterial;


    public Sprite[] pieceSprites;

    public GameObject piecePrefab;

    private GameObject[] pieceObjects = new GameObject[64];
    public Color whiteColor = new Color(240f / 255f, 217f / 255f, 181f / 255f); //#8D8EBC
    //public Color whiteColor = new Color(141f / 255f, 142f / 255f, 188f / 255f);

    public Color blackColor = new Color(181f / 255f, 136f / 255f, 99f / 255f); //#191C25
    //public Color blackColor = new Color(25f / 255f, 28f / 255f, 37f / 255f);

    //public Color whiteColor;
    //public Color blackColor;
    public ChessRules chessRules;



    public TextMeshProUGUI gameResultText;

    private List<GameObject> moveDots = new List<GameObject>(); // List to keep track of move dots


    void Start()
    {

        //ColorUtility.TryParseHtmlString("#8D8EBC", out whiteColor);
        //ColorUtility.TryParseHtmlString("#191C25", out whiteColor);




        if (gameResultText != null)
        {
            gameResultText.enabled = false;
        }
        else
        {
            Debug.LogError("Game result text is not assigned in the Inspector!");
        }
        InitializePieceSprites();
        GenerateBoard();
        if (chessRules != null)
        {
            chessRules.InitializeBoard(); // Initialize the logic
            chessRules.chessGUI.UpdateBoard(chessRules.board); // Sync visuals
            //chessRules.chessGUI.UpdateBoard();
        }
    }


    void InitializePieceSprites()
    {
        pieceSprites = new Sprite[12];

        // Load the piece sprites by name (make sure the PNG files are in the Assets/Resources folder)
        pieceSprites[0] = Resources.Load<Sprite>("WhitePawn");
        pieceSprites[1] = Resources.Load<Sprite>("WhiteRook");
        pieceSprites[2] = Resources.Load<Sprite>("WhiteKnight");
        pieceSprites[3] = Resources.Load<Sprite>("WhiteBishop");
        pieceSprites[4] = Resources.Load<Sprite>("WhiteQueen");
        pieceSprites[5] = Resources.Load<Sprite>("WhiteKing");

        pieceSprites[6] = Resources.Load<Sprite>("BlackPawn");
        pieceSprites[7] = Resources.Load<Sprite>("BlackRook");
        pieceSprites[8] = Resources.Load<Sprite>("BlackKnight");
        pieceSprites[9] = Resources.Load<Sprite>("BlackBishop");
        pieceSprites[10] = Resources.Load<Sprite>("BlackQueen");
        pieceSprites[11] = Resources.Load<Sprite>("BlackKing");
    }
    void GenerateBoard()
    {
        float tileSize = 2.0f; // Size of each square (adjust to remove gaps)
        float boardOffset = (boardSize * tileSize) / 2; // Adjust to center the board

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                // Create a square using a Quad mesh
                GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
                square.name = $"Square_{row}_{col}";

                // Adjust position so squares are adjacent with no gap
                square.transform.position = new Vector3(col * tileSize - boardOffset, row * tileSize - boardOffset, 0);
                Material squareMaterial = new Material(Shader.Find("Sprites/Default"));
                squareMaterial.color = (row + col) % 2 == 0 ? blackColor : whiteColor;
                // Add color to the square
                Renderer squareRenderer = square.GetComponent<Renderer>();
                //squareRenderer.material.color = (row + col) % 2 == 0 ? whiteColor : blackColor;
                
                squareRenderer.material = squareMaterial;
                //squareRenderer.material.color = (row + col) % 2 == 0 ? blackColor : whiteColor;

                // Optionally, scale the square to the desired size
                square.transform.localScale = new Vector3(tileSize, tileSize, 1);
            }
        }
    }

    public GameObject highlightedSquare;
    public GameObject highlightedSquare2;
    public GameObject bothighlightedSquare;
    public GameObject bothighlightedSquare2;
    public GameObject highlightedStartSquare;
    public GameObject highlightedTargetSquare;
    public int count = 0;
    public int sx = 0;
    public int sy = 0;
    public int x = 0;


    public bool turn = false;
    

    public int d1 = 0;
    public int d2 = 0;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            count += 1;
           // turn = !turn;
             HandleClick();
           
           // Debug.Log("count "+count);
        }
    }

    public void HandleClick()
    {
        chessRules.gen();
        List<int> wfCopy = new List<int>(chessRules.WF);
        List<int> bfCopy = new List<int>(chessRules.BF);
        if (count % 2 != 0)
        {


            if (highlightedSquare != null)
            {
                //Debug.Log("destroying...");
                Destroy(highlightedSquare);

                // Remove the previous highlight
            }
            if (highlightedSquare2 != null)
            {
                //Debug.Log("destroying...");
                Destroy(highlightedSquare2);

                // Remove the previous highlight
            }
            ClearMoveDots();

            
            
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure the z-axis is set to 0
                                 // Debug.Log(Math.Round(mousePosition.x)+" "+ Math.Round(mousePosition.y));
            Vector3 startingPosition = mousePosition;
            float squareSize = 2.0f;
            //int roundedX = (int)Math.Round(mousePosition.x);
            //int roundedY = (int)Math.Round(mousePosition.y);
            //int roundedX = (int)Math.Ceiling(mousePosition.x);
            //int roundedY = (int)Math.Ceiling(mousePosition.y);
            //Debug.Log("1st click "+roundedX + " " + roundedY);

            // sx = roundedX;
            //sy = roundedY;
            float boardOffsetX = -1.0f - (squareSize * 3.5f); // Adjusting for center at -1
            float boardOffsetY = -1.0f - (squareSize * 3.5f); // Adjusting for center at -1

            // Convert world position to board indices
            float offset = 0.01f; // Tiny adjustment to push values correctly

            // Convert world position to board indices
            int boardX = Mathf.RoundToInt((mousePosition.x - boardOffsetX + offset) / squareSize);
            int boardY = Mathf.RoundToInt((mousePosition.y - boardOffsetY + offset) / squareSize);
            // int boardX = Mathf.FloorToInt((mousePosition.x - boardOffsetX) / squareSize);
            // int boardY = Mathf.FloorToInt((mousePosition.y - boardOffsetY) / squareSize);
            
            // Ensure indices are within bounds
            boardX = Mathf.Clamp(boardX, 0, 7);
            boardY = Mathf.Clamp(boardY, 0, 7);

            

            // Convert to 0-63 array index
            int ss = boardY * 8 + boardX;

            Debug.Log($"Selected square: {ss} (X: {boardX}, Y: {boardY})");

            // Ensure the coordinates are within bounds


            // Convert to 0-63 index


            //HighlightSquare(new Vector3(sx, sy, 0));

            //int ss = ((int)Math.Round(mousePosition.y + 8.0f) / 2) * boardSize +
            //((int)Math.Round(mousePosition.x + 8.0f) / 2);
            // int ss = ((int)(mousePosition.y + 8.0f) / 2) * boardSize +
            //((int)(mousePosition.x + 8.0f) / 2);
            d1 = ss;
            HighlightSquare(d1);

            int p = chessRules.board[ss];
            chessRules.Record(p);
            chessRules.Record(ss);
            chessRules.gen();
            
            //Debug.Log("Selected Square: " + ss);

            //chessRules.Genw(chessRules.board);
            /*if (chessRules.BF.Count == 0) // No moves left for black (checkmate or stalemate)
            {
                gameResultText.text = "White wins!"; // Update the text dynamically
                gameResultText.enabled = true;
                //Debug.Log("White wins! Black has no legal moves.");
            }
            else if (chessRules.WF.Count == 0) // No moves left for black (checkmate or stalemate)
            {
                gameResultText.text = "Black wins!"; // Update the text dynamically
                gameResultText.enabled = true;
                
                //Debug.Log("Black wins!");
            }*/
            // Debug.Log("white highlighter Moves: " + string.Join(", ", chessRules.WF));
            //Debug.Log("black highlighter Moves: " + string.Join(", ", chessRules.BF));
            // else
            // {
            HighlightPossibleMoves(chessRules.WF, ss);
            HighlightPossibleMoves(chessRules.BF, ss);

            //HighlightPossibleMoves(chessRules.wpm, ss);
            //HighlightPossibleMoves(chessRules.bpm, ss);



            //Debug.Log("wcm count "+chessRules.WCM.Count);
            Highlightenp(chessRules.WCM, ss);
                Highlightenp(chessRules.BCM, ss);
            //Debug.Log("wep count " + chessRules.WEP.Count);
           // Debug.Log("wep  " + chessRules.WEP);
           // Debug.Log("wep " + string.Join(", ", chessRules.WEP));
           // Debug.Log("bep count " + chessRules.BEP.Count);
            //Debug.Log("bep " + chessRules.BEP);
            //Debug.Log("bep " + string.Join(", ", chessRules.BEP));
            HighlightPossibleMoves(chessRules.WEP, ss);
            HighlightPossibleMoves(chessRules.BEP, ss);
            //}


            // Log the rounded positions for debugging
            //Debug.Log("Start Position: " + roundedX + ", " + roundedY);

            


        }

        // Get the target position (after mouse is released, for example)
        else {
            ClearMoveDots();
            /*if (highlightedSquare != null)
            {
                //Debug.Log("destroying...");
                Destroy(highlightedSquare);

                // Remove the previous highlight
            }*/

            if (bothighlightedSquare != null)
            {
                //Debug.Log("destroying...");
                Destroy(bothighlightedSquare);
                
                // Remove the previous highlight
            }
            if (bothighlightedSquare2 != null)
            {
                //Debug.Log("destroying...");
                Destroy(bothighlightedSquare2);

                // Remove the previous highlight
            }
            
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mp.z = 0;
            Vector3 targetPosition = mp;
            

            float squareSize = 2.0f;
            
            float boardOffsetX = -1.0f - (squareSize * 3.5f); // Adjusting for center at -1
            float boardOffsetY = -1.0f - (squareSize * 3.5f); // Adjusting for center at -1

            float offset = 0.01f; // Tiny adjustment to push values correctly

            // Convert world position to board indices
            int targetX = Mathf.RoundToInt((mp.x - boardOffsetX + offset) / squareSize);
            int targetY = Mathf.RoundToInt((mp.y - boardOffsetY + offset) / squareSize);
            
            

            // Ensure indices are within bounds
            targetX = Mathf.Clamp(targetX, 0, 7);
            targetY = Mathf.Clamp(targetY, 0, 7);




            
            // Convert to 0-63 array index
            int ss = targetY * 8 + targetX;

            Debug.Log($"Selected square: {ss} (X: {targetX}, Y: {targetY})");
            HighlightSquare2(ss);

            // int endSquare = ConvertToSquareIndex(targetX, targetY);
            bool isWhite = (count % 2 != 0);  // White moves on odd turns, black on even turns
                                              // int pieceType = GetPieceType(sx, sy);
            //int ss = ((int)Math.Round(mp.y + 8.0f) / 2) * boardSize +
                   //   ((int)Math.Round(mp.x + 8.0f) / 2);



            //int ss = ((int)(mp.y + 8.0f) / 2) * boardSize +
                      //((int)(mp.x + 8.0f) / 2);
            d2 = ss;
            int p = chessRules.board[ss];
            //Debug.Log("Target Position: " + targetX + ", " + targetY);
            // chessRules.MovePiece(new Vector3(sx, sy, 0), new Vector3(targetX, targetY, 0));


            bool enpassant = false;
            bool isCastlingMove = false;
            //Debug.Log("wcm count "+chessRules.WCM.Count);
            for (int i = 0; i < chessRules.WCM.Count; i += 4)
            {
                //Debug.Log("d1 " + d1);
                //Debug.Log("d2 " + d2);
                //Debug.Log("wcm[0] " + chessRules.WCM[i]);
                //Debug.Log("wcm[1] " + chessRules.WCM[i+1]);
                if (d1 == chessRules.WCM[i] && d2 == chessRules.WCM[i + 1])
                {
                    chessRules.MoveC(d1, d2, chessRules.WCM[i + 2], chessRules.WCM[i + 3]);
                    isCastlingMove = true; // Castling move detected
                    break; // Exit the loop as we found a match
                }
            }



            for (int i = 0; i < chessRules.BCM.Count; i += 4)
            {
                //Debug.Log("d1 " + d1);
                //Debug.Log("d2 " + d2);
                //Debug.Log("wcm[0] " + chessRules.WCM[i]);
                //Debug.Log("wcm[1] " + chessRules.WCM[i+1]);
                if (d1 == chessRules.BCM[i] && d2 == chessRules.BCM[i + 1])
                {
                    chessRules.MoveC(d1, d2, chessRules.BCM[i + 2], chessRules.BCM[i + 3]);
                    isCastlingMove = true; // Castling move detected
                    break; // Exit the loop as we found a match
                }
            }

            // If no castling move was detected, execute the normal move
           
            


            for(int i = 0; i < chessRules.WEP.Count; i += 2)
            {
                if(d1==chessRules.WEP[i] && d2 == chessRules.WEP[i + 1])
                {
                    chessRules.MoveE(d1, d2);
                    enpassant = true;
                    break;
                }
            }


            for (int i = 0; i < chessRules.BEP.Count; i += 2)
            {
                if (d1 == chessRules.BEP[i] && d2 == chessRules.BEP[i + 1])
                {
                    chessRules.MoveE(d1, d2);
                    enpassant = true;
                    break;
                }
            }
            if (!isCastlingMove && !enpassant)
            {
                if (IsValidMove(d1, d2, wfCopy) || IsValidMove(d1, d2, bfCopy))
                {
                    chessRules.MovePieceIG(d1, d2);
                }
                else
                {
                    Debug.Log("Invalid move!");
                }

                //chessRules.MovePieceIG(d1, d2);
            }











            chessRules.gen2();
            //chessRules.Record(ss);
            //chessRules.Record(p);
            if (chessRules.BF.Count == 0) // No moves left for black (checkmate or stalemate)
            {
                gameResultText.text = "White won!"; // Update the text dynamically
                gameResultText.enabled = true;
                //Debug.Log("White wins! Black has no legal moves.");
            }
            if (chessRules.WF.Count == 0) // No moves left for black (checkmate or stalemate)
            {
                gameResultText.text = "Black won!"; // Update the text dynamically
                gameResultText.enabled = true;

                //Debug.Log("Black wins!");
            }
            //chessRules.gen();
            // chessRules.whiteTurn = !chessRules.whiteTurn;
            //chessRules.MovePiece(x, endSquare, isWhite, pieceType);
            //HighlightSquares(new Vector3(sx, sy, 0),new Vector3(targetX,targetY,0));
            

        }
        

        



        

     
        
    }










    public void HighlightSquare(int squareNumber)
    {
        

        float squareSize = 2.0f;
        float boardOffsetX = -1.0f - (squareSize * 3.5f);
        float boardOffsetY = -1.0f - (squareSize * 3.5f);

        int x = squareNumber % 8; // Column (file)
        int y = squareNumber / 8; // Row (rank)

        // Convert to Unity world position using the same offsets
        Vector3 position = new Vector3(
            boardOffsetX + (x * squareSize),
            boardOffsetY + (y * squareSize),
            -1 // Keep it in front of the board
        );

        highlightedSquare = GameObject.CreatePrimitive(PrimitiveType.Quad);
        highlightedSquare.transform.position = position;
        highlightedSquare.transform.localScale = new Vector3(squareSize, squareSize, 1);
        Material squareMaterial = new Material(Shader.Find("Sprites/Default"));
        Color forestGreen;
        ColorUtility.TryParseHtmlString("#aaa23a", out forestGreen);
        highlightedSquare.GetComponent<Renderer>().material = squareMaterial;
        highlightedSquare.GetComponent<Renderer>().material.color = forestGreen;
        highlightedSquare.GetComponent<Collider>().enabled = false; // Disable collider
    }





    public void botHighlightSquare(int squareNumber)
    {
        if (highlightedSquare != null)
        {
            //Debug.Log("destroying...");
            Destroy(highlightedSquare);

            // Remove the previous highlight
        }
        if (highlightedSquare2 != null)
        {
            //Debug.Log("destroying...");
            Destroy(highlightedSquare2);

            // Remove the previous highlight
        }


        float squareSize = 2.0f;
        float boardOffsetX = -1.0f - (squareSize * 3.5f);
        float boardOffsetY = -1.0f - (squareSize * 3.5f);

        int x = squareNumber % 8; // Column (file)
        int y = squareNumber / 8; // Row (rank)

        // Convert to Unity world position using the same offsets
        Vector3 position = new Vector3(
            boardOffsetX + (x * squareSize),
            boardOffsetY + (y * squareSize),
            -1 // Keep it in front of the board
        );

        bothighlightedSquare = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bothighlightedSquare.transform.position = position;
        bothighlightedSquare.transform.localScale = new Vector3(squareSize, squareSize, 1);
        Material squareMaterial = new Material(Shader.Find("Sprites/Default"));
        Color forestGreen;
        ColorUtility.TryParseHtmlString("#aaa23a", out forestGreen);
        bothighlightedSquare.GetComponent<Renderer>().material = squareMaterial;
        bothighlightedSquare.GetComponent<Renderer>().material.color = forestGreen;
        bothighlightedSquare.GetComponent<Collider>().enabled = false; // Disable collider
    }




    public void HighlightSquare2(int squareNumber)
    {


        float squareSize = 2.0f;
        float boardOffsetX = -1.0f - (squareSize * 3.5f);
        float boardOffsetY = -1.0f - (squareSize * 3.5f);

        int x = squareNumber % 8; // Column (file)
        int y = squareNumber / 8; // Row (rank)

        // Convert to Unity world position using the same offsets
        Vector3 position = new Vector3(
            boardOffsetX + (x * squareSize),
            boardOffsetY + (y * squareSize),
            -1 // Keep it in front of the board
        );

        highlightedSquare2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
        highlightedSquare2.transform.position = position;
        highlightedSquare2.transform.localScale = new Vector3(squareSize, squareSize, 1);
        Material squareMaterial = new Material(Shader.Find("Sprites/Default"));
        Color forestGreen;
        ColorUtility.TryParseHtmlString("#cdd26a", out forestGreen);
        highlightedSquare2.GetComponent<Renderer>().material = squareMaterial;
        highlightedSquare2.GetComponent<Renderer>().material.color = forestGreen;
        highlightedSquare2.GetComponent<Collider>().enabled = false; // Disable collider
    }

    public void botHighlightSquare2(int squareNumber)
    {


        float squareSize = 2.0f;
        float boardOffsetX = -1.0f - (squareSize * 3.5f);
        float boardOffsetY = -1.0f - (squareSize * 3.5f);

        int x = squareNumber % 8; // Column (file)
        int y = squareNumber / 8; // Row (rank)

        // Convert to Unity world position using the same offsets
        Vector3 position = new Vector3(
            boardOffsetX + (x * squareSize),
            boardOffsetY + (y * squareSize),
            -1 // Keep it in front of the board
        );

        bothighlightedSquare2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bothighlightedSquare2.transform.position = position;
        bothighlightedSquare2.transform.localScale = new Vector3(squareSize, squareSize, 1);
        Material squareMaterial = new Material(Shader.Find("Sprites/Default"));
        Color forestGreen;
        ColorUtility.TryParseHtmlString("#cdd26a", out forestGreen);
        bothighlightedSquare2.GetComponent<Renderer>().material = squareMaterial;
        bothighlightedSquare2.GetComponent<Renderer>().material.color = forestGreen;
        bothighlightedSquare2.GetComponent<Collider>().enabled = false; // Disable collider
    }







    public bool IsValidMove(int d1, int d2, List<int> possibleMoves)
    {
        for (int i = 0; i < possibleMoves.Count; i += 2) // Step through pairs of (source, target)
        {
            if (possibleMoves[i] == d1 && possibleMoves[i + 1] == d2)
            {
                return true; // Move is valid
            }
        }
        return false; // Move is not in the list
    }















    public void HighlightPossibleMoves(List<int> possibleMoves, int selectedSquare)
    {
        // Clear existing dots
       /* foreach (GameObject dot in moveDots)
        {
            Destroy(dot);
        }
        moveDots.Clear();*/

        // Filter moves and add dots for target squares of the selected piece
        for (int i = 0; i < possibleMoves.Count; i += 2)
        {
            int startSquare = possibleMoves[i];
            int targetSquare = possibleMoves[i + 1];

            // Only consider moves starting from the selected square
            if (startSquare == selectedSquare)
            {
                int row = targetSquare / boardSize;
                int col = targetSquare % boardSize;

                GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                dot.transform.position = new Vector3((col * 2.0f - 7.0f) - 1, (row * 2.0f - 7.0f) - 1, 0);
                dot.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                //Material squareMaterial = new Material(Shader.Find("Unlit/Color"));
                dot.GetComponent<Renderer>().material = moveDotMaterial;
                // Material transparentMaterial = new Material(Shader.Find("Standard"));
                //transparentMaterial.color = new Color(1, 0, 0, 0.5f); // Semi-transparent red
                //dot.GetComponent<Renderer>().material = transparentMaterial;
                //dot.GetComponent<Renderer>().material.color = Color.green;
                Color forestGreen;
                ColorUtility.TryParseHtmlString("#5c8e23", out forestGreen);
                //dot.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.75f); ;
                //dot.GetComponent<Renderer>().material.color = forestGreen;
                //squareMaterial.SetColor("_Color", forestGreen);
                //dot.GetComponent<Renderer>().material = squareMaterial;
               // dot.GetComponent<Renderer>().material.renderQueue = 3000;
               // squareMaterial.SetFloat("_Mode", 3);
                //dot.GetComponent<Renderer>().material.renderQueue = 5000;
                dot.GetComponent<Collider>().enabled = false; // Disable the collider for the dots
                moveDots.Add(dot);
            }
        }
    }



    public void Highlightenp(List<int> possibleMoves, int selectedSquare)
    {
        // Clear existing dots
       // chessRules.GenerateCastlingMoves()
        /* foreach (GameObject dot in moveDots)
         {
             Destroy(dot);
         }
         moveDots.Clear();*/
        
       // Debug.Log("select sqaure " + selectedSquare);
        //Debug.Log("PMC " + possibleMoves.Count);

        // Filter moves and add dots for target squares of the selected piece
        for (int i = 0; i < possibleMoves.Count; i += 4)
        {
            int startSquare = possibleMoves[i];
            int targetSquare = possibleMoves[i + 1];
           // Debug.Log("start sqaure " + startSquare);
            
            // Only consider moves starting from the selected square
            if (startSquare == selectedSquare)
            {
                //Debug.Log("castling sqaure "+targetSquare);
                int row = targetSquare / boardSize;
                int col = targetSquare % boardSize;

                GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                dot.transform.position = new Vector3((col * 2.0f - 7.0f) - 1, (row * 2.0f - 7.0f) - 1, 0);
                dot.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                Material squareMaterial = new Material(Shader.Find("Sprites/Default"));
                // Material transparentMaterial = new Material(Shader.Find("Standard"));
                //transparentMaterial.color = new Color(1, 0, 0, 0.5f); // Semi-transparent red
                //dot.GetComponent<Renderer>().material = transparentMaterial;
                //dot.GetComponent<Renderer>().material.color = Color.green;
                dot.GetComponent<Renderer>().material = squareMaterial;
                dot.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.75f); ;
                //dot.GetComponent<Renderer>().material.renderQueue = 5000;
                dot.GetComponent<Collider>().enabled = false; // Disable the collider for the dots
                moveDots.Add(dot);
            }
        }
    }










    // Call this method to clear all highlighted dots (e.g., after a move)
    public void ClearMoveDots()
    {
        foreach (GameObject dot in moveDots)
        {
            Destroy(dot);
        }
        moveDots.Clear();
    }

    public static bool ex = false;


    public void UpdateBoard(int[] board)
    {
        // Clear previous pieces
        foreach (GameObject piece in pieceObjects)
        {
            if (piece != null)
            {
                Destroy(piece);  // Destroy the previous piece (if any)
            }
        }

        // Iterate over the board array and place pieces at the correct positions
        for (int i = 0; i < 64; i++)
        {
            int pieceType = board[i];
            if (pieceType != 0) // If the square isn't empty
            {
                GameObject piece = new GameObject($"Piece_{i}");

                // Get the position of the square and adjust for correct placement
                Vector3 position = GetPositionFromIndex(i);

                // Adjust the piece's position to center it in the square
               // Debug.Log($"Piece {i} - Position - x: {position.x}, y: {position.y}");

                piece.transform.position = new Vector3(position.x-1, position.y-1, -1);


                // Add a SpriteRenderer component and assign the appropriate sprite
                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                if (pieceType >= 1 && pieceType <= pieceSprites.Length)
                {
                    sr.sprite = pieceSprites[pieceType - 1];  // Valid pieceType
                }
                else
                {
                   // Debug.LogError($"Invalid pieceType at index {i}: {pieceType}. Skipping this piece.");
                    Destroy(piece);  // Clean up the invalid piece
                    continue;
                }

                sr.sortingOrder = 1; // Ensure pieces appear above the squares

                // Scale the piece to fit inside the square
                piece.transform.localScale = new Vector3(1.5f, 1.5f, 1); // Adjust this value as needed

                pieceObjects[i] = piece;  // Keep track of the piece for future reference
            }
        }




       

        
    }

    public void UpdateBoardB(int[] board)
    {
        // 1. Clear previous pieces
        foreach (GameObject piece in pieceObjects)
        {
            if (piece != null)
            {
                Destroy(piece);
            }
        }

        // 2. Place new pieces in a mirrored way
        for (int i = 0; i < 64; i++)
        {
            // "blackIndex" is the mirrored index: 0 -> 63, 1 -> 62, etc.
            int blackIndex = 63 - i;
            int pieceType = board[blackIndex];

            if (pieceType != 0) // If the square isn't empty
            {
                GameObject piece = new GameObject($"Piece_{blackIndex}");

                // Use your same GetPositionFromIndex(i), but feed it 'i'
                // so the piece ends up in the reversed location visually
                Vector3 position = GetPositionFromIndex(i);

                // Adjust for centering (same offset as your White version)
                piece.transform.position = new Vector3(position.x - 1, position.y - 1, -1);

                // 3. Add a SpriteRenderer and assign the appropriate sprite
                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                if (pieceType >= 1 && pieceType <= pieceSprites.Length)
                {
                    sr.sprite = pieceSprites[pieceType - 1];
                }
                else
                {
                    Destroy(piece);  // Clean up invalid piece
                    continue;
                }
                sr.sortingOrder = 1;

                // 4. Scale piece to fit inside square
                piece.transform.localScale = new Vector3(1.5f, 1.5f, 1);

                // 5. Keep track of the piece for future reference
                pieceObjects[blackIndex] = piece;
            }
        }
    }





    Vector3 GetPositionFromIndex(int index)
    {
        int row = index / boardSize;
        int col = index % boardSize;

        // Position the pieces at the center of each square
        return new Vector3(col * 2.0f - 7.0f, row * 2.0f - 7.0f, 0);
    }



    




    

}





