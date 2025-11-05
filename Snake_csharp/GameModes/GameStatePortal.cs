using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Snake_csharp
{
    public class GameStatePortal: GameStateClassic
    {
        private Position Portal1;
        private Position Portal2;
        public GameStatePortal(int rows, int cols) : base(rows, cols) 
        {
            AddPortal();
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
            else if (hit == GridValue.Portal)
            {
                newHeadPos = (newHeadPos == Portal1) ? Portal2 : Portal1;
                AddHead(newHeadPos);
            }
            CheckPortal();
        }
        private void CheckPortal()
        {
            Position tail = TailPosition();
            if (tail == Portal1 || tail == Portal2)
            {
                Grid[Portal1.Row, Portal1.Col] = GridValue.Empty;
                Grid[Portal2.Row, Portal2.Col] = GridValue.Empty;
                AddPortal();
            }
        }
        private void AddPortal()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if (empty.Count() <= 2)
                return;
            Portal1 = empty[random.Next(empty.Count)];
            Portal2 = empty
                .Where(pos=>pos!=Portal1)
                .ToArray()[random.Next(empty.Count)];
            Grid[Portal1.Row, Portal1.Col] = GridValue.Portal;
            Grid[Portal2.Row, Portal2.Col] = GridValue.Portal;
        }

    }
}
