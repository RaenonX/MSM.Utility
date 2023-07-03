using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MSM.Common.Models;

namespace MSM.Common.Interfaces;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "empty")]
[JsonDerivedType(typeof(PxBarModel), "false")]
[JsonDerivedType(typeof(PxBarEmptyModel), "true")]
public interface IPxBarModel {
    [UsedImplicitly]
    public long EpochSecond { get; init; }

    [UsedImplicitly]
    public bool Empty { get; }
}