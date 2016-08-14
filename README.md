# statsquo
Maintain the status quo of your microservices

### What is it?
Stats quo takes inspiration from existing stats aggregators like *statsd* and *telegraf*.

The primary usecase we are focusing on is for it to be *embedded into microservice sidecars*; its feature set will therefore be relatively limited as it should be lightweight.

It is written in .Net (and soon enough *dotnet core*) so should be portable to all platforms (win, mac, linux). It will be available as an executable as well as a library, meaning it can be used as either a standalone program/service or as a library within any other applications.

### Architecture
TBA

#### Accumulators
- udp
- tcp (planned)
- http (planned)
- ...

#### Backends
- console
- influxdb (planned)
- graphite (planned)
- sql (planned)
- ...
