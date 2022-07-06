using System.Collections.Concurrent;
using System.Text;

namespace AwaitableProducerSample
{
    internal class AwaitableBlockBuilder : BlockBuilderBase
    {
        private readonly BlockingCollection<(byte[], TaskCompletionSource<string> taskSource)> _writeBuffer;

        public AwaitableBlockBuilder()
        {
            _writeBuffer = new();
        }

        public override void Start()
        {
            Task.Factory.StartNew(BlockBuilding, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        public override void Stop()
        {
            _writeBuffer.CompleteAdding();
        }

        public async Task WriteAsync(string data)
        {
            var taskSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _writeBuffer.Add((Encoding.ASCII.GetBytes(data), taskSource));
            await taskSource.Task;
        }

        public override void Write(string data)
        {
            _writeBuffer.Add((Encoding.ASCII.GetBytes(data), null));
        }

        private void BlockBuilding()
        {
            // Loop when there is a new item in the _writeBuffer, break when _writeBuffer.CompleteAdding() is called (in the dispose)
            foreach (var (bytes, taskSource) in _writeBuffer.GetConsumingEnumerable())
            {
                byte[] input = ObjectToByteArray(new Block { PreviousHash = HashOutput, Data = bytes });
                HashOutput = HashBytes(input);
                NumberOfBlocks++;
                if (NumberOfBlocks <= 10)
                {
                    taskSource?.SetResult(HashOutput);
                }
                else
                {
                    taskSource?.TrySetException(new Exception("Exceed maximum supported blocks of 10"));
                }
            }

            Console.WriteLine("Stopped block builder...");
        }
    }
}
