﻿namespace Project_Software_API.Properties.Backend.Models;

public enum FrontendReadyData
{
    HourlyDayInsideTemperatureAverage,
    DailyWeekInsideTemperatureAverage,
    DailyMonthInsideTemperatureAverage,
    
    HourlyDayOutsideTemperatureAverage,
    DailyWeekOutsideTemperatureAverage,
    DailyMonthOutsideTemperatureAverage,
    
    HourlyDayHumidityAverage,
    DailyWeekHumidityAverage,
    DailyMonthHumidityAverage,
    
    HourlyDayLightAverage,
    DailyWeekLightAverage,
    DailyMonthLightAverage,
    
    HourlyDayPressureAverage,
    DailyWeekPressureAverage,
    DailyMonthPressureAverage,
    
    CurrentInsideTemperature,
    CurrentOutsideTemperature,
    CurrentHumidity,
    CurrentLight,
    CurrentPressure,
    
    BatteryVoltage,
    BatteryPercentage,
    ModelId
}

