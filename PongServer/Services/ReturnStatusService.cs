using Grpc.Core;
using PongServer;

namespace PongServer.Services;

public class ReturnStatusService : ReturnStatus.ReturnStatusBase
{
    private readonly ILogger<ReturnStatusService> _logger;
    public ReturnStatusService(ILogger<ReturnStatusService> logger)
    {
        _logger = logger;
    }

    public override Task<ReturnReply> RequestReturn(ReturnRequest request, ServerCallContext context)
    {
        bool opponentHit = request.OpponentHit;
        Console.WriteLine(opponentHit);

        string message = "PONG";
        Random rnd = new Random();
        int number = rnd.Next(0, 10);
        if (number > 6)
        {
            message = "MISS";
        } 

        return Task.FromResult(new ReturnReply
        {
            Message = message
        });
    }
}
