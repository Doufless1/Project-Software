﻿namespace Project_Software_GUI
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Microsoft.Data.SqlClient;

    public class DataFetcher
    {
        private string connection_string = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public List<string>
            FetchData(string query) //Query is a SQL query that returns a single column of strings which will be returned as a list of strings
        {
            List<string> result = new List<string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0));
                        }
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