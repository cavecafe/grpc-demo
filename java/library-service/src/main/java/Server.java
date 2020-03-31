import book.LibraryGrpc;
import io.grpc.ServerBuilder;
import io.grpc.stub.StreamObserver;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import book.LibraryOuterClass.*;

public class Server {

    private static List<Book> books = init();

    private static List<Book> init() {

        List<Book> books = new ArrayList<>();

        Book.Builder b = Book.getDefaultInstance().newBuilderForType();

        b.setCode("0132350882");
        b.setTitle("Clean Code");
        b.setAuthor("Robert C. Martin");
        b.setPrice(52.35);
        b.setShelf("A1");
        b.setCategory(Category.Computer);
        books.add(b.build());

        b.setCode("1492034029");
        b.setTitle("BUILDING MICROSERVICES");
        b.setAuthor("Sam Newman");
        b.setPrice(50.11);
        b.setShelf("A2");
        b.setCategory(Category.Computer);
        books.add(b.build());

        b.setCode("1680527827");
        b.setTitle("BABIES LOVE KITTENS");
        b.setAuthor("Jessica Gibson");
        b.setPrice(10.99);
        b.setShelf("B1");
        b.setCategory(Category.Comic);
        books.add(b.build());

        b.setCode("0135350121");
        b.setTitle("Baby Flower Story");
        b.setAuthor("Sam Wade");
        b.setPrice(19.23);
        b.setShelf("B1");
        b.setCategory(Category.Essay);
        books.add(b.build());

        return books;
    }


    public static void main(String[] args) throws IOException, InterruptedException {

        final int PORT = 53000;

        io.grpc.Server service = ServerBuilder.forPort(PORT)
                .addService(new Library())
                .build()
                .start();

        Runtime.getRuntime().addShutdownHook(new Thread(service::shutdownNow));
        System.out.println("Started listening for rpc calls on " + PORT + "...");
        service.awaitTermination();

    }


    static class Library extends LibraryGrpc.LibraryImplBase {

        @Override
        public void bookByCode(BookCode request, StreamObserver<Book> responseObserver) {
            System.out.println("REQUEST:\n\t" + request.toString());

            for (Book book : books) {
                if (book.getCode().equalsIgnoreCase(request.getCode())) {
                    responseObserver.onNext(book);
                    break;
                }
            }
            responseObserver.onCompleted();
        }

        @Override
        public void booksByTitle(BookTitle request, StreamObserver<BookList> responseObserver) {
            System.out.println("REQUEST:\n\t" + request.toString());

            BookList.Builder builder = BookList.newBuilder();
            for (Book book : books) {
                if (book.getTitle().toUpperCase().contains(request.getTitle().toUpperCase())) {
                    builder.addBooks(book);
                }
            }
            responseObserver.onNext(builder.build());
            responseObserver.onCompleted();
        }

        @Override
        public void booksByAuthor(BookAuthor request, StreamObserver<BookList> responseObserver) {
            System.out.println("REQUEST:\n\t" + request.toString());

            BookList.Builder builder = BookList.newBuilder();
            for (Book book : books) {
                if (book.getAuthor().toUpperCase().contains(request.getAuthor().toUpperCase())) {
                    builder.addBooks(book);
                }
            }
            responseObserver.onNext(builder.build());
            responseObserver.onCompleted();
        }

    }
}
