using Neural.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neural.UserInterface;
public partial class MainWindow : Form {
	private readonly NeuralNetInterface _neuralNetInterface;
	private readonly NeuralNetInterface _neuralNetInterface2;

	private string _debugString1;
	private string _debugString2;
	private long _lastRepeat = 0;

	public MainWindow() {
		InitializeComponent();
		_neuralNetInterface = new();
		_neuralNetInterface.SetInputRate( 50 );
		_neuralNetInterface.MetricsUpdated += UpdateDebug;
		_neuralNetInterface2 = new();
		_neuralNetInterface2.SetInputRate( 50 );
		_neuralNetInterface2.MetricsUpdated += UpdateDebug2;
		//_neuralInterface.OnNeuralNetworkReady += OnNeuralNetworkReady;
		//_neuralInterface.OnOutputReceived += OnOutputReceived;
		//_neuralInterface.OnNeuronActivated += OnNeuronActivated;
		//_neuralInterface.OnCharacterSupplied += OnCharacterSupplied;

	}

	private void UpdateDebug( NetworkMetrics m ) {
		_debugString1 = $"""
			[{m.SimTimeSec,6:0.00}s]
			AvgAct={m.AvgActivation:0.000}
			Spk/s: tot={m.SpikesPerSec_Total:0} In={m.SpikesPerSec_Input:0}
			ResE={m.SpikesPerSec_ResExc:0} ResI={m.SpikesPerSec_ResInh:0}
			Pred={m.SpikesPerSec_Pred:0} Act={m.SpikesPerSec_Act:0}
			FR={m.MeanFiringRateHz:0.00}Hz
			DA δ={m.DopamineDelta:+0.000;-0.000;0.000}({m.DopamineRemainingSec * 1000:0}ms)
			Sat[lo/hi]={m.WeightSaturationLowPct:0.0}%/{m.WeightSaturationHighPct:0.0}%
			Emit/s={m.EmissionsInWindow / Math.Max( 1f, _neuralNetInterface.MetricsIntervalSec ):0.00}
			PendingIn={m.PendingInputCount}
			Act top={m.ActTop:0.000} gap={m.ActGap:0.000}
			Pred top={m.PredictionTop:0.000} gap={m.PredictionGap:0.000}
			""";
	}

	private void UpdateDebug2( NetworkMetrics m ) {
		_debugString2 = $"""
			[{m.SimTimeSec,6:0.00}s]
			AvgAct={m.AvgActivation:0.000}
			Spk/s: tot={m.SpikesPerSec_Total:0} In={m.SpikesPerSec_Input:0}
			ResE={m.SpikesPerSec_ResExc:0} ResI={m.SpikesPerSec_ResInh:0}
			Pred={m.SpikesPerSec_Pred:0} Act={m.SpikesPerSec_Act:0}
			FR={m.MeanFiringRateHz:0.00}Hz
			DA δ={m.DopamineDelta:+0.000;-0.000;0.000}({m.DopamineRemainingSec * 1000:0}ms)
			Sat[lo/hi]={m.WeightSaturationLowPct:0.0}%/{m.WeightSaturationHighPct:0.0}%
			Emit/s={m.EmissionsInWindow / Math.Max( 1f, _neuralNetInterface2.MetricsIntervalSec ):0.00}
			PendingIn={m.PendingInputCount}
			Act top={m.ActTop:0.000} gap={m.ActGap:0.000}
			Pred top={m.PredictionTop:0.000} gap={m.PredictionGap:0.000}
			""";
	}

	private void txtIn_KeyUp( object sender, KeyEventArgs e ) {
		if (e.KeyCode == Keys.Enter && txtIn.Focused)
			SendMessage();
	}

	private void SendMessage() => SendMessage( txtIn.Text );

	private void SendMessage( string message ) {
		message = message.Trim();
		_neuralNetInterface.EnqueueInput( message );
		_neuralNetInterface2.EnqueueInput( message );
		txtOut.Text += $"User: {message}{Environment.NewLine}";
		txtIn.Text = string.Empty;
	}

	private void globalTimer_Tick( object sender, EventArgs e ) {

		while (_neuralNetInterface.TryDequeueEmission( out string message )) {
			txtOut.Text += $"NN1: {message}{Environment.NewLine}";
			_neuralNetInterface2.EnqueueInput( message );
		}

		while (_neuralNetInterface2.TryDequeueEmission( out string message )) {
			txtOut.Text += $"NN2: {message}{Environment.NewLine}";
			_neuralNetInterface.EnqueueInput( message );
		}

		txtDebug.Text = _debugString1;
		txtDebug2.Text = _debugString2;
		txtCharDebug.Text = _neuralNetInterface.BufferSnapshot;
		txtCharDebug2.Text = _neuralNetInterface2.BufferSnapshot;

		long now = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
		if (int.TryParse( txtInterval.Text, out int interval ) && now - _lastRepeat > interval) {
			_lastRepeat = now;
			string message = txtRepeatMessage.Text;
			if (!string.IsNullOrEmpty( message ))
				SendMessage( txtRepeatMessage.Text );
		}
	}
}
