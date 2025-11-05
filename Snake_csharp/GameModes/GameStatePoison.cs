using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_csharp
{
    public class GameStatePoison: GameStateClassic
    {
        public GameStatePoison(int rows, int cols) : base(rows, cols)
        {
            AddPoison();
        }

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
                AddHead(newHeadPos);
                AddFood();
            }
            else if (hit == GridValue.Poison)
            {
                Score--;
                
                RemoveTail();
                AddHead(newHeadPos);
                if (snakePositions.Count > 0)
                    RemoveTail();
                CheckGameOver();
                AddPoison();
            }
        }
        private void CheckGameOver()
        {
            if (snakePositions.Count == 1)
            {
                GameOver = true;
            }
        }
        private void AddPoison()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if (empty.Count == 0)
            {
                return;
            }
            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Poison;
        }
    }
}
