using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackCasino
{
    public class Program
    {
        public static List<Card> Deck { get; set; }

        public static List<Player> Players { get; set; }

        public static List<Card> TableCards { get; set; }

        public static List<List<int>> GenFinisherResults { get; set; }

        private static void Main(string[] args)
        {
            GenerateCardsForDeck();
            ShuffleCards();
            CardCut();

            Console.WriteLine("Welcome to Black Casino!");
            Console.WriteLine("The Deck is 40 cards. No jacks, queens, kings or Jokers");
            Console.WriteLine("You will have 10 cards at any time, unless if its 3 players then you will have 13.");
            Console.WriteLine("Default : player vs Dealer");
            Console.WriteLine("How many players (max : 4; d = default) ?");

            var playerNum = Console.ReadLine();
            Players = new List<Player>();

            if (string.IsNullOrEmpty(playerNum) || playerNum == "d")
            {
                Players.Add(new Player
                {
                    Name = "Player 1!",
                    UserBuild = new Build { Cards = new List<Card>(), IsAvailableForSteal = true },
                    UserHand = new List<Card>(),
                    UserPile = new List<Card>()
                });
                Players.Add(new Player
                {
                    Name = "Dealer",
                    UserBuild = new Build { Cards = new List<Card>(), IsAvailableForSteal = true },
                    UserHand = new List<Card>(),
                    UserPile = new List<Card>()
                });
            }
            else
            {
                var isInt = Int32.TryParse(playerNum, out int playerNumm);
                if (playerNumm > 4)
                {
                    playerNumm = 4;
                }

                for (int i = 0; i < playerNumm; i++)
                {
                    Console.WriteLine($"Player {i + 1} name?");
                    var playerName = Console.ReadLine();
                    Players.Add(new Player
                    {
                        Name = string.IsNullOrEmpty(playerName) ? $"Player {i + 1}" : playerName,
                        UserBuild = new Build { Cards = new List<Card>() },
                        UserHand = new List<Card>(),
                        UserPile = new List<Card>()
                    });
                }
            }

            Console.WriteLine("\n");
            Console.WriteLine("Start");
            Console.WriteLine("Shuffle");
            Console.WriteLine("Cut");
            Console.WriteLine("\n");
            Console.WriteLine("Deal");

            TableCards = new List<Card>();

            Deal();
            ShowPlayer1Cards();
            ShowDealerCards();
            //Console.WriteLine("You : {cards} 8♠ 8♥ 8♣ 4♦ A♠ 6S 7C 3H 4H 2D {10}");
            //Console.WriteLine("Opponent : {cards} X X X X X X X X X X {10}");
            Console.WriteLine("\n");
            ShowTableCards();
            Console.WriteLine("\n");
            ShowAllPlayersCardsPile();
            Console.WriteLine("\n");
            ShowAllPlayersCardsBuild();
            Console.WriteLine("\n");
            Console.WriteLine($"Deck {Deck.Count}");

            Turns();

            Console.WriteLine("End of Game.");
            Console.WriteLine("Dealer says Good game.");

            Console.ReadLine();
        }

        private static void ShowWholeGame()
        {
            Console.WriteLine("Player Hands");
            ShowPlayer1Cards();
            ShowDealerCards();
            //Console.WriteLine("You : {cards} 8♠ 8♥ 8♣ 4♦ A♠ 6S 7C 3H 4H 2D {10}");
            //Console.WriteLine("Opponent : {cards} X X X X X X X X X X {10}");
            Console.WriteLine("\n");
            Console.WriteLine("Cards on Table");
            ShowTableCards();
            Console.WriteLine("\n");
            Console.WriteLine("Players Piles");
            ShowAllPlayersCardsPile();
            Console.WriteLine("\n");
            Console.WriteLine("Player Builds");
            ShowAllPlayersCardsBuild();
            Console.WriteLine("\n");
            Console.WriteLine($"Deck {Deck.Count}");
        }

        public static void GenerateCardsForDeck()
        {
            Deck = new List<Card>();

            var s = new Suits();
            foreach (var suit in (SuitType[])Enum.GetValues(typeof(SuitType)))
            {
                for (int i = 1; i <= 10; i++)
                {
                    var suitValue = string.Empty;
                    var pointVal = 0;
                    var isPointCard = false;

                    if (SuitType.Clubs == suit)
                    {
                        suitValue = s.Clubs;
                    }

                    if (SuitType.Spades == suit)
                    {
                        suitValue = s.Spades;
                    }

                    if (SuitType.Hearts == suit)
                    {
                        suitValue = s.Hearts;
                    }

                    if (SuitType.Diamonds == suit)
                    {
                        suitValue = s.Diamonds;
                    }

                    var genVal = string.Empty;

                    if (i == 1)
                    {
                        genVal = "A" + suitValue;
                        pointVal = 1;
                        isPointCard = true;
                    }
                    else
                    {
                        genVal = $"{i}{suitValue}";
                    }

                    if (SuitType.Diamonds == suit && i == 10)
                    {
                        pointVal = 2;
                        isPointCard = true;
                    }

                    if (SuitType.Spades == suit && i == 2)
                    {
                        pointVal = 1;
                        isPointCard = true;
                    }

                    Deck.Add(new Card
                    {
                        Number = i,
                        SuitType = suit,
                        SuitValue = suitValue,
                        Value = genVal,
                        IsPointCard = isPointCard,
                        PointValue = pointVal
                    });
                }
            }
        }

        public static void ShuffleCards()
        {
            if (Deck != null && Deck.Any())
            {
                var temp = new List<Card>();
                var random = new Random();

                for (int i = 0; i < 40; i++)
                {
                    var length = Deck.Count;
                    var n = random.Next(0, length - 1);
                    temp.Add(Deck[n]);
                    Deck.RemoveAt(n);
                }

                Deck = new List<Card>(temp);
            }
        }

        public static void CardCut()
        {
            if (Deck != null && Deck.Any())
                Deck.Reverse();
        }

        public static void Deal()
        {
            var cardCount = 10;
            if (Players.Count == 3)
            {
                cardCount = 13;
                TableCards.Add(Deck[0]);
                Deck.RemoveAt(0);
            }

            foreach (var player in Players)
            {
                for (int i = 0; i < cardCount; i++)
                {
                    player.UserHand.Add(Deck[0]);
                    Deck.RemoveAt(0);
                }
            }
        }

        private static void ShowAllPlayersCards()
        {
            foreach (var player in Players)
            {
                ShowPlayerCards(player);
            }
        }

        private static void ShowAllPlayersCardsPile()
        {
            foreach (var player in Players)
            {
                ShowPlayerCardsPile(player);
            }
        }

        private static void ShowAllPlayersCardsBuild()
        {
            foreach (var player in Players)
            {
                ShowPlayerCardsBuild(player);
            }
        }

        private static void ShowPlayerCards(Player player)
        {
            Console.WriteLine($"{player.Name} : {string.Join(' ', player.UserHand.Select(c => c.Value))} ({player.UserHand.Count})");
        }

        private static void ShowPlayerCardsPile(Player player)
        {
            Console.WriteLine($"{player.Name} : {string.Join(' ', player.UserPile.Select(c => c.Value))} ({player.UserPile.Count})");
        }

        private static void ShowPlayerCardsBuild(Player player)
        {
            Console.WriteLine($"{player.Name} : {string.Join(' ', player.UserBuild.Cards.Select(c => c.Value))} ({player.UserBuild.Cards.Count})");
        }

        private static void ShowTableCards()
        {
            Console.WriteLine($"Table Cards : {string.Join(' ', TableCards.Select(c => c.Value))} ({TableCards.Count})");
        }

        private static void ShowPlayer1Cards()
        {
            ShowPlayerCards(Players.Where(c => c.Name.Contains("Player")).FirstOrDefault());
        }

        private static void ShowDealerCards()
        {
            var player = Players.Where(c => c.Name.Contains("Dealer")).FirstOrDefault();
            ShowPlayerCards(player);

            var cards = string.Empty;
            for (int i = 0; i < player.UserHand.Count; i++)
            {
                cards += "X ";
            }
            Console.WriteLine($"{player.Name} : {cards} ({player.UserHand.Count})");
        }

        private static void Turns()
        {
            while (Deck.Count > 0 || GetAllUserHandCardCount() > 0)
            {
                Console.WriteLine("Player 1 turn");
                Console.WriteLine("Options : \n1. Throw card \n2. Build \n3. Capture");
                var s = Console.ReadLine();
                Int32.TryParse(s, out int ans);

                if (ans == 1)
                {
                    Console.WriteLine("Choose card ");
                    var ss = Console.ReadLine();
                    ss = ss.ToUpperInvariant();

                    var card = Players[0].UserHand.Where(c => c.Value == ss).FirstOrDefault();
                   
                    if (card != null)
                    {
                        TableCards.Add(card);

                        Players[0].UserHand.Remove(card);
                    }
                }
                else if (ans == 2)
                {
                    if (Players[0].UserBuild.Number == 0)
                    {
                        Console.WriteLine("What number do you want to build? ");
                        var sds = Console.ReadLine();
                        Int32.TryParse(sds, out int num);

                        if (!Players.Select(c => c.UserBuild.Number).Contains(num))
                        {
                            Players[0].UserBuild.Number = num;
                            Players[0].UserBuild.IsAvailableForSteal = true;
                        }
                        else
                        {
                            Console.WriteLine("That number is already taken. ");
                        }
                    }

                    Console.WriteLine("Options \n1. Steal Opponents Build \n2. Get from Available pile(s) or table. ");
                    var ana = Console.ReadLine();
                    Int32.TryParse(ana, out int numm);
                    var option1Success = true;
                    if (numm == 1)
                    {
                        option1Success = false;
                        var playerBuilds = Players.Where(c => c.UserBuild.IsAvailableForSteal && c.UserBuild.Cards.Count > 0).Select(c => c.UserBuild);
                        if (playerBuilds.Any())
                        {
                            foreach (var plyBuild in playerBuilds)
                            {
                                if (plyBuild.Number < Players[0].UserBuild.Number)
                                {
                                    var dif = Players[0].UserBuild.Number - plyBuild.Number;

                                    var cadsAval = Players[0].UserHand.Where(c => c.Number == dif);
                                    if (cadsAval.Any())
                                    {
                                        Console.WriteLine($"Which card do want to add? {string.Join(' ', cadsAval.Select(c => c.Value))}");
                                        ana = Console.ReadLine();
                                        var crd = cadsAval.FirstOrDefault(c => c.Value == ana);
                                        plyBuild.Cards.Add(crd);
                                        Players[0].UserBuild.Cards.AddRange(plyBuild.Cards);
                                        plyBuild.Cards = new List<Card>();
                                        option1Success = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Sorry unable to steal");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sorry unable to steal");
                        }
                    }

                    if (numm == 2 || !option1Success)
                    {
                        var playerPilesAval = Players.Where(c => c.UserPile.Any() && !c.Name.Contains("Player")).Select(c => c.UserPile.FirstOrDefault());

                        var available = new List<Card>();

                        available.AddRange(playerPilesAval);
                        available.AddRange(TableCards);

                        Console.WriteLine($"Available cards : {string.Join(' ', available.Where(c => c.Number < Players[0].UserBuild.Number).Select(c => c.Value))}");
                        var potentialBuild = new Build { Number = Players[0].UserBuild.Number, Cards = new List<Card>() };
                        potentialBuild.Number = Players[0].UserBuild.Number;

                        Console.WriteLine($"Which of your own card are you adding?");
                        ana = Console.ReadLine();

                        var cardDown = Players[0].UserHand.FirstOrDefault(c => c.Value == ana);
                        potentialBuild.Cards.Add(cardDown);

                        foreach (var availableCard in available)
                        {
                            Console.WriteLine($"Which card do you want to add from available?{string.Join(' ', available.Where(c => c.Number < Players[0].UserBuild.Number).Select(c => c.Value))} (type done )");
                            ana = Console.ReadLine();

                            if (ana == "done")
                                break;

                            if (potentialBuild.Cards.Count > Players[0].UserBuild.Number)
                                break;

                            var selctCard = available.FirstOrDefault(c => c.Value == ana);
                            if ((potentialBuild.Cards.Select(c => c.Number).Sum() + selctCard.Number) > potentialBuild.Number)
                                continue;

                            potentialBuild.Cards.Add(selctCard);
                        }

                        if (potentialBuild.Number == potentialBuild.Cards.Select(c => c.Number).Sum())
                        {
                            foreach (var player in Players)
                            {
                                foreach (var plycrd in potentialBuild.Cards)
                                {
                                    player.UserPile.Remove(plycrd);
                                    TableCards.Remove(plycrd);
                                }
                            }

                            Players[0].UserBuild.Cards.AddRange(potentialBuild.Cards);

                            if (Players[0].UserBuild.Cards.Count > 3 ||
                                Players[0].UserBuild.Cards.Count != Players[0].UserBuild.Cards.Select(c => c.Number).Distinct().Count())
                            {
                                Players[0].UserBuild.IsAvailableForSteal = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"What were you doing?");
                        }

                        //Finisher option
                        var match = true;
                        while (match)
                        {
                            playerPilesAval = Players.Where(c => c.UserPile.Any() && !c.Name.Contains("Player")).Select(c => c.UserPile.FirstOrDefault());

                            available = new List<Card>();

                            available.AddRange(playerPilesAval);
                            available.AddRange(TableCards);

                            available = new List<Card>(available.Where(c => c.Number <= Players[0].UserBuild.Number));

                            potentialBuild = new Build { Number = Players[0].UserBuild.Number, Cards = new List<Card>() };

                            potentialBuild.Cards.AddRange(available.Where(c => c.Number == potentialBuild.Number));

                            GenFinisherResults = new List<List<int>>();

                            GenFinisher(available.Where(c => c.Number < potentialBuild.Number).Select(c => c.Number).ToList(), potentialBuild.Number);

                            var finalList = GenFinisherResults.Where(c => c.Contains(1) || c.Contains(10) || c.Contains(2)).FirstOrDefault();

                            if (finalList == null)
                            {
                                finalList = GenFinisherResults.FirstOrDefault();
                            }

                            if (finalList != null)
                            {
                                var cardsToAdd = new List<Card>();
                                foreach (var v in finalList)
                                {
                                    var crd = available.FirstOrDefault(c => c.Number == v);
                                    if (crd != null)
                                    {
                                        cardsToAdd.Add(crd);
                                        available.Remove(crd);
                                    }
                                }

                                potentialBuild.Cards.AddRange(cardsToAdd);

                                foreach (var player in Players)
                                {
                                    foreach (var plycrd in potentialBuild.Cards)
                                    {
                                        player.UserPile.Remove(plycrd);
                                        TableCards.Remove(plycrd);
                                    }
                                }

                                Players[0].UserBuild.Cards.AddRange(potentialBuild.Cards);
                            }
                            else
                            {
                                match = false;
                                break;
                            }
                        }

                        // After loop

                        //Int32.TryParse(ana, out int asnum);
                    }
                }
                else if (ans == 3)
                {
                    Console.WriteLine($"Capture your build?");
                    var yes = Console.ReadLine();
                    if (yes.Contains("y"))
                    {
                        var capCard = Players[0].UserHand.FirstOrDefault(c => c.Number == Players[0].UserBuild.Number);
                        Players[0].UserBuild.Cards.Add(capCard);
                        Players[0].UserPile.AddRange(Players[0].UserBuild.Cards);
                        Players[0].UserBuild = new Build { Number = 0, Cards = new List<Card>(), IsAvailableForSteal = true };
                        Players[0].UserHand.Remove(capCard);
                        Console.WriteLine($"Captured");
                    }
                    else
                    {
                        var playerPilesAval = Players.Where(c => c.UserPile.Any() && !c.Name.Contains("Player")).Select(c => c.UserPile.FirstOrDefault());

                        var available = new List<Card>();

                        available.AddRange(playerPilesAval);
                        available.AddRange(TableCards);

                        Console.WriteLine($"Available cards : {string.Join(' ', available.Where(c => c.Number < Players[0].UserBuild.Number).Select(c => c.Value))}");
                        var potentialBuild = new Build { Number = Players[0].UserBuild.Number, Cards = new List<Card>() };
                        potentialBuild.Number = Players[0].UserBuild.Number;

                        Console.WriteLine($"Which of your own card are you adding?");
                        var ana = Console.ReadLine();

                        var cardDown = Players[0].UserHand.FirstOrDefault(c => c.Value == ana);
                        potentialBuild.Cards.Add(cardDown);

                        foreach (var availableCard in available)
                        {
                            Console.WriteLine($"Which card do you want to add from available? (type capture )");
                            ana = Console.ReadLine();

                            if (ana == "done")
                                break;

                            if (potentialBuild.Cards.Count > Players[0].UserBuild.Number)
                                break;

                            var selctCard = available.FirstOrDefault(c => c.Value == ana);
                            if ((potentialBuild.Cards.Select(c => c.Number).Sum() + selctCard.Number) > potentialBuild.Number)
                                continue;

                            potentialBuild.Cards.Add(selctCard);
                        }

                        if (potentialBuild.Number == potentialBuild.Cards.Select(c => c.Number).Sum())
                        {
                            foreach (var player in Players)
                            {
                                foreach (var plycrd in potentialBuild.Cards)
                                {
                                    player.UserPile.Remove(plycrd);
                                    TableCards.Remove(plycrd);
                                }
                            }

                            Players[0].UserBuild.Cards.AddRange(potentialBuild.Cards);
                        }
                        else
                        {
                            Console.WriteLine($"What were you doing?");
                        }
                    }
                }
                else
                {
                    break;
                }
                Console.WriteLine("End Turn");
                Console.ReadLine();
                Console.WriteLine("\n");
                ShowWholeGame();
                Console.WriteLine("\n\n");
                Console.WriteLine("Dealer turn");
                Console.WriteLine(" Doing dealer things...");
                DealerCaptureOrBuild();
                Console.WriteLine("End Turn");
                Console.ReadLine();

                Console.WriteLine("\n");
                ShowWholeGame();
                Console.WriteLine("\n\n");

                if (!Players.SelectMany(c => c.UserHand).Any())
                {
                    if (Deck.Any())
                    {
                        Console.WriteLine("Dealer says nice first round \nLets go for another one.");
                        Console.WriteLine("\n\n");
                        Console.WriteLine("Doing dealer things...");
                        Deal();
                        Console.WriteLine("\n");
                        Console.WriteLine("Done.");
                    }
                }
            }
        }

        private static int GetAllUserHandCardCount()
        {
            return Players.SelectMany(c => c.UserHand).Count();
        }

        private static Card GetCard(string card)
        {
            var parsed = Int32.TryParse(card.ElementAt(0).ToString(), out int num);

            return new Card
            {
                Number = parsed ? num : 1,
                IsPointCard = "2S 10D AC AD AH AS".Contains(card),
                PointValue = "2S 10D AC AD AH AS".Contains(card) ? ("10D" == card ? 2 : 1) : 0,
                SuitValue = card.ElementAt(1).ToString(),
                Value = card
            };
        }

        public static void GenFinisher(List<int> numbers, int target)
        {
            SumUpRecursive(numbers, target, new List<int>());
        }

        // Before youy get here
        // No number must be greater than target, and
        // No number must be equal to target
        private static void SumUpRecursive(List<int> numbers, int target, List<int> partial)
        {
            int s = 0;
            foreach (int x in partial) s += x;

            if (s == target)
                GenFinisherResults.Add(partial);

            if (s >= target)
                return;

            for (int i = 0; i < numbers.Count; i++)
            {
                List<int> remaining = new List<int>();
                int n = numbers[i];
                for (int j = i + 1; j < numbers.Count; j++) remaining.Add(numbers[j]);

                List<int> partial_rec = new List<int>(partial);
                partial_rec.Add(n);
                SumUpRecursive(remaining, target, partial_rec);
            }
        }

        private static void DealerCaptureOrBuild()
        {
            Players[1].UserBuild.Number = Players[1].UserHand.Max(c => c.Number);
            var steal = DealerStealBuild();
            var built = false;
            if (!steal)
            {
                built = DealerBuild();
            }

            if (!built)
            {
                // Capture
                if (Players[1].UserHand.Count < 2)
                {
                    DealerCaptureBuild();
                }
                else
                {
                    if (!DealerLongCapture())
                    {
                        ThrowCard();
                    }
                }
            }
        }

        public static bool DealerStealBuild()
        {
            var potentialBuildAlmost = new Build { Number = Players[1].UserBuild.Number, Cards = new List<Card>() };
            var playerBuilds = Players.Where(c => c.UserBuild.IsAvailableForSteal && c.UserBuild.Cards.Count > 0).Select(c => c.UserBuild);
            if (playerBuilds.Any())
            {
                var breackAll = false;
                foreach (var plyBuild in playerBuilds)
                {
                    foreach (var dealerCard in Players[1].UserHand)
                    {
                        if (plyBuild.Number + dealerCard.Number == Players[1].UserBuild.Number)
                        {
                            plyBuild.Cards.Add(dealerCard);
                            Players[1].UserBuild.Cards.AddRange(plyBuild.Cards);
                            plyBuild.Cards = new List<Card>();
                            breackAll = true;
                            break;
                        }
                    }

                    if (breackAll)
                    {
                        FinishUp(Players[1].UserBuild.Cards, Players[1].UserBuild.Number);
                        break;
                    }
                }

                if (breackAll)
                    return true;
            }

            return false;
        }

        public static bool DealerBuild()
        {
            var potentialBuildAlmost = new Build { Number = Players[1].UserBuild.Number, Cards = new List<Card>() };

            var playerPilesAval = Players.Where(c => c.UserPile.Any() && !c.Name.Contains("Dealer")).Select(c => c.UserPile.FirstOrDefault());

            var available = new List<Card>();

            available.AddRange(playerPilesAval);
            available.AddRange(TableCards);
            var isLowerThanNum = Players[1].UserHand.Where(c => c.Number < Players[1].UserBuild.Number).Any();
            foreach (var dealerCard in Players[1].UserHand.Where(c => c.Number < Players[1].UserBuild.Number))
            {
                available = new List<Card>(available.Where(c => c.Number <= Players[1].UserBuild.Number));
                available.Add(dealerCard);

                var potentialBuild = new Build { Number = Players[1].UserBuild.Number, Cards = new List<Card>() };

                potentialBuild.Cards.AddRange(available.Where(c => c.Number == potentialBuild.Number));

                GenFinisherResults = new List<List<int>>();

                GenFinisher(available.Where(c => c.Number < potentialBuild.Number).Select(c => c.Number).ToList(), potentialBuild.Number);

                var finalList = GenFinisherResults.Where(c => c.Contains(1) || c.Contains(10) || c.Contains(2)).FirstOrDefault();

                if (finalList == null)
                {
                    finalList = GenFinisherResults.FirstOrDefault();
                }

                if (finalList != null)
                {
                    var cardsToAdd = new List<Card>();
                    foreach (var v in finalList)
                    {
                        var crd = available.FirstOrDefault(c => c.Number == v);
                        if (crd != null)
                        {
                            cardsToAdd.Add(crd);
                            available.Remove(crd);
                        }
                    }

                    if (cardsToAdd.Contains(dealerCard))
                    {
                        potentialBuildAlmost.Cards.AddRange(cardsToAdd);
                        Players[1].UserHand.Remove(dealerCard);
                        break;
                    }
                }

                available.Remove(dealerCard);
            }

            // After loop
            if (potentialBuildAlmost.Cards.Any())
            {
                foreach (var player in Players)
                {
                    foreach (var plycrd in potentialBuildAlmost.Cards)
                    {
                        player.UserPile.Remove(plycrd);
                        TableCards.Remove(plycrd);
                    }
                }

                Players[1].UserBuild.Cards.AddRange(potentialBuildAlmost.Cards);
                FinishUp(Players[1].UserBuild.Cards, Players[1].UserBuild.Number);
                return true;
            }

            return false;
        }

        public static bool DealerLongCapture()
        {
            var potentialBuildAlmost = new Build { Number = Players[1].UserBuild.Number, Cards = new List<Card>() };

            var playerPilesAval = Players.Where(c => c.UserPile.Any() && !c.Name.Contains("Dealer")).Select(c => c.UserPile.FirstOrDefault());

            var available = new List<Card>();

            available.AddRange(playerPilesAval);
            available.AddRange(TableCards);
            var isLowerThanNum = Players[1].UserHand.Where(c => c.Number < Players[1].UserBuild.Number).Any();
            foreach (var dealerCard in Players[1].UserHand.Where(c => c.Number < Players[1].UserBuild.Number))
            {
                if (dealerCard.Number == Players[1].UserBuild.Number)
                {
                    var groups = Players[1].UserHand.Select(c => c.Number).GroupBy(c => c);
                    var group = groups.Where(c => c.Key == dealerCard.Number);
                    if (group.Count() < 2)
                    {
                        continue;
                    }
                }
                available = new List<Card>(available.Where(c => c.Number <= Players[1].UserBuild.Number));

                var potentialBuild = new Build { Number = dealerCard.Number, Cards = new List<Card>() };
                potentialBuildAlmost.Number = dealerCard.Number;

                potentialBuildAlmost.Cards.AddRange(available.Where(c => c.Number == potentialBuild.Number));
                if (potentialBuildAlmost.Cards.Any())
                {
                    potentialBuildAlmost.Cards.Add(dealerCard);
                    Players[1].UserHand.Remove(dealerCard);
                    break;
                }
                GenFinisherResults = new List<List<int>>();

                GenFinisher(available.Where(c => c.Number < potentialBuild.Number).Select(c => c.Number).ToList(), potentialBuild.Number);

                var finalList = GenFinisherResults.Where(c => c.Contains(1) || c.Contains(10) || c.Contains(2)).FirstOrDefault();

                if (finalList == null)
                {
                    finalList = GenFinisherResults.FirstOrDefault();
                }

                if (finalList != null)
                {
                    var cardsToAdd = new List<Card>();
                    foreach (var v in finalList)
                    {
                        var crd = available.FirstOrDefault(c => c.Number == v);
                        if (crd != null)
                        {
                            cardsToAdd.Add(crd);
                            available.Remove(crd);
                        }
                    }
                    if (cardsToAdd.Any())
                    {
                        potentialBuildAlmost.Cards.AddRange(cardsToAdd);
                        potentialBuildAlmost.Cards.Add(dealerCard);
                        Players[1].UserHand.Remove(dealerCard);
                        break;
                    }
                }
            }

            // After loop
            if (potentialBuildAlmost.Cards.Any())
            {
                foreach (var player in Players)
                {
                    foreach (var plycrd in potentialBuildAlmost.Cards)
                    {
                        player.UserPile.Remove(plycrd);
                        TableCards.Remove(plycrd);
                    }
                }

                Players[1].UserPile.AddRange(potentialBuildAlmost.Cards);
                FinishUp(Players[1].UserPile, potentialBuildAlmost.Number);

                return true;
            }

            return false;
        }

        public static void DealerCaptureBuild()
        {
            var player = Players[1];
            if (player.UserBuild.Cards.Any())
            {
                var capCard = player.UserHand.FirstOrDefault(c => c.Number == player.UserBuild.Number);
                if (capCard != null)
                {
                    player.UserBuild.Cards.Add(capCard);
                    player.UserPile.AddRange(player.UserBuild.Cards);
                    player.UserBuild = new Build { Number = 0, Cards = new List<Card>(), IsAvailableForSteal = true };
                    player.UserHand.Remove(capCard);
                    FinishUp(player.UserBuild.Cards, player.UserBuild.Number);
                    Console.WriteLine($"Captured build");
                }
            }
        }


        private static void ThrowCard()
        {
            // for now
            var availableForThrow = Players[1].UserHand.Where(c => c.Number != Players[1].UserBuild.Number);
            if (availableForThrow.Any())
            {
                var card = availableForThrow.OrderBy(c => c.Number).FirstOrDefault();

                if (card != null)
                {
                    TableCards.Add(card);

                    Players[1].UserHand.Remove(card);
                }
            }
            else
            {
                if (Players[1].UserHand.Count < 3)
                {
                    if (Players[1].UserHand.Any())
                    {
                        var card = Players[1].UserHand.FirstOrDefault();

                        if (card != null)
                        {
                            TableCards.Add(card);

                            Players[1].UserHand.Remove(card);
                        }
                    }
                }
            }
        }

        private static void FinishUp(List<Card> cards, int buildNumber)
        {
            var potentialBuildAlmost = new Build { Number = Players[1].UserBuild.Number, Cards = new List<Card>() };
            var playerPilesAval = Players.Where(c => c.UserPile.Any() && !c.Name.Contains("Dealer")).Select(c => c.UserPile.FirstOrDefault());

            var available = new List<Card>();

            available.AddRange(playerPilesAval);
            available.AddRange(TableCards);
            var match = true;
            while (match)
            {
                available = new List<Card>(available.Where(c => c.Number <= buildNumber));

                var potentialBuild = new Build { Number = buildNumber, Cards = new List<Card>() };

                potentialBuildAlmost.Cards.AddRange(available.Where(c => c.Number == potentialBuild.Number));

                GenFinisherResults = new List<List<int>>();

                GenFinisher(available.Where(c => c.Number < potentialBuild.Number).Select(c => c.Number).ToList(), potentialBuild.Number);

                var finalList = GenFinisherResults.Where(c => c.Contains(1) || c.Contains(10) || c.Contains(2)).FirstOrDefault();

                if (finalList == null)
                {
                    finalList = GenFinisherResults.FirstOrDefault();
                }

                if (finalList != null)
                {
                    if (!finalList.Any())
                    {
                        match = false;
                        break;
                    }
                    var cardsToAdd = new List<Card>();
                    foreach (var v in finalList)
                    {
                        var crd = available.FirstOrDefault(c => c.Number == v);
                        if (crd != null)
                        {
                            cardsToAdd.Add(crd);
                            available.Remove(crd);
                        }
                    };

                    potentialBuild.Cards.AddRange(cardsToAdd);

                    potentialBuildAlmost.Cards.AddRange(potentialBuild.Cards);
                }
                else
                {
                    match = false;
                    break;
                }
            }

            // After loop

            foreach (var player in Players)
            {
                foreach (var plycrd in potentialBuildAlmost.Cards)
                {
                    player.UserPile.Remove(plycrd);
                    TableCards.Remove(plycrd);
                }
            }

            cards.AddRange(potentialBuildAlmost.Cards);
        }

        //private static Card Display()
        //{
        //    return Players.Select(c => c.UserHand.Count).Sum();
        //}
    }

    public class Card
    {
        public int Number { get; set; }

        public SuitType SuitType { get; set; }

        public string SuitValue { get; set; }

        public string Value { get; set; }

        public bool IsPointCard { get; set; }

        public int PointValue { get; set; }
    }

    public class Player
    {
        public string Name { get; set; }
        public List<Card> UserHand { get; set; }

        public List<Card> UserPile { get; set; }

        public Build UserBuild { get; set; }
    }

    public enum SuitType
    {
        Clubs, Diamonds, Hearts, Spades
    }

    //public class Suits
    //{
    //    public string Clubs { get; set; } = "♣";

    //    public string Diamonds { get; set; } = "♦";

    //    public string Hearts { get; set; } = "♥";

    //    public string Spades { get; set; } = "♠";
    //}

    public class Suits
    {
        public string Clubs { get; set; } = "C";

        public string Diamonds { get; set; } = "D";

        public string Hearts { get; set; } = "H";

        public string Spades { get; set; } = "S";
    }

    public class Build
    {
        public int Number { get; set; }

        public List<Card> Cards { get; set; }

        public bool IsAvailableForSteal { get; set; }
    }
}