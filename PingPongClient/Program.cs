using System.Threading.Tasks;
using Grpc.Net.Client;
using PingPongClient;


// Establish a return client for the PingServer
using var returnChannelPing = GrpcChannel.ForAddress("https://localhost:7168");
var returnClientPing = new ReturnStatus.ReturnStatusClient(returnChannelPing);
Globals.connections.Add("PING", returnClientPing);
Globals.previousWinner = Globals.connections["PING"];

// Establish a return client for the PongServer
using var returnChannelPong = GrpcChannel.ForAddress("https://localhost:7210");
var returnClientPong = new ReturnStatus.ReturnStatusClient(returnChannelPong);
Globals.connections.Add("PONG", returnClientPong);


// Ping Pong Logic
bool over = false;

// Game Loop
while(over == false)
{
    over = await PlayPoint(Globals.connections["PING"], Globals.connections["PONG"], Globals.previousWinner);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();


async Task<bool> PlayPoint(ReturnStatus.ReturnStatusClient clientPing, ReturnStatus.ReturnStatusClient clientPong, ReturnStatus.ReturnStatusClient previousWinner)
{
    // Determine who will return the serve
    ReturnStatus.ReturnStatusClient returningPlayer;
    if (previousWinner == clientPing)
    {
        returningPlayer = clientPong;
    }
    else{
        returningPlayer = clientPing;
    }

    // Mimic Previous Winner Serving
    Console.WriteLine("Serve is up!");

    // Simulate a point being played
    bool pointInPlay = true;
    while(pointInPlay)
    {
        // Reach out over gRPC to returning player and check if they return the point
        var returnReply = await returningPlayer.RequestReturnAsync(
                  new ReturnRequest { OpponentHit = true });
        Console.WriteLine("Return Status: " + returnReply.Message);

        // If returned, switch returningPlayer to opposite player. Else, update the score and end the point.
        if (returnReply.Message == "PING")
        {
            returningPlayer = clientPong;
        }
        else if (returnReply.Message == "PONG")
        {
            returningPlayer = clientPing;
        }
        else
        {
            SetPreviousWinnerAndUpdateScore(returningPlayer);
            pointInPlay = false;
        }
    }
    return Over();
}

bool Over()
{
    if (Globals.pingScore >= 100 || Globals.pongScore >= 100)
    {
        Console.WriteLine("Game over! " + "Final score: Ping Server - " + Globals.pingScore + " Pong Server - " + Globals.pongScore);
        return true;
    }
    return false;
}

void SetPreviousWinnerAndUpdateScore(ReturnStatus.ReturnStatusClient returningPlayer)
{
    if (returningPlayer == Globals.connections["PONG"])
    {
        Globals.previousWinner = Globals.connections["PING"];
        Globals.pingScore++;
    }
    else{
        Globals.previousWinner = Globals.connections["PONG"];
        Globals.pongScore++;
    }
}

// Keep track of global variables
public static class Globals{
    public static IDictionary<string, ReturnStatus.ReturnStatusClient> connections = new Dictionary<string, ReturnStatus.ReturnStatusClient>();
    public static ReturnStatus.ReturnStatusClient? previousWinner = null;
    public static int pingScore = 0;
    public static int pongScore = 0;
}