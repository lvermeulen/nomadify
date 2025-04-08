using System;

namespace Nomad.Abstractions;

[AttributeUsage(AttributeTargets.Property)]
public class RestorableStatePropertyAttribute : Attribute;