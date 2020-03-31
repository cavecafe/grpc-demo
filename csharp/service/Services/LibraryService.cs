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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace library_service
{
    public class LibraryService : Library.LibraryBase
    {
        private List<Book> books;

        private void init()
        {
            #region Book Data

            books = new List<Book>();
            Book book1 = new Book();
            book1.Code = "0132350882";
            book1.Title = "Clean Code";
            book1.Author = "Robert C. Martin";
            book1.Price = 52.35;
            book1.Shelf = "A1";
            book1.Category = Category.Computer;
            books.Add(book1);

            Book book2 = new Book();
            book2.Code = "1492034029";
            book2.Title = "BUILDING MICROSERVICES";
            book2.Author = "Sam Newman";
            book2.Price = 50.11;
            book2.Shelf = "A2";
            book2.Category = Category.Computer;
            books.Add(book2);

            Book book3 = new Book();
            book3.Code = "1680527827";
            book3.Title = "BABIES LOVE KITTENS";
            book3.Author = "Jessica Gibson";
            book3.Price = 10.99;
            book3.Shelf = "B1";
            book3.Category = Category.Comic;
            books.Add(book3);

            Book book4 = new Book();
            book4.Code = "0135350121";
            book4.Title = "Baby Flower Story";
            book4.Author = "Sam Wade";
            book4.Price = 19.23;
            book4.Shelf = "B1";
            book4.Category = Category.Essay;
            books.Add(book4);

            #endregion
        }

        private readonly ILogger<LibraryService> _logger;
        public LibraryService(ILogger<LibraryService> logger)
        {
            _logger = logger;
            init();
        }

        public override Task<Book> BookByCode(BookCode request, ServerCallContext context)
        {
            var query = from book in books
                            where book.Code == request.Code select book;
            return Task.FromResult(query.FirstOrDefault());
        }

        public override Task<BookList> BooksByTitle(BookTitle request, ServerCallContext context)
        {
            var query = from book in books
                        where book.Title.Contains(request.Title, StringComparison.CurrentCultureIgnoreCase)
                        select book;

            BookList list = new BookList();
            list.Books.AddRange(query);
            return Task.FromResult(list);
        }

        public override Task<BookList> BooksByAuthor(BookAuthor request, ServerCallContext context)
        {
            var query = from book in books
                        where book.Author.StartsWith(request.Author, StringComparison.CurrentCultureIgnoreCase)
                        select book;

            BookList list = new BookList();
            list.Books.AddRange(query);
            return Task.FromResult(list);
        }

    }
}
