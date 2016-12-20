# Hive
parallel task execution using a simple client / server model

## Features

* Built using cross platform technology (Linux / Windows / Mac OS compatibility)
* UDP or HTTP API to interact with Queen (Server)
* Allows for automatic provisioning of drones
* Communication between Queen (Server) and Drones (Nodes) is done though UDP
* Queue and Schedule tasks to be completed by Drones
* Provisioning and Data Storage can be replaced or extended though plugins

## Dependencies

* [Lidgren](https://github.com/lidgren/lidgren-network-gen3) - UDP Networking
* [Netonsoft.Json](http://www.newtonsoft.com/json) - Json serialization and deserialization 
* [NUnit](https://www.nunit.org) - Unit Testing
* [EmbedIO](https://unosquare.github.io/embedio) - Embedded Web Server
* [NLog](http://nlog-project.org) - Logging

## Build

You must have a version of Visual Studio or msbuild version 4.0.30319 installed.

For a first time build run:

    .\prereqs.ps1

To build the solution run:

    .\build.ps1

## More information

Please see the [Wiki](https://github.com/SwarmAutomation/Hive/wiki) for more information.
