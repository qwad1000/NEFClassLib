using NEFClassLib.FuzzyNumbers;

namespace NEFClassLib.Partitions
{
    public interface IPartition<FuzzyType>
    {
        FuzzyType this[int index] { get; }
        int PartitionSize { get; }

        double GetMaxMembership(double x);
        int GetMaxMembershipIndex(double x);
        double[] GetMembershipVector(double x);
    }
}