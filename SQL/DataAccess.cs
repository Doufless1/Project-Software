﻿using System;
using System.Collections.Generic;
using enums;

namespace sql_fetcher
{
    public class DataAccess
    {
        private static readonly string ConnectionString =
            "Server=tcp:group13.database.windows.net,1433;" +
            "Database=weather_state;" +
            "User ID=cloudadmin;" +
            "Password=Group13pass;" +
            "Encrypt=True;" +
            "TrustServerCertificate=False;" +
            "Connection Timeout=30;";

        private static readonly DataFetcher DataFetcher = new DataFetcher(ConnectionString);
        private static readonly DataStorage DataStorage = new DataStorage(DataFetcher);
        private static readonly GatewayDataStorage GatewayDataStorage = new GatewayDataStorage(DataFetcher);

        public DataAccess()
        {
            try
            {
                // Initialize queries in DataStorage.cs with the queries to be fetched
                foreach (string location in Devices.GetDevices())
                {
                    try
                    {
                        GatewayDataStorage.Add(AccesableData.longitude,  location,
                            $"SELECT longitude FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.latitude,  location,
                            $"SELECT latitude FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.altitude,  location,
                            $"SELECT altitude FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.avgRssi,  location,
                            $"SELECT avg_rssi FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.avgSnr,  location,
                            $"SELECT avg_snr FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.maxRssi,  location,
                            $"SELECT max_rssi FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.MinRssi,  location,
                            $"SELECT min_rssi FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.MaxSnr,  location,
                            $"SELECT max_snr FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");
                        GatewayDataStorage.Add(AccesableData.MinSnr,  location,
                            $"SELECT min_snr FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND gatewayID = 'gateway_input'");


                        
                        // Current datapoints
                        DataStorage.Add(AccesableData.CurrentTemperature, 0, location,
                            $"SELECT TOP 1 temperature FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
                        DataStorage.Add(AccesableData.CurrentHumidity, 0, location,
                            $"SELECT TOP 1 humidity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
                        DataStorage.Add(AccesableData.CurrentLight, 0, location,
                            $"SELECT TOP 1 luminosity FROM weather WHERE deviceID LIKE '%{location.ToString().ToLower()}' ORDER BY weather.date DESC");
                        DataStorage.Add(AccesableData.BatteryPercentage, 0, location,
                            $@"SELECT d.battery_percentage
                       FROM dbo.device d
                       where d.deviceID LIKE '%{location.ToString().ToLower()}'");
                        DataStorage.Add(AccesableData.SignalToNoiseRatio, 0, location,
                            $"SELECT TOP 1 avg_rssi FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND avg_rssi IS NOT NULL ORDER BY deviceID ASC");

                        // Past datapoints
                        for (int i = 1; i < 31; i++)
                        {
                            try
                            {
                                // Generate date range for the query
                                string dateRangeQuery = $"BETWEEN DATEADD(DAY, -{i}, GETDATE()) AND DATEADD(DAY, -{i - 1}, GETDATE())";

                                DataStorage.Add(AccesableData.DayTemperature, i, location,
                                    $"SELECT temperature FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.DayHumidity, i, location,
                                    $"SELECT humidity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.DayLight, i, location,
                                    $"SELECT luminosity FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.DayPressure, i, location,
                                    $"SELECT pressure FROM weather WHERE CONVERT(date, date) {dateRangeQuery} AND deviceID LIKE '%{location.ToString().ToLower()}'");
                                DataStorage.Add(AccesableData.SignalToNoiseRatio, 0, location,
                                    $"SELECT TOP 1 avg_rssi FROM gateway WHERE deviceID LIKE '%{location.ToString().ToLower()}' AND avg_rssi IS NOT NULL ORDER BY deviceID ASC");

                            }
                            catch (Exception ex)
                            {
                                LogException($"Error while adding past data for day {i} and location {location}", ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogException($"Error while adding current data for location {location}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("Error in DataAccess constructor", ex);
                throw new InvalidOperationException("Failed to initialize data access.", ex);
            }
        }

        public List<double> GetData(AccesableData name, int dayFromNow, string location)
        {
            try
            {
                for (int i = 0; i < DataStorage.Name.Count; i++)
                {
                    if (DataStorage.Name[i] == name && DataStorage.Location[i] == location &&
                        DataStorage.DayFromNow[i] == dayFromNow)
                    {
                        return DataStorage.Data[i];
                    }
                }
            }
            catch (Exception ex)
            {
                LogException($"Error while fetching data for {name}, dayFromNow: {dayFromNow}, location: {location}", ex);
            }

            // Return an empty list if no data found or an error occurred
            return new List<double>();
        }

        private void LogException(string context, Exception ex)
        {
            // Log exception with context
            Console.WriteLine($"{context}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
