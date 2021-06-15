using AbfSharp.ABFFIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AbfSharp
{
    public class AbfHeader
    {
        // info that does not change as sweeps/channels change

        public readonly string filePath;
        public readonly string fileName;
        public readonly string id;
        public readonly long fileSize;
        public readonly double fileSizeMb;
        //public readonly string[] adcNames;
        //public readonly string[] adcUnits;
        //public readonly string[] dacNames;
        //public readonly string[] dacUnits;

        // data
        public readonly int channelCount;
        public readonly int sweepCount;
        public readonly int tagCount;
        public readonly Tag[] tags;

        public readonly int sampleRate;
        public readonly double pointsPerMs;

        public readonly double totalLengthMin;
        public readonly double totalLengthSec;
        public readonly int totalLengthPoints;

        public readonly double sweepLengthMin;
        public readonly double sweepLengthSec;
        public readonly int sweepLengthPoints;

        public readonly double sweepIntervalSec;
        public readonly double sweepIntervalMin;

        // details from the header
        public readonly DateTime StartDateTime;
        //public readonly AbfVersion version;
        //public readonly string comment;
        //public readonly string creator;
        //public readonly string creatorVersion;
        public readonly string protocolFilePath;
        public readonly string protocol;

        // TODO: make this private or encapsulate it in a method that thows a warning?
        public readonly Structs.ABFFileHeader HeaderStruct;

        public AbfHeader(string abfFilePath, ABFFIO.AbfInterface abffio)
        {
            HeaderStruct = abffio.header;

            filePath = System.IO.Path.GetFullPath(abfFilePath);
            fileName = System.IO.Path.GetFileName(abfFilePath);
            id = System.IO.Path.GetFileNameWithoutExtension(abfFilePath);
            fileSize = new System.IO.FileInfo(abfFilePath).Length;
            fileSizeMb = (double)fileSize / 1e6;

            if (HeaderStruct.fADCSequenceInterval == 0)
                throw new Exception("abf header hasn't been properly initialized");

            sampleRate = (int)(1e6 / HeaderStruct.fADCSequenceInterval);
            pointsPerMs = (double)sampleRate / 1000;
            channelCount = HeaderStruct.nADCNumChannels;

            sweepCount = HeaderStruct.lActualEpisodes;
            sweepLengthPoints = HeaderStruct.lNumSamplesPerEpisode / channelCount;
            sweepLengthSec = (double)sweepLengthPoints / sampleRate;
            sweepLengthMin = sweepLengthSec / 60;
            sweepIntervalSec = HeaderStruct.fEpisodeStartToStart;
            if (sweepIntervalSec == 0)
                sweepIntervalSec = sweepLengthSec;
            sweepIntervalMin = sweepIntervalSec / 60;

            totalLengthSec = sweepIntervalSec * sweepCount;
            totalLengthMin = totalLengthSec / 60;
            totalLengthPoints = sweepLengthPoints * sweepCount;

            tagCount = HeaderStruct.lNumTagEntries;
            tags = new Tag[tagCount];

            protocolFilePath = HeaderStruct.sProtocolPath.Trim();
            if (protocolFilePath == "(untitled)")
                protocolFilePath = "";
            protocol = System.IO.Path.GetFileNameWithoutExtension(protocolFilePath);

            Structs.ABFTag[] abfTags = abffio.ReadTags();
            for (int i = 0; i < abfTags.Length; i++)
            {
                ABFFIO.Structs.ABFTag abfTag = abfTags[i];
                double timeSec = abfTag.lTagTime * abffio.header.fSynchTimeUnit / 1e6;
                string comment = new string(abfTag.sComment).Trim();
                int timeSweep = (int)(timeSec / sweepIntervalSec);
                Tag tag = new(timeSec, timeSweep, comment, abfTag.nTagType);
                tags[i] = tag;
            }

            StartDateTime = ReadCreatedDateTime(HeaderStruct);
        }

        private DateTime ReadCreatedDateTime(Structs.ABFFileHeader header)
        {
            int datecode = (int)header.uFileStartDate;

            int day = datecode % 100;
            datecode /= 100;

            int month = datecode % 100;
            datecode /= 100;

            int year = datecode;

            if (year < 1980 || year >= 2080)
                throw new InvalidOperationException("unexpected creation date year in header");

            return new DateTime(year, month, day).AddMilliseconds(header.uFileStartTimeMS);
        }

        public string GetDescription()
        {
            string info = "";

            // use reflection to iterate through all public variables of this class
            var fields = this.GetType().GetFields();
            foreach (var field in fields)
            {
                // get the basic information for each item
                string type = field.FieldType.ToString();
                string name = field.Name;
                string value = field.GetValue(this).ToString();

                // customize string value of specific types
                if (field.FieldType == typeof(double))
                    value = string.Format("{0:0.0000}", (double)field.GetValue(this));
                else if (
                        field.FieldType == typeof(int) ||
                        field.FieldType == typeof(Int16) ||
                        field.FieldType == typeof(long)
                        )
                    value = string.Format("{0}", field.GetValue(this));
                else if (field.FieldType == typeof(string))
                    value = $"\"{field.GetValue(this)}\"";
                else if (field.FieldType == typeof(Tag[]))
                    foreach (Tag tag in (Tag[])field.GetValue(this))
                        value += "\n" + tag.ToString();
                else
                    name = $"({type}) {name}";

                if (value == "vsABF.Tag[]")
                    continue;

                info += $"{name}: {value}\n";
            }

            info = info.Trim();
            return info;
        }
    }
}
