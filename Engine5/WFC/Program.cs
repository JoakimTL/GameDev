using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WFC
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Input sidelength:");
            var sizeInput = Console.ReadLine();

            if (!int.TryParse(sizeInput, out int size))
                return;


            //var map = new Map(size);

        }
    }

    internal class Tile
    {
        private static int _currentId = 1;
        public int Id { get; }
        public Color Color { get; }

        public Tile(Color color)
        {
			this.Id = _currentId++;
			this.Color = color;
        }
    }

    internal class Map
    {
        private int _size;
        private Tile[] _tiles;
        private Dictionary<int, double>[] _connections;

        private int[] _map;

        public Map(int size, IEnumerable<Tile> tiles)
        {
            this._size = size;
			this._tiles = tiles.ToArray();
			this._connections = new Dictionary<int, double>[ this._tiles.Length];
            for (int i = 0; i < this._tiles.Length; i++)
				this._connections[i] = new Dictionary<int, double>();

			this._map = new int[size * size];
        }

        public void AddTileConnection(Tile a, Tile b, double weight)
        {
			this._connections[a.Id][b.Id] = weight;
			this._connections[b.Id][a.Id] = weight;
        }

        private double GetConnection(int a, int b) => this._connections[a].TryGetValue(b, out double val) ? val : 0;

        public int GetIndex(int x, int y) => x >= 0 && x < this._size && y >= 0 && y < this._size ? x + y * this._size : -1;
        public (int x, int y) GetCoordinates(int index)
        {
            int y = Math.DivRem(index, this._size, out int x);
            return (x, y);
        }

        public void Seed(int x, int y, Tile t) => this._map[GetIndex(x, y)] = t.Id;

        public void Generate(Random rand)
        {
            if ( this._tiles.Length == 0)
                return;

            int[] neighbourIds = new int[4];
            double[] choices = new double[4];
            HashSet<int> seeded = new HashSet<int>();
            Queue<int> unseeded = new Queue<int>();

            void EnqueueUnseeded(int x, int y)
            {
                int ni;
                ni = GetIndex(x - 1, y);
                if (ni != -1 && !seeded.Contains(ni))
                    unseeded.Enqueue(ni);
                ni = GetIndex(x, y - 1);
                if (ni != -1 && !seeded.Contains(ni))
                    unseeded.Enqueue(ni);
                ni = GetIndex(x + 1, y);
                if (ni != -1 && !seeded.Contains(ni))
                    unseeded.Enqueue(ni);
                ni = GetIndex(x, y + 1);
                if (ni != -1 && !seeded.Contains(ni))
                    unseeded.Enqueue(ni);
            }

            int FindAppropriateTile(int a, int x, int y)
            {
                for (int i = 0; i < choices.Length; i++)
                    choices[i] = 0;
                int ni;
                ni = GetIndex(x - 1, y);
                if (ni != -1)
                    choices[0] = GetConnection(a, this._map[ni]);
                ni = GetIndex(x, y - 1);
                if (ni != -1)
                    choices[1] = GetConnection(a, this._map[ni]);
                ni = GetIndex(x + 1, y);
                if (ni != -1)
                    choices[2] = GetConnection(a, this._map[ni]);
                ni = GetIndex(x, y + 1);
                if (ni != -1)
                    choices[3] = GetConnection(a, this._map[ni]);

                double choiceSum = 0;
                for (int i = 0; i < choices.Length; i++)
                    choiceSum += choices[i];
                for (int i = 0; i < choices.Length; i++)
                    choices[i] /= choiceSum;
                double lastChoice = 0;
                for (int i = 0; i < choices.Length; i++)
                {
                    choices[i] += lastChoice;
                    lastChoice = choices[i];
                }
                double choice = rand.NextDouble();
                int chosenTile = 0;
                for (int i = 0; i < choices.Length; i++)
                {
                    if (choice < choices[i])
                        chosenTile = i;
                }
                return 0;
            }

            for (int i = 0; i < this._map.Length; i++)
            {
                if ( this._map[i] != 0)
                    seeded.Add(i);
            }

            if (seeded.Count == 0)
            {
                int index = rand.Next(0, this._map.Length);
				this._map[index] = rand.Next(1, this._tiles.Length + 1);
                seeded.Add(index);
            }

            foreach (int i in seeded)
            {
                var p = GetCoordinates(i);
                EnqueueUnseeded(p.x, p.y);
            }

            while (unseeded.Count > 0)
            {
                var i = unseeded.Dequeue();
                var p = GetCoordinates(i);
				this._map[i] = FindAppropriateTile( this._map[i], p.x, p.y);
                EnqueueUnseeded(p.x, p.y);
            }
        }
    }
}
