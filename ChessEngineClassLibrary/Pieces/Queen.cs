using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClassLibrary.Pieces
{
    public class Queen : Piece
    {
        public Queen(PColor pColor, string pName) : base(pColor, pName)
        {
            PieceType = PType.Queen;
        }
    }
}
