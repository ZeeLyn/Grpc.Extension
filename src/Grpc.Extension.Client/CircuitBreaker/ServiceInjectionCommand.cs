using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Extension.Core;
using MessagePack;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Grpc.Extension.Client.CircuitBreaker
{
	internal class ServiceInjectionCommand : IServiceInjectionCommand
	{
		private CircuitBreakerServiceBuilder CircuitBreakerServiceBuilder { get; }

		private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, InjectionCommand>> _serviceCommand =
			new ConcurrentDictionary<Type, ConcurrentDictionary<string, InjectionCommand>>();
		public ServiceInjectionCommand(CircuitBreakerServiceBuilder circuitBreakerServiceBuilder)
		{
			CircuitBreakerServiceBuilder = circuitBreakerServiceBuilder;
		}

		private readonly ConcurrentDictionary<string, object> scripts = new ConcurrentDictionary<string, object>();
		public InjectionCommand GetCommand(Type serviceType, string serviceName)
		{
			var attr = CircuitBreakerServiceBuilder.GetAttribute<CircuitBreakerAttribute>(serviceType, serviceName);
			if (attr == null)
				throw new InvalidOperationException();
			var typeCommands = _serviceCommand.GetOrAdd(serviceType, new ConcurrentDictionary<string, InjectionCommand>());
			return typeCommands.GetOrAdd(serviceName, new InjectionCommand { Command = attr.FallbackInjection, Namespace = attr.InjectionNamespace });
		}

		public async Task<object> Run(string command, params string[] injectionNamespaces)
		{
			if (!scripts.ContainsKey(command))
			{
				var scriptOptions = ScriptOptions.Default.WithImports("System.Threading.Tasks");
				if (injectionNamespaces != null && injectionNamespaces.Length > 0)
				{
					scriptOptions = scriptOptions.WithReferences(injectionNamespaces);
				}
				return scripts.GetOrAdd(command, LZ4MessagePackSerializer.Serialize(await CSharpScript.EvaluateAsync(command, scriptOptions)));
			}
			scripts.TryGetValue(command, out var result);
			return result;

		}
	}
}
