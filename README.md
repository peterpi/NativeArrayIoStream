# NativeArrayIoStream
A C# [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream) that reads and writes to a Unity `NativeArray<byte>`


### Why would you use one? ###

You might use this in the same way as a regular C# MemoryStream.  However if you use a NativeArray<byte> (and a NativeArrayIoStream around it) then the NativeArray will bypass the managed heap, reducing fragmentation.

### Usage ###

Just create the NativeArrayIoStream, passing in an existing NativeArray:

```
NativeArray<byte> array = ...;

using (var stream = new NativeArrayIoStream(array)) {
    // e.g. 
    someJsonDocument.WriteTo(stream)
}
```

### Notes ###

* `Dispose()`-ing the stream does not dispose the underlying array.  This is by design.  Adding a flag to do so is a possible feature request.
