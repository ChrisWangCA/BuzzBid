using System;
using System.Collections.Generic;
using System.Linq;
using BuzzBid.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;
using System.Security.Cryptography;

public class DBService
{
    private readonly BuzzBidContext _dbContext;
    private IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    public DBService(BuzzBidContext dbContext)
    {
        _dbContext = dbContext;
    }
    public DBService()
    {
        _dbContext = new BuzzBidContext();
    }

    public List<User> GetUsers()
    {
        return _dbContext.Users.ToList();
    }

    public DataSet ExecuteSql(string sql)
    {
        DataSet dataSet = new DataSet();
        using (SqlConnection conn = new SqlConnection(config["ConnectionStrings:BuzzBid"]))
        {
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                // Create a SqlDataAdapter
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    // Open connection
                    conn.Open();

                    // Fill the DataSet with the results of the query
                    adapter.Fill(dataSet);
                }
            }

        }
        return dataSet;
    }

    public int UpdateSql(string sql)
    {
        int rowsAffected = 0;
        using (SqlConnection conn = new SqlConnection(config["ConnectionStrings:BuzzBid"]))
        {
            conn.Open();
            
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                rowsAffected = command.ExecuteNonQuery();

                // Check if any rows were affected
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Data updated successfully. Rows affected: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine("No data updated.");
                }
            }

        }
        return rowsAffected;
    }
    public void ExecuteNonQuerySql(string sql, Dictionary<string, object> parameters)
    {
        using (SqlConnection conn = new SqlConnection(config["ConnectionStrings:BuzzBid"]))
        {
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                // Add parameters to the command to prevent SQL injection
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }

                // Open connection
                conn.Open();

                // Execute the command
                command.ExecuteNonQuery();
            }
        }
    }
    public string ComputeSha256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

}