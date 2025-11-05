using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_csharp
{
    public class GameStateImmortal:GameStateClassic
    {
        public GameStateImmortal(int rows, int cols) : base(rows, cols) { }

        public override void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            int newRow = (newHeadPos.Row + Rows) % Rows;
            int newCol = (newHeadPos.Col + Cols) % Cols;
            newHeadPos = new Position(newRow, newCol);
            GridValue hit = WillHit(newHeadPos);
            if (hit == GridValue.Food)
            {
                Score++;
                AddHead(newHeadPos);
                AddFood();
            }
            else
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            if (snakePositions.Count == Rows * Cols)
            {
                GameOver = true;
            }
        }
    }
}
