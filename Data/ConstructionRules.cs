namespace DroneFactory.Data
{
    public static class ConstructionRules
    {
        public const int MinGenerators = 1;
        public const int MaxGenerators = 2;
        public const int MinMoves = 1;
        public const int MaxMoves = 3;

        public static string Validate(int generatorCount, int moveCount)
        {
            if (generatorCount < MinGenerators || generatorCount > MaxGenerators)
            {
                return "a drone must have between " + MinGenerators + " and "
                    + MaxGenerators + " generators (got " + generatorCount + ")";
            }

            if (moveCount < MinMoves || moveCount > MaxMoves)
            {
                return "a drone must have between " + MinMoves + " and "
                    + MaxMoves + " move modules (got " + moveCount + ")";
            }

            if (moveCount >= 2 && generatorCount != 2)
            {
                return "a drone with 2 or more move modules must have exactly 2 generators (got "
                    + generatorCount + ")";
            }

            return null;
        }
    }
}
