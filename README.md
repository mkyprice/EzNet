# EzNet
## An easy to use networking library

EzNet is a simple to use networking library 

> 🚧 **Warning** Proceed at your own risk. This is an in-development library so it will change _a lot_. I'll try to keep the master branch stable, but expect many breaking changes.

## Features

- Async Sending/Receiving
- Easy to swap Transport layer
- Minimal allocations
- Define functions to handle your packet types
- Define function to handle your request functions

## Usage

```cs
// Create a tcp connection
using Connection client = ConnectionFactory.BuildClient(endpoint, isReliable);
// Register our response function for all MyPacket types
connection.RegisterResponseHandler<float, MyPacket>((p) => 
{
    return p.A + p.B;
});
```

## Dependancies

EzNet has no external dependancies
It targets netstandard2.0 and net461 and uses C#8
