using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

namespace AdventOfCode2020.Day22
{
    internal class AOC_day22_tests
    {
        [Test]
        public void TestSample()
        {
            string[] data = @"
Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10".Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            Combat game = new Combat();
            game.DealCards(data);
            Assert.That(game.Part1Answer(), Is.EqualTo(306));

            Combat game2 = new Combat();
            game2.DealCards(data);
            Assert.That(game2.Part2Answer(), Is.EqualTo(291));
        }

        [Test]
        public void TestPart1Answer()
        {
            string[] data = File.ReadAllLines("Day22Data.txt");
            Combat game = new Combat();
            game.DealCards(data);
            Console.WriteLine(game.Part1Answer());
        }

        [Test]
        public void TestPart2Answer()
        {
            string[] data = File.ReadAllLines("Day22Data.txt");
            Combat game = new Combat();
            game.DealCards(data);
            Console.WriteLine(game.Part2Answer());
        }
    }

    abstract class GameRule
    {
        internal readonly int PlayerOne = 0;
        internal readonly int PlayerTwo = 1;

        protected Deck[] decks = new Deck[2];

        protected bool IsGameOver()
        {
            return (decks[PlayerOne].IsEmpty() || decks[PlayerTwo].IsEmpty());
        }
        protected void CloneDecks(IEnumerable<Deck> players, int nP1Cards, int nP2Cards)
        {
            decks[PlayerOne] = new Deck("Player 1");
            decks[PlayerTwo] = new Deck("Player 2");
            this.decks[PlayerOne].LoadDeck(players.First().ExtractCardValues(nP1Cards));
            this.decks[PlayerTwo].LoadDeck(players.Last().ExtractCardValues(nP2Cards));
        }
        public abstract Deck Play(IEnumerable<Deck> players, int nPlayer1Deck = 0, int nPlayer2Deck = 0);
    }

    internal class Part1Rule : GameRule
    {
        private void PlayRound()
        {
            var player1Hand = decks[PlayerOne].DequeueCard();
            var player2Hand = decks[PlayerTwo].DequeueCard();
            if (player1Hand.FaceValue > player2Hand.FaceValue)
            {
                decks[PlayerOne].EnqueueCard(player1Hand);
                decks[PlayerOne].EnqueueCard(player2Hand);
            }
            else
            {
                decks[PlayerTwo].EnqueueCard(player2Hand);
                decks[PlayerTwo].EnqueueCard(player1Hand);
            }
        }
        public override Deck Play(IEnumerable<Deck> players, int nPlayer1Deck = 0, int nPlayer2Deck = 0)
        {
            CloneDecks(players, nPlayer1Deck, nPlayer2Deck);
            while (!IsGameOver())
            {
                PlayRound();
            }
            return (decks[PlayerOne].IsEmpty()) ? decks[PlayerTwo] : decks[PlayerOne];

        }
    }

    internal class DeckData
    {
        private readonly int[] faceValues;

        public DeckData(int[] data)
        {
            faceValues = data.ToArray();

        }

        public bool IsMatch(int[] data)
        {
            if (faceValues.Length != data.Length) return false;
            for (int i = 0; i < faceValues.Length; i++)
            {
                if (faceValues[i] != data[i]) return false;
            }
            return true;
        }
    }

    internal class DeckRecorder
    {
        private Dictionary<int, List<DeckData>> records = new Dictionary<int, List<DeckData>>();

        public bool Record(int firstCardValue, int[] restValues)
        {
            if(records.ContainsKey(firstCardValue))
            {
                foreach(var record in records[firstCardValue])
                {
                    if (record.IsMatch(restValues))
                    {
                        return false;
                    }
                }
                records[firstCardValue].Add(new DeckData(restValues));
                return true;
            }
            else
            {
                records.Add(firstCardValue, new List<DeckData>());
                records[firstCardValue].Add(new DeckData(restValues));
                return true;
            }
        }
    }

    internal class Part2Rule : GameRule
    {
        DeckRecorder player1History = new DeckRecorder();
        DeckRecorder player2History = new DeckRecorder();


        private StringBuilder history = new StringBuilder();

        private bool DidPlayerOneWin(SpaceCard player1Hand, SpaceCard player2Hand)
        {
            // Rule 2: winner of sub-game
            if (decks[PlayerOne].Count >= player1Hand.FaceValue && 
                decks[PlayerTwo].Count >= player2Hand.FaceValue)
            {
                var subGame = new Part2Rule();
                var result = subGame.Play(decks, player1Hand.FaceValue, player2Hand.FaceValue);
                if (result.Name.Contains("1")) return true;
                else return false;
            }
            // Rule 3: higher face value wins.
            else
            {
                return player1Hand.FaceValue >= player2Hand.FaceValue;
            }
        }
        private void PlayRound()
        {
            var player1Hand = decks[PlayerOne].DequeueCard();
            var player2Hand = decks[PlayerTwo].DequeueCard();

            // Rule 1: repeat sequence

            var p1Record = player1History.Record(player1Hand.FaceValue, decks[PlayerOne].ExtractCardValues(0));
            var p2Record = player2History.Record(player2Hand.FaceValue, decks[PlayerTwo].ExtractCardValues(0));


            if(!(p1Record && p2Record))
            {
                decks[PlayerOne].EnqueueCard(player1Hand);
                decks[PlayerOne].EnqueueCard(player2Hand);
                decks[PlayerTwo].Clear();
                return;
            }

            if (DidPlayerOneWin(player1Hand, player2Hand))
            {
                decks[PlayerOne].EnqueueCard(player1Hand);
                decks[PlayerOne].EnqueueCard(player2Hand);
            }
            else
            {
                decks[PlayerTwo].EnqueueCard(player2Hand);
                decks[PlayerTwo].EnqueueCard(player1Hand);
            }
        }
        public override Deck Play(IEnumerable<Deck> players, int nPlayer1Deck = 0, int nPlayer2Deck = 0)
        {
            CloneDecks(players, nPlayer1Deck, nPlayer2Deck);
            while (!IsGameOver())
            {
                PlayRound();
            }
            return (decks[PlayerOne].IsEmpty()) ? decks[PlayerTwo] : decks[PlayerOne];

        }
    }


    internal class Combat
    {
        internal readonly int PlayerOne = 0;
        internal readonly int PlayerTwo = 1;

        private List<Deck> decks = new List<Deck>();

        public void DealCards(string[] cardData)
        {
            Deck currentDeck = null;
            foreach(var item in cardData)
            {
                if(!String.IsNullOrEmpty(item))
                {
                    if (item.Contains("Player"))
                    {
                        currentDeck = new Deck(item.Trim(':'));
                        decks.Add(currentDeck);
                    }
                    else
                    {
                        if (currentDeck == null) throw new Exception("Incompatible Card Data");
                        currentDeck.EnqueueCard(new SpaceCard(Convert.ToInt32(item)));
                    }
                }
            }
        }

        public Deck Play(GameRule playRule)
        {
            return playRule.Play(decks.ToArray());
        }

        public long Part1Answer()
        {
            var winner = Play(new Part1Rule());
            long score = 0;
            while(!winner.IsEmpty())
            {
                var card = winner.DequeueCard();
                score += card.FaceValue * (winner.Count + 1);
            }
            return score;
        }

        public long Part2Answer()
        {
            var winner = Play(new Part2Rule());
            long score = 0;
            while (!winner.IsEmpty())
            {
                var card = winner.DequeueCard();
                score += card.FaceValue * (winner.Count + 1);
            }
            return score;
        }
    }

    internal class SpaceCard
    {
        private readonly int value;

        public int FaceValue { get { return value; } }

        public SpaceCard(int faceValue)
        {
            value = faceValue;
        }
    }

    internal class Deck
    {
        public string Name { get; init; }

        public int Count { get { return cards.Count; } }
        
        private Queue<SpaceCard> cards = new Queue<SpaceCard>();

        public Deck(string name)
        {
            Name = name;
        }

        public void LoadDeck(int[] deck)
        {
            cards.Clear();
            foreach (var card in deck)
            {
                cards.Enqueue(new SpaceCard(card));
            }
        }

        public void EnqueueCard(SpaceCard card)
        {
            cards.Enqueue(card);
        }

        public SpaceCard DequeueCard()
        {
            return cards.Dequeue();
        }

        public SpaceCard PeekCard()
        {
            return cards.Peek();
        }

        public int[] ExtractCardValues(int nElements = 0)
        {
            var temp = new List<int>();
            var cards = this.cards.ToArray();
            if(nElements == 0) nElements = cards.Length;

            for(int i = 0; i < nElements; i++)
            {
                temp.Add(cards[i].FaceValue);
            }
            return temp.ToArray();
        }

        public void Clear()
        {
            cards.Clear();
        }

        public bool IsEmpty()
        {
            if (cards.Count == 0) return true;
            return false;
        }
    }
}
