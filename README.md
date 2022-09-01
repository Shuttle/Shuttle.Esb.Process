# Shuttle.Esb.Process

Process management for Shuttle.Esb endpoints using Shuttle.Recall event sourcing.

[Shuttle.Esb Documentation](http://shuttle.github.io/shuttle-esb/)
[Shuttle.Recall Documentation](http://shuttle.github.io/shuttle-recall/)

[Shuttle.Esb Samples](https://github.com/Shuttle/Shuttle.Esb.Samples)

# Configuration

Add the process management services to the `IServiceCollection` as follows:

```c#
services.AddProcessManagement(builder => {
	builder.AddAssembly(assembly);
	builder.AddAssembly("assemblyName");
});
```

The `builder.AddAssembly()` method will result in all classes that implement `IProcessMessageAssessor` being added to the `IMessageHandlingAssessor` as well as registering the appropriate mappings in the `ProcessActivator` of the `IProcessMessageHandler<>` and `IProcessStartMessageHandler<>` interface implementations.