using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Data {
	public class UniqueQueue<T> : IEnumerable<T> {
		private HashSet<T> hashSet;
		private Queue<T> queue;

		public UniqueQueue() {
			hashSet = new HashSet<T>();
			queue = new Queue<T>();
		}

		public UniqueQueue( UniqueQueue<T> q ) {
			hashSet = new HashSet<T>( q.hashSet );
			queue = new Queue<T>( q.queue );
		}

		public int Count {
			get {
				return hashSet.Count;
			}
		}

		public void Clear() {
			hashSet.Clear();
			queue.Clear();
		}

		public bool Contains( T item ) {
			return hashSet.Contains( item );
		}

		public void Enqueue( T item ) {
			lock( queue ) {
				lock( hashSet ) {
					if( hashSet.Add( item ) ) {
						queue.Enqueue( item );
					}
				}
			}
		}

		public T Dequeue() {
			lock( queue ) {
				lock( hashSet ) {
					T item = queue.Dequeue();
					hashSet.Remove( item );
					return item;
				}
			}
		}

		public bool TryDequeue( out T o ) {
			lock( queue ) {
				lock( hashSet ) {
					bool r = queue.TryDequeue( out o );
					if( r )
						hashSet.Remove( o );
					return r;
				}
			}
		}

		public T Peek() {
			return queue.Peek();
		}

		public IEnumerator<T> GetEnumerator() {
			return queue.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return queue.GetEnumerator();
		}
	}
}
