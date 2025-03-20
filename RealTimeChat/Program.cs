using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat
{
    class Program
    {
        public static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            Console.Write("What's your name? ");

            var name = Console.ReadLine();

            Console.WriteLine("*******************************************************");

            var client = new FirebaseClient("https://erts-2025-default-rtdb.europe-west1.firebasedatabase.app/");
            var child = client.Child("messages");

            var observable = child.AsObservable<Message>();


            Console.WriteLine("Start chatting");

            // subscribe to messages comming in, ignoring the ones that are from me
            var subscription = observable
                .Where(f => !string.IsNullOrEmpty(f.Key)) // you get empty Key when there are no data on the server for specified node
                .Where(f => f.Object?.Author != name)
                .Subscribe(f => Console.WriteLine($"{f.Object.Author}: {f.Object.Content}"));

            while (true)
            {
                var message = Console.ReadLine();

                if (message?.ToLower() == "q")
                {
                    break;
                }

                await child.PostAsync(new Message { Author = name, Content = message });
            }

            subscription.Dispose();
        }
    }
}
