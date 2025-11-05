using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Snake_csharp
{
    public class GameStateClassic
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; protected set; }
        public int Score { get; protected set; }
        public bool GameOver { get; protected set; }

        protected readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        protected LinkedList<Position> snakePositions = new LinkedList<Position>();
        protected readonly Random random = new Random();
        public GameStateClassic(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            AddSnake();
            AddFood();
            Dir = Direction.Right;
        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }
        protected IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        protected void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
                return;

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Position HeadPosition() => snakePositions.First.Value;

        public Position TailPosition() => snakePositions.Last.Value;

        public IEnumerable<Position> SnakePositions() => snakePositions;

        protected void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }
        protected void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            if (snakePositions.Count(x=>x==tail)==1)
            {
                Grid[tail.Row, tail.Col] = GridValue.Empty;
            }
            snakePositions.RemoveLast();
        }
        
        private Direction GetDirection()
        {
            if (dirChanges.Count==0)
            {
                return Dir;
            }

            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }
            Direction lastDir = GetDirection();
            return newDir != lastDir.Opposite() && lastDir!=newDir;
        }
        public void ChangeDirection(Direction dir)
        {
            if(CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }
        public GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
                return GridValue.Outside;

            if(newHeadPos == TailPosition())
                return GridValue.Empty;

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }
        public virtual void Move()
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
        }
    }
}
