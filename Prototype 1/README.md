# RabbitMQ based Test chat program

This test program requires RabbitMQ running as a docker container.  Install Docker Desktop.

docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management

Once RabbitMQ is running, use the Terminal window to first start the Receive app.  This acts as the central server to receive client connections.

dotnet Receive.dll

Then run mutiple instances of Send.dll from the RabbitMQ/Send folder.  Each instance of Send is a client.
