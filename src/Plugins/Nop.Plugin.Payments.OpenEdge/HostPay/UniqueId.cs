using System;

namespace Nop.Plugin.Payments.OpenEdge.HostPay;

public class UniqueId
{
	public static string GenerateId()
	{
		long num = 1L;
		byte[] array = Guid.NewGuid().ToByteArray();
		foreach (byte b in array)
		{
			num *= b + 1;
		}
		return $"{num - DateTime.UtcNow.Ticks:x}";
	}
}
