public static class ShotEvaluator
{
    public static HoleResult Evaluate(int holeIndex, int shots, int par)
    {
        ShotResultType result;
        int score;

        if (shots == 1)
        {
            result = ShotResultType.HoleInOne;
            score = 10;
        }
        else if (shots < par)
        {
            result = ShotResultType.Birdie;
            score = 5;
        }
        else if (shots == par)
        {
            result = ShotResultType.Par;
            score = 3;
        }
        else
        {
            result = ShotResultType.Bogey;
            score = 1;
        }

        return new HoleResult
        {
            HoleIndex = holeIndex,
            Shots = shots,
            Par = par,
            Result = result,
            Score = score
        };
    }
}
public enum ShotResultType
{
    HoleInOne,
    Birdie,
    Par,
    Bogey
}

public class HoleResult
{
    public int HoleIndex;
    public int Par;
    public int Shots;
    public ShotResultType Result;
    public int Score;
}
