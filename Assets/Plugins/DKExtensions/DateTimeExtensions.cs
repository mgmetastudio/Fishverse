using System;

public static partial class Extensions
{
	/// <summary>Returns dateTime in seconds</summary>
	public static long InSeconds(this DateTime dateTime)
	{
		return dateTime.Ticks / TimeSpan.TicksPerSecond;
	}

	/// <summary>Formated to 'days:hours:minets:seconds'</summary>
	/// <param name="spliter">Spliter between numbers</param>
	public static string FormatedDateTime(this TimeSpan span, string spliter = ":")
	{
		if (span.Days > 0)
		{
			return span.Days + spliter + span.Hours + spliter + span.Minutes.ToString("00") + spliter + span.Seconds.ToString("00");
		}
		else
		{
			if (span.Hours > 0)
				return span.Hours + spliter + span.Minutes.ToString("00") + spliter + span.Seconds.ToString("00");
			else
				return span.Minutes + spliter + span.Seconds.ToString("00");
		}
	}
}
