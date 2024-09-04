using ApplicationTemplate.Server.Services.IServices;
using DataTransferModels.ValueObjects;
using Domain.Constants;

namespace ApplicationTemplate.Server.Services
{
    public class DiceService : IDiceService
    {
        private const int _diceSideCount = 6;

        public static readonly ReadOnlyMemory<Dice> NormalDices = new Dice[_diceSideCount]
        {
            Dice.One, Dice.Two, Dice.Three, Dice.Four, Dice.Five, Dice.Six
        };

        public static readonly ReadOnlyMemory<Dice> SpecialDices = new Dice[_diceSideCount]
        {
            Dice.X, Dice.Two, Dice.Three, Dice.Four, Dice.Five, Dice.TwentyFour
        };

        private static readonly DiceSet _normalDiceSet = new(NormalDices.ToArray());
        private static readonly DiceSet _specialDiceSet = new(SpecialDices.ToArray());

        public async Task<int> RollDiceSet(DiceSetTypes diceSetTypes)
            => await Roll(GetDiceSetsByType(diceSetTypes));

        private static List<DiceSet> GetDiceSetsByType(DiceSetTypes diceSetTypes)
        {
            return diceSetTypes switch
            {
                DiceSetTypes.Normal3Dice => [_normalDiceSet, _normalDiceSet, _normalDiceSet],
                DiceSetTypes.XAnd24Dice => [_specialDiceSet],
                _ => throw new ArgumentException("DiceSetTypes is not valid"),
            };
        }

        private static Task<int> Roll(List<DiceSet> diceSets)
        {
            Random random = new();
            int sum = 0;

            foreach (var diceSet in diceSets)
            {
                int roll = random.Next(0, 6);

                var dice = diceSet.Dices[roll];

                if (dice == Dice.X) return Task.FromResult(-1); // Immediate loss if "X" is rolled

                sum += dice.Number; // Add the dice number to the sum
            }

            return Task.FromResult(sum);
        }

        private class DiceSet
        {
            public List<Dice> Dices { get; }

            public DiceSet(Dice[] dices)
            {
                var diceList = dices.ToList();

                if (diceList.Count != _diceSideCount)
                    throw new ArgumentException($"DiceSet must contain exactly {_diceSideCount} dice.");

                Dices = diceList;
            }
        }
    }
}
