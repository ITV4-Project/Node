using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking.Data.Buffering {
	internal class Buffer<T> : IBuffer<T> {
		private List<T> _buffer;

		public Buffer() {
			_buffer = new List<T>();
		}

		public void Add(T item) {
			_buffer.Add(item);
		}

		public void Clear() {
			_buffer = new List<T>();
		}

		public IQueryable<T> GetAllItems() {
			return _buffer.AsQueryable();
		}

		public void Remove(T item) {
			_buffer.Remove(item);
		}
	}
}
