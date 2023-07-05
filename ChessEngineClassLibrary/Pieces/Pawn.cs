﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Pieces
{
    public class Pawn : Piece
    {

        public Pawn(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Pawn;
        }



    }
}
