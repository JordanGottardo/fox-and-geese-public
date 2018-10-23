using UnityEngine;
using System;
using System.Collections.Generic;
using FoxAndGeese;

//JORDAN ma i tasselli che non inizializzo non sono null

public class MyUtility {
    private static Dictionary<String, PawnType[,]> winningBoards;
    private static Dictionary<String, List<Move>> correctMoves;

    // Sezione mosse corrette ===================================================
    // ritorna un dizionario di array di mosse corrette
    // una mossa corretta è restare anche nella stessa casella in questo caso
    public static Dictionary<String, List<Move>> CreateCorrectMoves() {
        //inizializzo il campo dati
        correctMoves = new Dictionary<string, List<Move>>();
        for (int r = 0; r < 7; r++) {
            for (int c = 0; c < 7; c++) {
                // se la posizione non è nella board non fare niente;
                if (!IsPositionOutOfCross(r, c)) {
                    if ((r + c) % 2 == 0) {
                        // nei casi pari ho la diagonale
                        correctMoves.Add(r.ToString() + c.ToString(), InitCorrectMovesEven(r, c));
                    } else {
                        //nei casi dispari non ho la diagonale
                        correctMoves.Add(r.ToString() + c.ToString(), InitCorrectMovesOdd(r, c));
                    }
                }
            }
        }
        return correctMoves;
        /*
        //Debug.Log("CreateCorrectMoves - inizio");
        //inizializzo il campo dati
        correctMoves = new Dictionary<string, Move[]>();
        // assegno i 5 centri
        correctMoves.Add("13", InitCorrectMovesFromCenter(1, 3));
        correctMoves.Add("31", InitCorrectMovesFromCenter(3, 1));
        correctMoves.Add("33", InitCorrectMovesFromCenter(3, 3));
        correctMoves.Add("35", InitCorrectMovesFromCenter(3, 5));
        correctMoves.Add("53", InitCorrectMovesFromCenter(5, 3));
        //Debug.Log("CreateCorrectMoves - finiti i 5 centri");
        // assegno gli angoli estremi che sono classificabili in 4 categorie
        correctMoves.Add("02", InitCorrectMovesTopLeftCorner(0, 2));
        correctMoves.Add("20", InitCorrectMovesTopLeftCorner(2, 0));
        correctMoves.Add("04", InitCorrectMovesTopRightCorner(0, 4));
        correctMoves.Add("26", InitCorrectMovesTopRightCorner(2, 6));
        correctMoves.Add("62", InitCorrectMovesBottomLeftCorner(6, 2));
        correctMoves.Add("40", InitCorrectMovesBottomLeftCorner(4, 0));
        correctMoves.Add("64", InitCorrectMovesBottomRightCorner(6, 4));
        correctMoves.Add("46", InitCorrectMovesBottomRightCorner(4, 6));
        //Debug.Log("CreateCorrectMoves - finiti gli angoli estremi");
        // assegno i lati che sono classificabili in 4 categorie
        correctMoves.Add("03", InitCorrectMovesTopCenterEdge(0, 3));
        correctMoves.Add("21", InitCorrectMovesTopCenterEdge(2, 1));
        correctMoves.Add("25", value: InitCorrectMovesTopCenterEdge(2, 5));
        correctMoves.Add("63", InitCorrectMovesBottomCenterEdge(6, 3));
        correctMoves.Add("41", InitCorrectMovesBottomCenterEdge(4, 1));
        correctMoves.Add("45", InitCorrectMovesBottomCenterEdge(4, 5));
        correctMoves.Add("30", InitCorrectMovesLeftCenterEdge(3, 0));
        correctMoves.Add("12", InitCorrectMovesLeftCenterEdge(1, 2));
        correctMoves.Add("52", InitCorrectMovesLeftCenterEdge(5, 2));
        correctMoves.Add("36", InitCorrectMovesRightCenterEdge(3, 6));
        correctMoves.Add("14", InitCorrectMovesRightCenterEdge(1, 4));
        correctMoves.Add("54", InitCorrectMovesRightCenterEdge(5, 4));
        //Debug.Log("CreateCorrectMoves - finiti i lati");
        // assegno i 4 angoli concavi 22 24 42 44
        correctMoves.Add("22", InitCorrectMovesConcaveCorner(2, 2));
        correctMoves.Add("42", InitCorrectMovesConcaveCorner(4, 2));
        correctMoves.Add("24", InitCorrectMovesConcaveCorner(2, 4));
        correctMoves.Add("44", InitCorrectMovesConcaveCorner(4, 4));
        //Debug.Log("CreateCorrectMoves - finiti i concavi");
        // assegno nel quadrato centrale i punti medi dei suoi 4 lati
        correctMoves.Add("23", InitCorrectMovesCenterQuad(2, 3));
        correctMoves.Add("32", InitCorrectMovesCenterQuad(3, 2));
        correctMoves.Add("34", InitCorrectMovesCenterQuad(3, 4));
        correctMoves.Add("43", InitCorrectMovesCenterQuad(4, 3));
        //Debug.Log("CreateCorrectMoves - finito tutto");
        return correctMoves;
        */
    }

    //inizializza le mosse corrette per le celle pari (diagonale si)
    private static List<Move> InitCorrectMovesEven(int r, int c) {
        List<Move> correctMoves = new List<Move>();
        //setto l'intorno del centro dato come oche
        for (int roundR = r - 1; roundR <= r + 1; roundR++) {
            for (int roundC = c - 1; roundC <= c + 1; roundC++) {
                if (!IsPositionOutOfCross(roundR, roundC)) {
                    correctMoves.Add(new Move(PawnType.Goose, r, c, roundR, roundC));
                    correctMoves.Add(new Move(PawnType.Fox, r, c, roundR, roundC));
                }
            }
        }
        return correctMoves;
    }

    //inizializza le mosse corrette per le celle dispari (diagonale no)
    private static List<Move> InitCorrectMovesOdd(int r, int c) {
        //Debug.Log("Sono triste: " + r + c);
        List<Move> correctMoves = new List<Move>();
        //setto l'intorno del centro dato come oche
        for (int roundR = r - 1; roundR <= r + 1; roundR++) {
            if (!IsPositionOutOfCross(roundR, c)) {
                //Debug.Log("Sono triste: " + roundR + c + !IsPositionOutOfCross(roundR, c));
                correctMoves.Add(new Move(PawnType.Fox, r, c, roundR, c));
                correctMoves.Add(new Move(PawnType.Goose, r, c, roundR, c));
            }
        }
        for (int roundC = c - 1; roundC <= c + 1; roundC++) {
            if (!IsPositionOutOfCross(r, roundC)) {
                //Debug.Log("Sono triste: " + r + roundC + !IsPositionOutOfCross(r, roundC));
                correctMoves.Add(new Move(PawnType.Fox, r, c, r, roundC));
                correctMoves.Add(new Move(PawnType.Goose, r, c, r, roundC));
            }
        }
        return correctMoves;
    }

    // Sezione winning boards ==================================================
    // ritorna un dizionario di array di board parziali vincenti per le oche
    public static Dictionary<String, PawnType[,]> CreateWinningBoards() {
        //inizializzo il campo dati
        winningBoards = new Dictionary<string, PawnType[,]>();
        for (int r = 0; r < 7; r++) {
            for (int c = 0; c < 7; c++) {
                // se la posizione non è nella board non fare niente;
                if (!IsPositionOutOfCross(r, c)) {
                    if ((r + c) % 2 == 0) {
                        // nei casi pari ho la diagonale
                        winningBoards.Add(r.ToString() + c.ToString(), InitWinningBoardEven(r, c));
                    } else {
                        //nei casi dispari non ho la diagonale
                        winningBoards.Add(r.ToString() + c.ToString(), InitWinningBoardOdd(r, c));
                    }
                }
            }
        }
        return winningBoards;
    }

    //inizializza le winning boards per le celle pari (diagonale si)
    private static PawnType[,] InitWinningBoardEven(int r, int c) {
        PawnType[,] oneBoard = new PawnType[7, 7];
        //setto l'intorno del centro dato come oche
        for (int roundR = r - 1; roundR <= r + 1; roundR++) {
            for (int roundC = c - 1; roundC <= c + 1; roundC++) {
                if (!IsPositionOutOfCross(roundR, roundC)) {
                    oneBoard[roundR, roundC] = PawnType.Goose;
                }
            }
        }
        int roundR2 = r - 2;
        while (roundR2 <= r + 2) {
            int roundC = c - 2;
            while(roundC <= c + 2) {
                if (!IsPositionOutOfCross(roundR2, roundC)) {
                    //Debug.Log("Sono triste: " + roundR2 + roundC + !IsPositionOutOfCross(roundR2, roundC));
                    oneBoard[roundR2, roundC] = PawnType.Goose;
                }
                roundC = roundC + 2;
            }
            roundR2 = roundR2 + 2;
        }
        // setto il centro come volpe
        oneBoard[r, c] = PawnType.Fox;
        return oneBoard;
    }

    //inizializza le winning boards per le celle dispari (diagonale no)
    private static PawnType[,] InitWinningBoardOdd(int r, int c) {
        //Debug.Log("Sono triste: " + r + c);
        PawnType[,] oneBoard = new PawnType[7, 7];
        //setto l'intorno del centro dato come oche
        for (int roundR = r - 2; roundR <= r + 2; roundR++) {
            if (!IsPositionOutOfCross(roundR, c)) {
                //Debug.Log("Sono triste: " + roundR + c + !IsPositionOutOfCross(roundR, c));
                oneBoard[roundR, c] = PawnType.Goose;
            }
        }
        for(int roundC = c - 2; roundC <= c + 2; roundC++) {
            if (!IsPositionOutOfCross(r, roundC)) {
                //Debug.Log("Sono triste: " + r + roundC + !IsPositionOutOfCross(r, roundC));
                oneBoard[r, roundC] = PawnType.Goose;
            }
        }
        // setto il centro come volpe
        oneBoard[r, c] = PawnType.Fox;
        return oneBoard;
    }

    // Metodo ausiliario =======================================================
    // ritorna se una corrdinata è fuori dalla croce di gioco
    public static bool IsPositionOutOfCross(int row, int column) {
        if (row<0 || row>6 || column<0 || column>6) return true;
        if (row < 2 && column < 2
            || row < 2 && column > 4
            || row > 4 && column < 2
            || row > 4 && column > 4) {
            return true;
        } else {
            return false;
        }
    }
}