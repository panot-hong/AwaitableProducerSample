using System.Collections.Concurrent;
using System.Text;

namespace AwaitableProducerSample
{
    internal class SimpleBlockBuilder : BlockBuilderBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ConcurrentQueue<byte[]> _writeBuffer;

        public SimpleBlockBuilder()
        {
            _cancellationTokenSource = new();
            _writeBuffer = new();
        }

        public override void Start()
        {
            Task.Factory.StartNew(BlockBuilding, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
        }

        public override void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public override void Write(string data)
        {
            _writeBuffer.Enqueue(Encoding.ASCII.GetBytes(data));
        }

        private async void BlockBuilding()
        {
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                while (_writeBuffer.TryDequeue(out byte[] bytes))
                {
                    if (NumberOfBlocks < 10)
                    {
                        byte[] input = ObjectToByteArray(new Block { PreviousHash = HashOutput, Data = bytes });
                        HashOutput = HashBytes(input);
                        NumberOfBlocks++;
                    }
                    else
                    {
                        Console.WriteLine("Exceed maximum supported blocks of 10");
                    }
                }

                // Take some rest...
                await Task.Delay(200);
            }

            Console.WriteLine("Stopped block builder...");
        }
    }
}
