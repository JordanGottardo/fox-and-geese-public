using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxAndGeese;
using System;

public class Move {

    public PawnType pawnType;
    public int startingX;
    public int startingZ;
    public int finalX;
    public int finalZ;

    public Move() {

    }

    //costruttore
    public Move(PawnType pawnType, int startingX, int startingZ, int finalX, int finalZ) {
        this.pawnType = pawnType;
        this.startingX = startingX;
        this.startingZ = startingZ;
        this.finalX = finalX;
        this.finalZ = finalZ;
    }

    public override string ToString() {
        return "pawnType = " + pawnType + "x = " + startingX + "z = " + startingZ + "finalX = " + finalX + "finalZ = " + finalZ + " intPawnType = ";
    }

    // ritorna se due mosse sono uguali
    public static bool operator ==(Move a, Move b) {
        return a.finalX == b.finalX
            && a.finalZ == b.finalZ
            && a.pawnType == b.pawnType
            && a.startingX == b.startingX
            && a.startingZ == b.startingZ;
    }

    // ritorna se due mosse sono diverse
    public static bool operator !=(Move a, Move b) { return !(a == b); }

    // interpola la cella centrale, in mezzo, date due celle a distanza 2
    public Vector2 InterpolateEatenPawn() {
        int sx = startingX;
        int sz = startingZ;
        int ex = finalX;
        int ez = finalZ;
        // per interpolare solo quando le posizioni sono allineate
        if ((  (Math.Abs(sx - ex) == 2) && (Math.Abs(sz - ez) == 2) && (((sx + sz) % 2) == 0))
            || Math.Abs(sx - ex) == 2 && Math.Abs(sz - ez) == 0
            || Math.Abs(sx - ex) == 0 && Math.Abs(sz - ez) == 2
            || Math.Abs(sx - ex) == 0 && Math.Abs(sz - ez) == 0
            ) {
            return new Vector2((sx + ex) / 2, (sz + ez) / 2);
        }
        return new Vector2(-1, -1);
    }
}