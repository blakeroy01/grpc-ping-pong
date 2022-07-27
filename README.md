# grpc-ping-pong
The goal of this project is to familiarize myself with .NET, gRPC, protocol buffers, and redis.

The project architecture can be described by the following.

There will be 2 .NET servers running with a hardcoded variable of `PlayTo`. This variable will determine at what score the game of ping pong ends. Each server will connect to a redis instance, then start sending "Ping" and "Pong" back and forth over gRPC. The chance that each server "returns" the network call is 60%.

If either server returns "Miss", the receiving server will update the score in Redis. If the score is >= `PlayTo`, that server will tell the other server that it has lost.
