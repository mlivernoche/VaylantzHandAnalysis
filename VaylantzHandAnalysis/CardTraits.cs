namespace VaylantzHandAnalysis;

[Flags]
public enum CardTraits
{
    None = 0,
    Defensive = 1,
    BeyondThePendMat = 2,
    NormalSummon = 4,
    FieldSpell = 8,

    LowLevelFireVaylantz = 16,
    HighLevelFireVaylantz = 32,

    LowLevelWaterVaylantz = 64,
    HighLevelWaterVaylantz = 128,

    HighScale = 256,
    
    ProsperityTarget = 512,

    MultipleOK = 1048,
}
