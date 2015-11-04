using System;
using System.IO;

namespace UnityProxy
{
	/// <summary>
	/// Watches the Unity log file and redirects it to standard output.
	/// </summary>
	public class Watcher
	{
		/// <summary>
		/// Magic string for detecting progress bar messages.
		/// </summary>
		private const string ProgressBarMarker = "DisplayProgressbar: ";

		/// <summary>
		/// Size of the log when it was previously read.
		/// </summary>
		private static long previousLogSize = 0;

		/// <summary>
		/// Indicates if this thread should stop.
		/// </summary>
		private volatile bool shouldStop = false;

		/// <summary>
		/// Full log text.
		/// </summary>
		private volatile string fullLog = "";

		/// <summary>
		/// Path to the log file.
		/// </summary>
		private string logPath;


		/// <summary>
		/// Gets the full log text.
		/// </summary>
		public string FullLog
		{
			get
			{
				return fullLog;
			}
		}

		/// <summary>
		/// Creates a new Watcher that will read the log from the specified path.
		/// </summary>
		/// <param name="logPath"></param>
		public Watcher(string logPath)
		{
			this.logPath = logPath;
		}

		public void Run()
		{
			while (true)
			{
				if (File.Exists(logPath))
				{
					using (FileStream stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						stream.Position = previousLogSize;
						previousLogSize = stream.Length;

						using (StreamReader reader = new StreamReader(stream))
						{
							string newText = reader.ReadToEnd();
							LogProgressMessages(newText);
							fullLog += newText;
							Console.Write(newText);
						}
					}
				}

				if (shouldStop) break;
				
				System.Threading.Thread.Sleep(1000);
			}
		}

		/// <summary>
		/// Searches for progress bar messages and forwards them to TeamCity.
		/// </summary>
		/// <param name="text"></param>
		private void LogProgressMessages(string text)
		{
			string[] lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].StartsWith(ProgressBarMarker))
				{
					string progressName = lines[i].Substring(ProgressBarMarker.Length);
					Console.WriteLine();
					Console.WriteLine("##teamcity[progressMessage '" + progressName + "']");
				}
			}
		}

		//Stops the watcher.
		public void Stop()
		{
			shouldStop = true;
		}
	}
}
