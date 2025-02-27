public class MidBeat
{
    public double baseTime;
    public double minimumTime;
    public double maximumTime;

    public bool used;

    BeatManager.BeatType beatType;

    public MidBeat(double baseTime, BeatManager.BeatType beatType)
    {
        this.baseTime = baseTime;

        double beatDuration = BeatManager.instance.secondsPerBeat * 0.5f;
        double successRange = beatDuration / 3f; // 3 -> Normal, 3.5 -> Hard, 4 -> Dancer

        minimumTime = baseTime - successRange;
        maximumTime = baseTime + successRange;
        used = false;
        this.beatType = beatType;
    }
}