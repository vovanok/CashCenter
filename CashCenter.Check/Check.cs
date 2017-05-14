using CashCenter.Check.DescriptorModel;
using CashCenter.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CashCenter.Check
{
	public class Check
	{
        public CheckDescriptor Descriptor { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }
        public decimal Cost { get; private set; }
        public string Email { get; private set; }

        public Check(string descriptorId, Dictionary<string, string> parameters, decimal cost, string email)
        {
            Descriptor = GetCheckDescriptorById(descriptorId);
            Parameters = parameters;
            Cost = cost;
            Email = email;
        }

        public IEnumerable<string> GetCommonLines()
        {
            if (Descriptor == null)
                return new List<string>();

            var commonLines = new List<string>();
            foreach (var line in Descriptor.Lines)
            {
                commonLines.AddRange(GetStringByLineDescriptor(line, Parameters, Config.CheckPrinterMaxLineLength));
            }

            return commonLines;
        }

        private IEnumerable<string> GetStringByLineDescriptor(CheckLineDescriptor line, Dictionary<string, string> parameters, int lineLength)
        {
            var lineContent = line.Content;
            foreach (var parameter in parameters)
            {
                lineContent = TryPlaceParameter(lineContent, parameter.Key, parameter.Value);
            }

           switch (line.ContentAlign)
            {
                case CheckLineDescriptor.Align.Left:
                    return StringUtils.SplitStringByLines(lineContent, lineLength);
                case CheckLineDescriptor.Align.Center:
                    return StringUtils.StringInCenter(lineContent, lineLength);
                case CheckLineDescriptor.Align.Repeated:
                    return new[] { StringUtils.FilledString(lineContent, lineLength) };
                default:
                    return new List<string>();
            }
        }

        private string TryPlaceParameter(string srcValue, string parameterName, string parameterValue)
        {
            return srcValue.Replace($"{{{parameterName}}}", parameterValue);
        }

        #region Descriptors

        private static ChecksSetDescriptor checksSetDescriptor;

        static Check()
        {
            checksSetDescriptor = LoadChecksSetDesctiptors(Config.ChecksFilename);
            if (checksSetDescriptor == null)
                throw new Exception("XML с описанием чеков не загружен");
        }

        private static CheckDescriptor GetCheckDescriptorById(string checkDescriptorId)
        {
            if (checksSetDescriptor == null)
                return null;

            return checksSetDescriptor.Checks.FirstOrDefault(checkDesc => checkDesc.Id == checkDescriptorId);
        }

        private static ChecksSetDescriptor LoadChecksSetDesctiptors(string filename)
        {
            var serializer = new XmlSerializer(typeof(ChecksSetDescriptor));
            using (StreamReader reader = new StreamReader(filename))
            {
                return (ChecksSetDescriptor)serializer.Deserialize(reader);
            }
        }

        #endregion
    }
}
