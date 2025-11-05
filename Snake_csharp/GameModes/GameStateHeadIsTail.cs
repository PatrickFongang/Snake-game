using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Snake_csharp
{
    public class GameStateHeadIsTail: GameStateClassic 
    {
        public GameStateHeadIsTail(int row,int col): base(row, col) { }

        public override void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if (snakePositions.Count == Rows * Cols)
            {
                GameOver = true;
            }

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
                Score++;
                Dir = NewHeadDirection(snakePositions);
                AddHead(newHeadPos);
                snakePositions = new LinkedList<Position>(snakePositions.Reverse());
                AddFood();
            }
        }
        private Direction NewHeadDirection(IEnumerable<Position> snake)
        {
            return snake.Last()-snake.ElementAt(snake.Count() - 2);
        }
    }
}
