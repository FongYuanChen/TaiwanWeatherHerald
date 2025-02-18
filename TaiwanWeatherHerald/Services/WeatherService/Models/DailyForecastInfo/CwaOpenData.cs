using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TaiwanWeatherHerald.Services.WeatherService.Models.DailyForecastInfo
{
    [XmlRoot(ElementName = "cwaopendata", Namespace = "urn:cwa:gov:tw:cwacommon:0.1")]
    public class CwaOpenData
    {
        [XmlElement(ElementName = "Identifier")]
        public string Identifier { get; set; }

        [XmlElement(ElementName = "Sender")]
        public string Sender { get; set; }

        [XmlElement(ElementName = "Sent")]
        public DateTime Sent { get; set; }

        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "Scope")]
        public string Scope { get; set; }

        [XmlElement(ElementName = "MsgType")]
        public string MsgType { get; set; }

        [XmlElement(ElementName = "Dataid")]
        public string Dataid { get; set; }

        [XmlElement(ElementName = "Source")]
        public string Source { get; set; }

        [XmlElement(ElementName = "Dataset")]
        public Dataset Dataset { get; set; }
    }

    public class Dataset
    {
        [XmlElement(ElementName = "DatasetInfo")]
        public DatasetInfo DatasetInfo { get; set; }

        [XmlArray(ElementName = "Locations")]
        [XmlArrayItem(ElementName = "Location")]
        public List<Location> Locations { get; set; }
    }

    public class DatasetInfo
    {
        [XmlElement(ElementName = "DatasetDescription")]
        public string DatasetDescription { get; set; }

        [XmlElement(ElementName = "DatasetLanguage")]
        public string DatasetLanguage { get; set; }

        [XmlElement(ElementName = "IssueTime")]
        public DateTime IssueTime { get; set; }

        [XmlElement(ElementName = "ValidTime")]
        public ValidTime ValidTime { get; set; }

        [XmlElement(ElementName = "Update")]
        public DateTime Update { get; set; }
    }

    public class ValidTime
    {
        [XmlElement(ElementName = "StartTime")]
        public DateTime StartTime { get; set; }

        [XmlElement(ElementName = "EndTime")]
        public DateTime EndTime { get; set; }
    }

    public class Location
    {
        [XmlElement(ElementName = "LocationName")]
        public string LocationName { get; set; }

        [XmlElement(ElementName = "Geocode")]
        public string GeoCode { get; set; }

        [XmlElement(ElementName = "Latitude")]
        public double Latitude { get; set; }

        [XmlElement(ElementName = "Longitude")]
        public double Longitude { get; set; }

        [XmlElement(ElementName = "ParameterSet")]
        public ParameterSet ParameterSet { get; set; }

        [XmlElement(ElementName = "WeatherElement")]
        public List<WeatherElement> WeatherElements { get; set; }
    }

    public class ParameterSet
    {
        [XmlElement(ElementName = "Parameter")]
        public List<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {
        [XmlElement(ElementName = "ParameterName")]
        public string ParameterName { get; set; }

        [XmlElement(ElementName = "ParameterValue")]
        public string ParameterValue { get; set; }
    }

    public class WeatherElement
    {
        [XmlElement(ElementName = "ElementName")]
        public string ElementName { get; set; }

        [XmlElement(ElementName = "Time")]
        public List<Time> Times { get; set; }
    }

    public class Time
    {
        [XmlElement(ElementName = "StartTime")]
        public DateTime StartTime { get; set; }

        [XmlElement(ElementName = "EndTime")]
        public DateTime EndTime { get; set; }

        [XmlElement(ElementName = "ElementValue")]
        public ElementValue ElementValue { get; set; }
    }

    public class ElementValue
    {
        [XmlElement(ElementName = "Temperature")]
        public int? Temperature { get; set; }

        [XmlElement(ElementName = "MaxTemperature")]
        public int? MaxTemperature { get; set; }

        [XmlElement(ElementName = "MinTemperature")]
        public int? MinTemperature { get; set; }

        [XmlElement(ElementName = "DewPoint")]
        public int? DewPoint { get; set; }

        [XmlElement(ElementName = "RelativeHumidity")]
        public int? RelativeHumidity { get; set; }

        [XmlElement(ElementName = "MaxApparentTemperature")]
        public int? MaxApparentTemperature { get; set; }

        [XmlElement(ElementName = "MinApparentTemperature")]
        public int? MinApparentTemperature { get; set; }

        [XmlElement(ElementName = "MaxComfortIndex")]
        public int? MaxComfortIndex { get; set; }

        [XmlElement(ElementName = "MaxComfortIndexDescription")]
        public string MaxComfortIndexDescription { get; set; }

        [XmlElement(ElementName = "MinComfortIndex")]
        public int? MinComfortIndex { get; set; }

        [XmlElement(ElementName = "MinComfortIndexDescription")]
        public string MinComfortIndexDescription { get; set; }

        [XmlElement(ElementName = "WindSpeed")]
        public string WindSpeed { get; set; }

        [XmlElement(ElementName = "BeaufortScale")]
        public string BeaufortScale { get; set; }

        [XmlElement(ElementName = "WindDirection")]
        public string WindDirection { get; set; }

        [XmlElement(ElementName = "ProbabilityOfPrecipitation")]
        public string ProbabilityOfPrecipitation { get; set; }

        [XmlElement(ElementName = "Weather")]
        public string Weather { get; set; }

        [XmlElement(ElementName = "WeatherCode")]
        public int? WeatherCode { get; set; }

        [XmlElement(ElementName = "UVIndex")]
        public int? UVIndex { get; set; }

        [XmlElement(ElementName = "UVExposureLevel")]
        public string UVExposureLevel { get; set; }

        [XmlElement(ElementName = "WeatherDescription")]
        public string WeatherDescription { get; set; }
    }
}
