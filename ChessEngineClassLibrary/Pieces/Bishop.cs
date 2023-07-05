using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Bishop;
        }
    }
}
