namespace Civlike.Messages;

public sealed record WorldGenerationProgressMessage( string ProgressMessage );

public sealed record WorldGenerationStepProgressPercentMessage( double ProgressPercent );