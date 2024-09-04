using Models;

namespace DataTransferModels.ValueObjects
{
    public class Dice(int number) : ValueObject
    {
        public static Dice From(int number)
        {
            var Dice = new Dice(number);

            if (!SupportedDices.Contains(Dice))
            {
                throw new Exception();
            }

            return Dice;
        }


        public static Dice One => new(1);
        public static Dice Two => new(2);
        public static Dice Three => new(3);
        public static Dice Four => new(4);
        public static Dice Five => new(5);
        public static Dice Six => new(6);

        public static Dice X => new(-1);
        public static Dice TwentyFour => new(24);


        public int Number { get; private set; } = number;

        public static implicit operator string(Dice colour)
        {
            return colour.ToString();
        }

        public static explicit operator Dice(int code)
        {
            return From(code);
        }

        public override string ToString()
        {
            return Number.ToString();
        }

        protected static IEnumerable<Dice> SupportedDices
        {
            get
            {
                yield return One;
                yield return Two;
                yield return Three;
                yield return Four;
                yield return Five;
                yield return Six;
                yield return X;
                yield return TwentyFour;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
        }
    }
}
