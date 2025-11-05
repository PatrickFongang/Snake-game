

using System.CodeDom;

namespace Snake_csharp
{
    public class Position
    {
        public int Row { get; }
        public int Col { get; }
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public Position Translate(Direction direction)
        {
            return new Position(Row + direction.RowOffset, Col + direction.ColOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public static Direction operator -(Position a, Position b)
        {
            return new Direction(a.Row - b.Row, a.Col - b.Col);
        }
    }
}
