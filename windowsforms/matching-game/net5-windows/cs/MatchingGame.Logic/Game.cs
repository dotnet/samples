using System;
using System.Collections.Generic;

namespace MatchingGame.Logic
{
    public sealed class Game
    {
        private readonly char[] _cards;
        private readonly bool[] _openCards;
        private readonly List<int> _turnCards = new List<int>(2);
        private int _turns;

        private Game(int width, int height, char[] board)
        {
            Width = width;
            Height = height;
            _cards = board;
            _openCards = new bool[_cards.Length];
        }

        public static Game Create()
        {
            var numberOfPairs = 8;
            var width = numberOfPairs / 2;
            var height = width;

            var board = new char[width * height];
            var availableCards = new List<char>()
            {
                '!', '!', 'N', 'N', ',', ',', 'k', 'k',
                'b', 'b', 'v', 'v', 'w', 'w', 'z', 'z'
            };

            var random = new Random();

            for (var i = 0; i < numberOfPairs * 2; i++)
            {
                var cardIndex = random.Next(availableCards.Count - 1);
                var card = availableCards[cardIndex];
                board[i] = card;
                availableCards.RemoveAt(cardIndex);
            }

            return new Game(width, height, board);
        }

        public int Turns => _turns;

        public int Width { get; }

        public int Height { get; }

        public int RemainingCardsInTurn => 2 - _turnCards.Count;

        private int GetIndex(int w, int h)
        {
            return h * Width + w;
        }

        public int OpenCard(int w, int h)
        {
            if (_turnCards.Count > 1)
                throw new InvalidOperationException("Cannot open more than two cards");

            var cardIndex = GetIndex(w, h);
            var cardValue = _cards[cardIndex];
            _turnCards.Add(cardIndex);
            _openCards[cardIndex] = true;

            if (_turnCards.Count == 2)
                _turns++;

            return cardValue;
        }

        public void CloseCards()
        {
            foreach (var cardIndex in _turnCards)
                _openCards[cardIndex] = false;
        }

        public char GetCard(int w, int h)
        {
            var cardIndex = GetIndex(w, h);
            return _cards[cardIndex];
        }

        public bool IsOpen(int w, int h)
        {
            var v = GetIndex(w, h);
            return _openCards[v];
        }

        public bool CompleteTurn()
        {
            if (_turnCards.Count != 2)
                return false;

            var firstCardIndex = _turnCards[0];
            var secondCardIndex = _turnCards[1];
            var isMatch = _cards[firstCardIndex] == _cards[secondCardIndex];
            if (!isMatch)
                CloseCards();

            _turnCards.Clear();
            return isMatch;
        }

        public bool IsComplete()
        {
            foreach (var isVisible in _openCards)
                if (!isVisible)
                    return false;

            return true;
        }
    }
}