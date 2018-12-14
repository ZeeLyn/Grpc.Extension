using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extension.Client
{
	public class AsyncLock
	{
		private readonly SemaphoreSlim _semaphoreSlim;

		public AsyncLock()
		{
			_semaphoreSlim = new SemaphoreSlim(0, 1);
		}

		public async Task<Release> GetLockAsync(int expire)
		{
			await _semaphoreSlim.WaitAsync(expire);
			return new Release(_semaphoreSlim);
		}

		public void ReleaseLock()
		{
			_semaphoreSlim.Release(1);
		}
	}

	public struct Release : IDisposable
	{
		private readonly SemaphoreSlim _semaphoreSlim;

		public Release(SemaphoreSlim semaphoreSlim)
		{
			_semaphoreSlim = semaphoreSlim;
		}

		public void Dispose()
		{
			_semaphoreSlim.Release();
		}
	}
}
