using Grpc.Core;

namespace Grpc.Extension.Client
{
	public static class GrpcClientExtension
	{
		public static void Invok<TClient>(this TClient client) where TClient : ClientBase
		{

		}
	}
}
