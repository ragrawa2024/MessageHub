# RabbitMQ based Test chat program

Using RabbitMQ to create a producer & consumer system.   RabbitMQ is a free and extensive message queuing system.  It uses AMQP as its underlying protocol and it can also work with MQTT clients.
Since our exercise needs a central component, we could have 1 consumer play that part.   For the dependent components, we will treat those as clients.   The clients will be both producers and consumers and can choose to subscribe to any topic.

For the prototype, we need to install Docker Desktop on the machine and then run RabbitMQ in a container.  

docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management

Once RabbitMQ container is running in Docker Desktop, use the Terminal window to first start the Receive app from RabbitMQ/Receive folder.  This acts as the central server to receive client connections.

dotnet Receive.dll

Then run multiple instances of Send.dll from the RabbitMQ/Send folder.  Each instance of Send is a client.


