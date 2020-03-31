/*
MIT License

Copyright (c) 2020 Thomas Kim

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Grpc.Net.Client;
using library_service;

namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // string address = "https://localhost:5001"; 
            string address = "http://192.168.0.40:53000"; // TODO set your IP address

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            bool macOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            if (macOS)
            {
                // This switch must be set before creating the GrpcChannel/HttpClient.
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                address = "http://localhost:5000";
            }
            using var channel = GrpcChannel.ForAddress(address);

            var library = new Library.LibraryClient(channel);

            bool quit = false;

            Console.Write("Book search");
            while (!quit)
            {
                Console.Write("\n\nSearch options:\n\n 1: by Code\n 2: by Title\n 3: by Author\n X: to exit\n\n");
                var selection = Console.ReadKey(true);

                try {
                    switch (selection.KeyChar)
                    {
                        case '1':
                            {
                                Console.Write("\nCode: ");
                                var code = Console.ReadLine();
                                var book = await library.BookByCodeAsync(new BookCode { Code = code });
                                Console.WriteLine(" -> Found your book\n" + JsonHelper.FormatJson(book.ToString()));
                            }
                            break;

                        case '2':
                            {
                                Console.Write("\nTitle: ");
                                var title = Console.ReadLine();
                                var books = await library.BooksByTitleAsync(new BookTitle { Title = title });
                                Console.WriteLine(" -> " + books.Books.Count + " book(s) found.\n" + JsonHelper.FormatJson(books.ToString()));
                            }
                            break;

                        case '3':
                            {
                                Console.Write("\nAuthor: ");
                                var author = Console.ReadLine();
                                var books = await library.BooksByAuthorAsync(new BookAuthor { Author = author });
                                Console.WriteLine(" -> " + books.Books.Count + " book(s) found.\n" + JsonHelper.FormatJson(books.ToString()));
                            }
                            break;

                        case 'x':
                        case 'X':
                            Console.WriteLine("\n -> Bye for now.");
                            quit = true;
                            break;
                        default:
                            Console.WriteLine("\n -> Invalid selection");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
             
        }
    }
}