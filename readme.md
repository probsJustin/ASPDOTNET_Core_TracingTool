# ASP .NET Core Tracing Reproducer Tool 
**Preface:** This tools whole mission in life is to run specific transactions and methods with in ASP .NET Core. It was designed at Dynatrace and it works in conjunction with other reproducers and tools to help easily reproduce fake transactions and requests across multiple technologies. This tool is maintained by the Support Lab Team. 

## Getting Started: 

### Using Local Support Lab Pre-built Docker Hub Images (preferred) : 
TODO: list pre-built docker containers hosted in docker hub

### Docker (Manual): 
Go ahead and pull down the specific image that you need for this instance and make sure that you have the correct framework pertaining to the situation.
 
I went ahead and used .NET Framework 3.1
``docker pull mcr.microsoft.com/dotnet/core/sdk:3.1``

After that we will go ahead and spin up the container for the application. We will map port 5000 on the container to port 2000 on the host machine. We will also name the container "suplab_aspcore_tracing"
``docker run -it --rm -p 2000:5000 --name "suplab_aspcore_tracing" mcr.microsoft.com/dotnet/core/sdk:3.1``

Then we will copy over the application that you have cloned from bitbucket. Go to that directory and run the following command (make sure to pick the release that fits your need):
Note: that if any changes were made to the application that you need to then build the app and publish it again.  
``cd ./bin/Release/netcoreapp3.1``

Then copy the contents to the "/app" folder of the container: 
``docker cp ./ suplab_aspcore_tracing:/app``

Next go back to the container and run the following command: 
``dotnet /app/ASPCORE_TracingTool.dll`` 

Now you can go to your browser on your host machine and type in your local IP at port 2000 and it should bring directly to the app. 
Example: 
``http://192.168.1.81:2000/``

### Local Machine: 

You can easily pull in the repository and build the application:
Note: There are multiple ways to build the application. Two main schools of thought on this are through the visual studio publish tool and publishing it to a local folder or using the dotnet tool to build the application. Here is the documentation for the dotnet tool regarding the build parameter: [https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build)
In both of these you can easily set the framework that you want build the application for. 

After the application is built you can go to where you have your .dll for your application and complete similar steps of the docker tutorial: 
Run the following command: 
``dotnet ./ASPCORE_TracingTool.dll`` 

Now you can go to your browser on your host machine and type in your local IP at port 5000 (default port for the app) and it should bring directly to the app. 
Example: 
``http://localhost:5000/``

## API Endpoints: 

## Debug Parameters: 

## Debug Headers:

##  .NET Remoting: 

## Threading and Forking:

## Notes: 

## To Do: 

## Report an Issue: 