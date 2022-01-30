using Core;
using System.Linq;

namespace NodeNetworking.Data.Buffering {
	public interface IBuffer<T> {

		public void Add(T item);
		public void Remove(T item);

		public IQueryable<T> GetAllItems();

		public void Clear();
	}
}
