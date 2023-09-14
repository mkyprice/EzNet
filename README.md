# EzNet
## An easy networking library

EzNet is a simple to use networking library 

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

## License

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
