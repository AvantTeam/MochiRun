using System.Collections.Generic;
using System;

//priority queue by https://hairy.geek.nz/2019/06/27/csharp-prio-queue.html
namespace DataStructures.Queue {
	public class PriorityQueueEntry<TPrio, TItem> {
		public TPrio p { get; }
		public TItem data { get; }
		public PriorityQueueEntry(TPrio p, TItem data) {
			this.p = p;
			this.data = data;
		}
	}

	public class PriorityQueue<TPrio, TItem> where TPrio : IComparable {
		private LinkedList<PriorityQueueEntry<TPrio, TItem>> q;

		public PriorityQueue() {
			q = new LinkedList<PriorityQueueEntry<TPrio, TItem>>();
		}

		public int Count { get { return q.Count; } }

		public void Enqueue(TPrio p, TItem data) {
			if(q.Count == 0) {
				q.AddFirst(new PriorityQueueEntry<TPrio, TItem>(p, data));
				return;
			}
			// This is a bit classical C but whatever
			LinkedListNode<PriorityQueueEntry<TPrio, TItem>> current = q.First;
			while(current != null) {
				if(current.Value.p.CompareTo(p) > 0) {
					q.AddBefore(current, new PriorityQueueEntry<TPrio, TItem>(p, data));
					return;
				}
				current = current.Next;
			}
			q.AddLast(new PriorityQueueEntry<TPrio, TItem>(p, data));
		}

		public TItem Dequeue() {
			// LinkedList -> LinkedListNode -> PriorityQueueEntry -> data
			var ret = q.First.Value.data;
			q.RemoveFirst();
			return ret;
		}

		public TItem Top() {
			return q.First.Value.data;
		}
	}
}