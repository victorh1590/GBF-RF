# GBF-RF
A Granblue Fantasy Raid Finder implementation in .NET.

>Project Status: In Development.

I'm working on this as a side project when i'm free. This means it will take a while to be on a state it can be deployed and used by the community. There are many considerations to be made about the architecture and design choices.

The project already delivers the basic functionality of connecting to the Twitter API stream, getting the Tweets, parsing them and notifying the clients. But I haven't tested how this implementation scales yet.

Dependencies:

* .NET 6
* Blazor (Front-end)
* SignalR (Websockets)
* xUnit (Tests)