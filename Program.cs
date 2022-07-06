using AwaitableProducerSample;

BlockBuilderBase blockBuilder = new AwaitableBlockBuilder();
//BlockBuilderBase blockBuilder = new SimpleBlockBuilder();

blockBuilder.Start();
while(true)
{
    Console.WriteLine("==============================================================");
    Console.WriteLine("Enter any message to create a new block of data in hash. Enter q or quit to stop.");
    var input = Console.ReadLine();
    if (input == "q" || input == "quit")
    {
        blockBuilder.Stop();
        break;
    }

    try
    {
        //blockBuilder.Write(input);
        await ((AwaitableBlockBuilder)blockBuilder).WriteAsync(input);
        Console.WriteLine(blockBuilder.Status);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception is caught - {ex.Message}");
    }
}

