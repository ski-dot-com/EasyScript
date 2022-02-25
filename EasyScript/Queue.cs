using System.Runtime.Serialization;

namespace EasyScript
{
    public class Queue<T> : LinkedList<T>
    {
        public Queue()
        {
        }

        public Queue(int capacity) : base(new List<T>(capacity))
        {
        }

        public Queue(IEnumerable<T> collection) : base(collection)
        {
        }

        protected Queue(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        public T Peek()
        {
            LinkedListNode<T>? tmp = First;
            if (tmp is null)
            {
                throw new InvalidOperationException("キューが空でした。");
            }
            else
            {
                return tmp.Value;
            }
        }
        public T Dequeue()
        {
            LinkedListNode<T>? tmp = First;
            RemoveFirst();
            if (tmp is null)
            {
                throw new InvalidOperationException("キューが空でした。");
            }
            else
            {
                return tmp.Value;
            }
        }
        public void Enqueue(T t) => AddLast(t);
    }
    public class Deq<T> : Queue<T>
    {
        public Deq()
        {
        }

        public Deq(int capacity) : base(capacity)
        {
        }

        public Deq(IEnumerable<T> collection) : base(collection)
        {
        }

        protected Deq(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public T RDequeue()
        {
            LinkedListNode<T>? tmp = Last;
            RemoveLast();
            if (tmp is null)
            {
                throw new InvalidOperationException("キューが空でした。");
            }
            else
            {
                return tmp.Value;
            }
        }
        public void REnqueue(T t) => AddFirst(t);
        public T RPeek()
        {
            LinkedListNode<T>? tmp = Last;
            if (tmp is null)
            {
                throw new InvalidOperationException("キューが空でした。");
            }
            else
            {
                return tmp.Value;
            }
        }
    }
}
