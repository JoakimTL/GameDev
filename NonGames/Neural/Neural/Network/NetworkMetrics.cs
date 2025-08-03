namespace Neural.Network;

public readonly record struct NetworkMetrics(
	   float SimTimeSec,
	   // activity
	   float AvgActivation,
	   float SpikesPerSec_Total,
	   float SpikesPerSec_Input,
	   float SpikesPerSec_ResExc,
	   float SpikesPerSec_ResInh,
	   float SpikesPerSec_Pred,
	   float SpikesPerSec_Act,
	   float MeanFiringRateHz,      // SpikesPerSec_Total / totalNeurons
									// modulators
	   float DopamineDelta,
	   float DopamineRemainingSec,
	   // synapses
	   float WeightSaturationLowPct,
	   float WeightSaturationHighPct,
	   // I/O & housekeeping
	   int EmissionsInWindow,
	   int PendingInputCount,
	   float ActTop,
	   float ActGap,
	   float PredictionTop,
	   float PredictionGap
   );