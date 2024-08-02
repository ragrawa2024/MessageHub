# MessageHub
Real-time communication between a Central Component and a configurable number of Dependent Components.   

Several approaches were considered. 

1. Using RabbitMQ as the broker between publishers and consumers.
2. Using TCPClient and TCPListener classes to create a TCP/IP messaging system for communication.

Another approach that looked promising was implementing a Pub-Sub system using Delegates and Events.  But more research will be needed since this is a new area for me.

Since the requirements ask that dependent components can talk to each other, we will have to make all the components as both Publishers and Subscribers.  This is similar to what is seen in several Chat clients.
