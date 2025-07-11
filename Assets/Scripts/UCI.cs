using System;
using UnityEngine;
using System.Collections.Generic;
public class UCI : MonoBehaviour
{



    public ulong whitePawns = Convert.ToUInt64("0000000000000000000000000000000000000000000000001111111100000000", 2);  // White pawns on rank 2 (a2-h2)
    public ulong blackPawns = Convert.ToUInt64("1111111100000000000000000000000000000000000000000000000000000000", 2);  // Black pawns on rank 7 (a7-h7)
    public ulong whiteRooks = Convert.ToUInt64("1000000000000000000000000000000000000000000000000000000000000000", 2);  // White rooks on a1 and h1
    public ulong blackRooks = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000000001", 2);  // Black rooks on a8 and h8
    public ulong whiteKnights = Convert.ToUInt64("0100000000000000000000000000000000000000000000000000000000000000", 2);  // White knights on b1 and g1
    public ulong blackKnights = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000000010", 2);  // Black knights on b8 and g8
    public ulong whiteBishops = Convert.ToUInt64("0010000000000000000000000000000000000000000000000000000000000000", 2);  // White bishops on c1 and f1
    public ulong blackBishops = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000000100", 2);  // Black bishops on c8 and f8
    public ulong whiteQueen = Convert.ToUInt64("0001000000000000000000000000000000000000000000000000000000000000", 2);  // White queen on d1
    public ulong blackQueen = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000001000", 2);  // Black queen on d8
    public ulong whiteKing = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000010000", 2);  // White king on e1
    public ulong blackKing = Convert.ToUInt64("0000000000000000000000000000000000000000000000000000000000100000", 2);  // Black king on e8
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        InitializeBoard();


    }
    public void InitializeBoard()
    {
        // Displaying the binary representation of the bitboards
        Debug.Log("White Pawns: " + Convert.ToString((long)whitePawns, 2).PadLeft(64, '0'));
        Debug.Log("Black Pawns: " + Convert.ToString((long)blackPawns, 2).PadLeft(64, '0'));
        Debug.Log("White Rooks: " + Convert.ToString((long)whiteRooks, 2).PadLeft(64, '0'));
        Debug.Log("Black Rooks: " + Convert.ToString((long)blackRooks, 2).PadLeft(64, '0'));
        Debug.Log("White Knights: " + Convert.ToString((long)whiteKnights, 2).PadLeft(64, '0'));
        Debug.Log("Black Knights: " + Convert.ToString((long)blackKnights, 2).PadLeft(64, '0'));
        Debug.Log("White Bishops: " + Convert.ToString((long)whiteBishops, 2).PadLeft(64, '0'));
        Debug.Log("Black Bishops: " + Convert.ToString((long)blackBishops, 2).PadLeft(64, '0'));
        Debug.Log("White Queen: " + Convert.ToString((long)whiteQueen, 2).PadLeft(64, '0'));
        Debug.Log("Black Queen: " + Convert.ToString((long)blackQueen, 2).PadLeft(64, '0'));
        Debug.Log("White King: " + Convert.ToString((long)whiteKing, 2).PadLeft(64, '0'));
        Debug.Log("Black King: " + Convert.ToString((long)blackKing, 2).PadLeft(64, '0'));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}



