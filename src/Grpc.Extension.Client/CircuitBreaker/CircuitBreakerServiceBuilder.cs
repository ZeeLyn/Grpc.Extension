using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grpc.Extension.Core;
using MagicOnion;
using Microsoft.Extensions.DependencyModel;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class CircuitBreakerServiceBuilder
	{
		private static readonly Dictionary<Type, Dictionary<string, List<Attribute>>> ServiceAttributes =
			new Dictionary<Type, Dictionary<string, List<Attribute>>>();
		public void InitializeService()
		{
			var ignoreAssemblyFix = new[]
				{"Microsoft", "System", "Grpc.Core", "Consul", "MagicOnion", "Polly", "Newtonsoft.Json", "MessagePack","Google.Protobuf","Remotion.Linq","SOS.NETCore","WindowsBase","mscorlib","netstandard","Grpc.Extension.Client"};
			var assemblies = DependencyContext.Default.RuntimeLibraries.SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default).Where(p => !ignoreAssemblyFix.Any(ignore => p.Name.StartsWith(ignore, StringComparison.CurrentCultureIgnoreCase))).Select(z => Assembly.Load(new AssemblyName(z.Name)))).Where(p => !p.IsDynamic).ToList();

			var types = assemblies.SelectMany(p => p.GetExportedTypes().Where(type => type.IsInterface && typeof(IServiceMarker).IsAssignableFrom(type))).ToList();
			foreach (var type in types)
			{
				ServiceAttributes[type] = new Dictionary<string, List<Attribute>>();
				foreach (var method in type.GetMethods().Where(p => p.IsPublic))
				{
					var attrs = method.GetCustomAttributes().Where(p =>
						p.GetType() == typeof(NonCircuitBreakerAttribute) ||
						p.GetType() == typeof(CircuitBreakerAttribute)).ToList();
					if (attrs.All(p => p.GetType() != typeof(NonCircuitBreakerAttribute)) &&
						attrs.All(p => p.GetType() != typeof(CircuitBreakerAttribute)))
					{
						throw new InvalidOperationException($"With circuit breaker enabled, service {type.FullName}.{method.Name} must add the CircuitBreakerAttribute custom attribute. If you need to disable the circuit breaker, you can add the NonCircuitBreakerAttribute custom attribute.");
					}

					ServiceAttributes[type][$"/{type.Name}/{method.Name}"] = attrs;
				}
			}
		}

		public TAttribute GetAttribute<TAttribute>(Type serviceType, string service) where TAttribute : Attribute
		{
			if (!ServiceAttributes.TryGetValue(serviceType, out var dic)) return null;
			if (!dic.TryGetValue(service, out var attributes)) return null;
			var attr = attributes.FirstOrDefault(p => p.GetType() == typeof(TAttribute));
			return attr as TAttribute;
		}
	}
}
