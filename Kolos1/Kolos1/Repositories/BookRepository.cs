using Kolos1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace Kolos1.Repositories;

public class BookRepository
{
    private readonly IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<BookDTO> getBook(int id)
    {
        var query =
            "SELECT b.pk as bookID, b.title as bookTitle, a.first_name as firstName, a.last_name as lastName FROM books b " +
            "join books_authors ba on b.PK = ba.FK_book " +
            "join authors a on a.PK = ba.FK_author " +
            "where b.pk = @id";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        
        await reader.ReadAsync();

        if (!reader.HasRows) throw new Exception();

        var bookIDOrdinal = reader.GetOrdinal("bookID");
        var bookTitleOrdinal = reader.GetOrdinal("bookTitle");
        var authorFirstNameOrdinal = reader.GetOrdinal("firstName");
        var authorlastNameOrdinal = reader.GetOrdinal("lastName");

        BookDTO bookDto= null;

        while (await reader.ReadAsync())
        {
            if (bookDto is not null)
            {
                bookDto.authors.Add(new AuthorDTO()
                {
                    firstName = reader.GetString(authorFirstNameOrdinal),
                    lastName = reader.GetString(authorlastNameOrdinal)
                });
            }
            else
            {
                bookDto = new BookDTO()
                {
                    id = reader.GetInt32(bookIDOrdinal),
                    title = reader.GetString(bookTitleOrdinal),
                    authors = new List<AuthorDTO>()
                    {
                        new AuthorDTO()
                        {
                            firstName = reader.GetString(authorFirstNameOrdinal),
                            lastName = reader.GetString(authorlastNameOrdinal)
                        }
                    }
                };
            }
        }

        return bookDto;
    }

    public async Task addBook(NewBookDTO newBookDto)
    {

        var query = "Insert into books values (@title); SELECT @@identity as bookID; ";
                    
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@title", newBookDto.title);
        
        await connection.OpenAsync();

        //var id1 = bookID;
        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            var id = await command.ExecuteScalarAsync();
            foreach (var author in newBookDto.authors)
            {
                command.Parameters.Clear();
                command.CommandText = "INSERT into authors values (@firstName, @lastName);  SELECT @@identity as AuthorID";
                    command.Parameters.AddWithValue("@firstName", author.firstName);
                command.Parameters.AddWithValue("@lastName", author.lastName );
                
                await command.ExecuteNonQueryAsync();
                
                command.Parameters.Clear();
                command.CommandText = "INSERT into books_authors values(@bookID1, @authorID1)";
                //command.Parameters.AddWithValue("@bookID1", id1 );
                //command.Parameters.AddWithValue("@authorID1", AuthorID );
                
                
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch(Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    
    public async Task<bool> doesBookExists(int id)
    {
        var query = "Select 1 from books where pk = @id";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        
        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }
}