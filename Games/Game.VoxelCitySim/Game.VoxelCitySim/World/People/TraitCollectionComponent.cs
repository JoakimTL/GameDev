using Engine;
using Engine.Modules.Entity;
using System.Runtime.CompilerServices;

namespace Game.VoxelCitySim.World.People;

[Identifier( "8b7611fe-1959-469f-920d-e42cb18fae7e" )]
public sealed class TraitCollectionComponent : SerializableComponentBase {

	private readonly Dictionary<Type, TraitBase> _traits;

	public TraitCollectionComponent() {
		_traits = [];
	}

	public T GetTrait<T>() where T : TraitBase, new() {
		if (_traits.TryGetValue( typeof( T ), out TraitBase? traitBase ) && traitBase is T trait)
			return trait;
		trait = new();
		_traits[ typeof( T ) ] = trait;
		return trait;
	}

	protected override bool InternalDeserialize( ReadOnlySpan<byte> data ) {
		if (data.Length % 20 != 0) {
			this.LogWarning( "Data not aligned." );
			return false;
		}

		unsafe {
			fixed (byte* dataPtr = data) {
				for (int i = 0; i < data.Length; i += 20) {
					Guid identifier = *(Guid*) (dataPtr + i);
					float value = *(float*) (dataPtr + i + sizeof( Guid ));
					Type? traitType = TraitLibrary.GetTraitType( identifier );
					if (traitType is null)
						continue;
					if (!_traits.TryGetValue( traitType, out TraitBase? traitBase ) || traitBase is not TraitBase trait)
						trait = Activator.CreateInstance( traitType ) as TraitBase ?? throw new Exception( "Unable to create trait." );
					GetTraitValue( trait ) = value;
					_traits[ traitType ] = trait;
				}
			}
		}

		return true;
	}

	protected override bool InternalSerialize( Span<byte> data, out uint writtenBytes ) {
		writtenBytes = 0;
		if (this._traits.Count == 0)
			return true;
		unsafe {
			fixed (byte* dataPtr = data) {
				foreach (TraitBase trait in this._traits.Values) {
					Guid? identifier = TraitLibrary.GetTraitGuid( trait.GetType() );
					if (!identifier.HasValue)
						continue;

					*(Guid*) (dataPtr + writtenBytes) = identifier.Value;
					writtenBytes += (uint) sizeof( Guid );
					*(float*) (dataPtr + writtenBytes) = trait.Value;
					writtenBytes += sizeof( float );
				}
			}
		}
		return true;
	}

	[UnsafeAccessor( UnsafeAccessorKind.Field, Name = "_value" )]
	private static extern ref float GetTraitValue( TraitBase trait );
}
