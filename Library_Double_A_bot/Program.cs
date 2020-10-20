using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Library_Double_A_bot
{
    class Program
    {
        private static string libraryPath = @"H:\Others\Telegram_bot\Library_Double_A_bot\Library";
        private static string[] allfiles = Directory.GetFiles(libraryPath);
        private static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("1301992385:AAECih04QPAq-yJeYiGyAgvo0v12xk2QusE") {Timeout = TimeSpan.FromSeconds(10)};

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Bot id: {me.Id}. Bot name: {me.FirstName}");
            

            botClient.OnMessage += Bot_onMessage;
            botClient.StartReceiving();


            Console.ReadKey();
        }

        private static async void Bot_onMessage(object sender, MessageEventArgs e)
        {
            var text = e?.Message?.Text;
            int n = 1;
            if (text == null)
                return;

            if(text == @"\getlist")
            {
                foreach (string filename in allfiles)
                {

                    await botClient.SendTextMessageAsync(
                         chatId: e.Message.Chat,
                         text: $"{n}.{filename.Remove(0, libraryPath.Length + 1)}"
                    ).ConfigureAwait(false);
                    n++;
                }
            }

            if(text == @"\download-1")
            {
                 try
                 {                
                    string path = allfiles[Convert.ToInt32(text.Remove(0, text.LastIndexOf('-')+1))-1];
                    Console.WriteLine(path);
                    await botClient.SendTextMessageAsync(e.Message.Chat, "Хороший выбор! Пожалуйтса подождите, пока ваша книга загрузится.");
                    using (var sendFileStream = File.Open(path, FileMode.Open))
                    {
                        await botClient.SendDocumentAsync(e.Message.Chat, new Telegram.Bot.Types.InputFiles.InputOnlineFile(sendFileStream));
                        await botClient.SendTextMessageAsync(e.Message.Chat, "Ваша книга загружена, приятного чтения!");
                    }
                                                      
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error downloading: " + ex.Message);
                }
                
            }
           
        }

     /*   private static async void DownloadFile(object fileId, string path)
        {
            try
            {
                path = allfiles[Convert.ToInt32(path.Remove(0, path.LastIndexOf('-')))];
                var file = await botClient.GetFileAsync(path);

                using (var saveImageStream = new FileStream(path, FileMode.Create))
                {
                    await file.FileStream.CopyToAsync(saveImageStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error downloading: " + ex.Message);
            }
        }*/
    }
}
