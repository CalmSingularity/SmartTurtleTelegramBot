using System;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.SmallBasic.Library;
using Newtonsoft.Json.Linq;

namespace TelegramBot
{
	class Program
	{
		static void Main(string[] args)
		{
			WebClient webClient = new WebClient(); // allows to retrieve data from internet

			Console.WriteLine("You can create a Telegram bot and get a token by talking to @BotFather.");
			Console.Write("Now enter your bot token: ");
			string token = Console.ReadLine();
			string startUrl = $"https://api.telegram.org/bot{token}";

			int updateID = 0;
			string messageFromID, messageText, messageFromName;

			while (true)
			{
				string url = $"{startUrl}/getUpdates?offset={updateID + 1}";
				string response = webClient.DownloadString(url);
				var array = JObject.Parse(response)["result"].ToArray();

				foreach (var msg in array)
				{
					updateID = Convert.ToInt32(msg["update_id"]);
					try
					{
						messageFromID = msg["message"]["from"]["id"].ToString();
						messageFromName = msg["message"]["from"]["first_name"].ToString() + " " + msg["message"]["from"]["last_name"].ToString();
						messageText = msg["message"]["text"].ToString();
						Console.WriteLine($"Received update {updateID} from {messageFromID} {messageFromName}: \"{messageText}\"\n");

						var request = messageText.Split(' ');

						switch (request[0].ToLower())
						{
							case "m":
							case "м":
								int distance = Convert.ToInt32(request[1]);
								if (distance >= -100 && distance <= 100)
								{
									Turtle.Move(distance);
									messageText = $"Черепашка прошла {distance} пикселей.";
								}
								else
								{
									messageText += " - ошибочная команда.\nДля справки отправьте /start";
								}
								break;
							case "t":
							case "т":
								int angle = Convert.ToInt32(request[1]);
								if (angle >= -360 && angle <= 360)
								{
									Turtle.Turn(angle);
									messageText = $"Черепашка повернулась на {angle} градусов.";
								}
								else
								{
									messageText += " - ошибочная команда.\nДля справки отправьте /start";
								}
								break;
							case "/start":
								messageText = "Допустимые команды:\n";
								messageText += "M 50 - двигаться вперед на 50 пикселей\n";
								messageText += "M -20 - двигаться назад на 20 пикселей\n";
								messageText += "(количество пикселей от -100 до 100)\n";
								messageText += "T 90 - повернуться по часовой стрелке на 90 градусов\n";
								messageText += "T -45 - повернуться против часовой стрелки на 45 градусов\n";
								messageText += "(количество градусов от -360 до 360)";
								break;
							default:
								messageText += " - недопустимая команда.\nДля справки отправьте /start";
								break;
						}

						url = $"{startUrl}/sendMessage?chat_id={messageFromID}&text={messageText}";
						webClient.DownloadString(url);
						Console.WriteLine($"Sent to {messageFromID} {messageFromName}: \"{messageText}\"\n");
					}
					catch { }

				}

				Thread.Sleep(500);  // check new updates twice per second
			}

		}
	}
}
