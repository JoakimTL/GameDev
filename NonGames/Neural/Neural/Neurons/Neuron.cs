using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural.Neurons;
public sealed class Neuron {

	private readonly List<Connection> _connections;

	public float ExcitationLevel { get; private set; }
	public float ExhaustionLevel { get; private set; }
	public float Value { get; set; }

	public IReadOnlyList<Connection> Connections => _connections;

	public Neuron( float excitationLevel ) {
		ExcitationLevel = excitationLevel;
		ExhaustionLevel = 0;
		Value = 0;
		_connections = [];
	}

	public void Activate() {
		ExhaustionLevel *= 0.95f;
		if (Value > ExhaustionLevel)
			Value = 0;
		foreach (var connection in Connections)
			connection.TriggerConnection( this );
	}

	public void Trigger( float activationValue ) {
		if (activationValue < ExcitationLevel)
			return;

		if (activationValue <= ExhaustionLevel)
			return;

		ExhaustionLevel += activationValue;
		Value = activationValue;
	}

}

public sealed class Connection {
	public Neuron Target { get; set; }
	public float Weight { get; set; }
	public Connection( Neuron target, float weight ) {
		Target = target;
		Weight = weight;
	}

	public void TriggerConnection( Neuron source ) {
		Target.Trigger( source.Value * Weight );
	}
}

public sealed class Brain {

	private readonly List<Neuron> _neurons;
	private readonly List<Neuron> _inputNeurons;
	private readonly List<Neuron> _outputNeurons;


	public IReadOnlyList<Neuron> Neurons => _neurons;
	public IReadOnlyList<Neuron> InputNeurons => _inputNeurons;
	public IReadOnlyList<Neuron> OutputNeurons => _outputNeurons;

	public Brain(int inputNeurons, int outputNeurons, int intermediaryNeurons) {
		_neurons = [];
		_inputNeurons = [];
		_outputNeurons = [];

		for (int i = 0; i < inputNeurons; i++) {
			var neuron = new Neuron( 0 );
			_neurons.Add( neuron );
			_inputNeurons.Add( neuron );
		}

		for (int i = 0; i < intermediaryNeurons; i++) {
			var neuron = new Neuron( 0 );
			_neurons.Add( neuron );
		}

		for (int i = 0; i < outputNeurons; i++) {
			var neuron = new Neuron( 0 );
			_neurons.Add( neuron );
			_outputNeurons.Add( neuron );
		}
	}

}