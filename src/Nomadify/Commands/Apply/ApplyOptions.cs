using System.Collections.Generic;
using Nomadify.Commands.Base;
using Nomadify.Commands.Options;

namespace Nomadify.Commands.Apply;

public class ApplyOptions : BaseCommandOptions, IContainerOptions
{
    public string? OutputPath { get; set; }

    //public string? Region { get; set; }
    //public string? Namespace { get; set; }
    //public string? Id { get; set; }
    //public string? ParentId { get; set; }
    //public string? Name { get; set; }
    //public string? Type { get; set; }
    //public int? Priority { get; set; }
    //public bool AllAtOnce { get; set; }
    //public string?[]? Datacenters { get; set; }
    //public string? NodePool { get; set; }
    //public Constraint?[]? Constraints { get; set; }
    //public Taskgroup?[]? TaskGroups { get; set; }
    //public Update? Update { get; set; }
    //public Periodic? Periodic { get; set; }
    //public Parameterizedjob? ParameterizedJob { get; set; }
    //public object? Payload { get; set; }
    //public Meta? Meta { get; set; }
    //public string? VaultToken { get; set; }
    //public string? Status { get; set; }
    //public string? StatusDescription { get; set; }
    //public int? CreateIndex { get; set; }
    //public int? ModifyIndex { get; set; }
    //public int? JobModifyIndex { get; set; }
    public string? ContainerBuilder { get; set; }
    public string? ContainerRegistry { get; set; }
    public string? ContainerRepositoryPrefix { get; set; }
    public List<string>? ContainerImageTags { get; set; }
}

//public class Update
//{
//    public long? Stagger { get; set; }
//    public int? MaxParallel { get; set; }
//}

//public class Periodic
//{
//    public bool? Enabled { get; set; }
//    public string? Spec { get; set; }
//    public string? SpecType { get; set; }
//    public bool? ProhibitOverlap { get; set; }
//}

//public class Parameterizedjob
//{
//    public string? Payload { get; set; }
//    public string?[]? MetaRequired { get; set; }
//    public string?[]? MetaOptional { get; set; }
//}

//public class Meta
//{
//    public string? Foo { get; set; }
//    public string? Baz { get; set; }
//}

//public class Constraint
//{
//    public string? LTarget { get; set; }
//    public string? RTarget { get; set; }
//    public string? Operand { get; set; }
//}

//public class Taskgroup
//{
//    public string? Name { get; set; }
//    public int? Count { get; set; }
//    public Constraint?[]? Constraints { get; set; }
//    public Affinity?[]? Affinities { get; set; }
//    public RestartPolicy? RestartPolicy { get; set; }
//    public TaskObject?[]? Tasks { get; set; }
//    public EphemeralDisk? EphemeralDisk { get; set; }
//    public Meta? Meta { get; set; }
//}

//public class RestartPolicy
//{
//    public int? Attempts { get; set; }
//    public long? Interval { get; set; }
//    public long? Delay { get; set; }
//    public string? Mode { get; set; }
//}

//public class EphemeralDisk
//{
//    public bool? Sticky { get; set; }
//    public int? SizeMb { get; set; }
//    public bool? Migrate { get; set; }
//}

//public class Affinity
//{
//    public string? LTarget { get; set; }
//    public string? RTarget { get; set; }
//    public string? Operand { get; set; }
//    public int? Weight { get; set; }
//}

//public class TaskObject
//{
//    public Config? Config { get; set; }
//    public string? Driver { get; set; }
//    public Lifecycle? Lifecycle { get; set; }
//    public string? Name { get; set; }
//    public Resources? Resources { get; set; }
//    public string? User { get; set; }
//    public Env? Env { get; set; }
//    public Service?[]? Services { get; set; }
//    public object? Vault { get; set; }
//    public Template?[]? Templates { get; set; }
//    public object? Constraints { get; set; }
//    public object? Affinities { get; set; }
//    public DispatchPayload? DispatchPayload { get; set; }
//    public Meta? Meta { get; set; }
//    public long? KillTimeout { get; set; }
//    public LogConfig? LogConfig { get; set; }
//    public Artifact?[]? Artifacts { get; set; }
//    public bool? Leader { get; set; }
//}

//public class Config
//{
//    public string? Command { get; set; }
//    public string? Image { get; set; }
//    public PortMap?[]? PortMap { get; set; }
//}

//public class PortMap
//{
//    public int? Db { get; set; }
//}

//public class Lifecycle
//{
//    public string? Hook { get; set; }
//    public bool? Sidecar { get; set; }
//}

//public class Resources
//{
//    public int? Cpu { get; set; }
//    public int? MemoryMb { get; set; }
//    public int? DiskMb { get; set; }
//    public Network?[]? Networks { get; set; }
//}

//public class Network
//{
//    public string? Device { get; set; }
//    public string? Cidr { get; set; }
//    public string? Ip { get; set; }
//    public int? MBits { get; set; }
//    public ReservedPort?[]? ReservedPorts { get; set; }
//    public DynamicPort?[]? DynamicPorts { get; set; }
//}

//public class ReservedPort
//{
//    public string? Label { get; set; }
//    public int? Value { get; set; }
//}

//public class DynamicPort
//{
//    public string? Label { get; set; }
//    public int? Value { get; set; }
//}

//public class Env
//{
//    public string? Foo { get; set; }
//    public string? Baz { get; set; }
//}

//public class DispatchPayload
//{
//    public string? File { get; set; }
//}

//public class LogConfig
//{
//    public bool Disabled { get; set; }
//    public int? MaxFiles { get; set; }
//    public int? MaxFileSizeMb { get; set; }
//}

//public class Service
//{
//    public string? Name { get; set; }
//    public string? PortLabel { get; set; }
//    public string[]? Tags { get; set; }
//    public Check[]? Checks { get; set; }
//}

//public class Check
//{
//    public string? Name { get; set; }
//    public string? Type { get; set; }
//    public string? Command { get; set; }
//    public object? Args { get; set; }
//    public string? Path { get; set; }
//    public string? Protocol { get; set; }
//    public string? PortLabel { get; set; }
//    public long? Interval { get; set; }
//    public int? Timeout { get; set; }
//    public string? InitialStatus { get; set; }
//}

//public class Template
//{
//    public string? SourcePath { get; set; }
//    public string? DestPath { get; set; }
//    public string? EmbeddedTmpl { get; set; }
//    public string? ChangeMode { get; set; }
//    public string? ChangeSignal { get; set; }
//    public long? Splay { get; set; }
//    public string? Perms { get; set; }
//}

//public class Artifact
//{
//    public string? GetterSource { get; set; }
//    public GetterOptions? GetterOptions { get; set; }
//    public string? RelativeDest { get; set; }
//}

//public class GetterOptions
//{
//    public string? Checksum { get; set; }
//}
