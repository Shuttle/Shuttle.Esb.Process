# Shuttle.Esb.Process

Process management for Shuttle.Esb endpoints using Shuttle.Recall event sourcing.

[Shuttle.Esb Documentation](http://shuttle.github.io/shuttle-esb/)
[Shuttle.Recall Documentation](http://shuttle.github.io/shuttle-recall/)

[Shuttle.Esb Samples](https://github.com/Shuttle/Shuttle.Esb.Samples)

# Registration / Activation

The required components may be registered by calling `ComponentRegistryExtensions.RegisterProcessManagement(IComponentRegistry)`.

In order to activate the process managgement functionality you may call `ComponentResolverExtensions.ResolveProcessManagement(IComponentResolver)`.

