using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Snake_csharp
{
    public class GameStateTwoSnakes : GameStateClassic
    {
        private LinkedList<Position> secondSnakePositions = new LinkedList<Position>();
        public GameStateTwoSnakes(int rows, int cols) : base(rows, cols) { }
        protected override void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                Grid[r, Cols - c - 1] = GridValue.SecondSnake;
                snakePositions.AddFirst(new Position(r, c));
                secondSnakePositions.AddFirst(new Position(r, Cols - c - 1));
            }
        }

        public override Position SecondHeadPosition() => secondSnakePositions.First.Value;

        public Position SecondTailPosition() => secondSnakePositions.Last.Value;

        public override IEnumerable<Position> SecondSnakePositions() => secondSnakePositions;
        protected void AddSecondHead(Position pos)
        {
            secondSnakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.SecondSnake;
        }
        protected void RemoveSecondTail()
        {
            Position tail = secondSnakePositions.Last.Value;
            if (secondSnakePositions.Count(x => x == tail) == 1)
            {
                Grid[tail.Row, tail.Col] = GridValue.Empty;
            }
            secondSnakePositions.RemoveLast();
        }
        public override void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            Position newSecondHeadPos = SecondHeadPosition().Translate(Dir.Opposite());
            (GridValue First, GridValue Second) hits = (WillHit(newHeadPos), WillHit(newSecondHeadPos));
            if (snakePositions.Count == Rows * Cols)
            {
                GameOver = true;
            }

            if (GameOverIfCollision(hits.First,hits.Second))
            {
                GameOver = true;
            }
            else if (hits.First == GridValue.Food || hits.Second == GridValue.Food)
            {
                Score++;
                AddHead(newHeadPos);
                AddSecondHead(newSecondHeadPos);
                AddFood();
                
            }
            else
            {
                RemoveTail();
                AddHead(newHeadPos);
                RemoveSecondTail();
                AddSecondHead(newSecondHeadPos);
            }
        }
        private bool GameOverIfCollision(GridValue first, GridValue second)
        {
            return first == GridValue.Outside || first == GridValue.Snake ||
                   second == GridValue.Outside || second == GridValue.SecondSnake
                   || first == GridValue.SecondSnake || second == GridValue.Snake;
        }

    }
}
