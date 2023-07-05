using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Pieces
{
    public class King : Piece
    {
        public King(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.King;
        }
    }
}
