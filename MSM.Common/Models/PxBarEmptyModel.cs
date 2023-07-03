using MSM.Common.Interfaces;

namespace MSM.Common.Models;

public class PxBarEmptyModel : IPxBarModel {
    public required long EpochSecond { get; init; }

    public bool Empty => true;
}