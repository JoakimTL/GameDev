namespace Civlike.Messages;

public sealed record WorldGenerationProgressMessage( string ProgressMessage );

public sealed record WorldGenerationCompleteMessage( string Message );

public sealed record WorldGenerationSubProgressMessage( string SubProgressMessage );

public sealed record WorldGenerationStepProgressPercentMessage( double ProgressPercent );