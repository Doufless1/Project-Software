﻿    using System.Data;
    using Microsoft.IdentityModel.Tokens;

    namespace sql_fetcher
    {
        using System;
        using System.Collections.Generic;
        using Microsoft.Data.SqlClient;

        public class DataFetcher
        {
            private string connection_string;

            public DataFetcher(string connectionString)
            {
                connection_string = connectionString;
            }
            
            public List<double>
                FetchData(string query) //Query is a SQL query that returns a single column of strings which will be returned as a list of strings
            {
                List<double> result = new List<double>();
                try
                {
                    using (SqlConnection connection = new SqlConnection(connection_string))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            while (reader.Read())
                            {
                                result.Add(reader.GetDouble(0));
                            }

                            if (result.IsNullOrEmpty()) throw new Exception("No data could be fetched for: " + query);
                        }
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured in DataFetcher: ${e.Message}");
                }

                return result;
            }
        }
    }