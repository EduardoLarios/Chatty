# Chatty
Programming Languages project. WPF based chat app. 

A small chat application that creates a server on the host machine with C# and .NET Core 2.1, as well as a WPF client that uses .NET Framework 4.7 for the GUI. The solution exists at the moment as a proof of concept that uses a client-server architecture on a local scale to allow communication between parties in a local network. The implementation makes use of threads in both the client and the server to serve different purposes, such as handling each TCP connection on the server and separating the GUI and logic threads on the client.
